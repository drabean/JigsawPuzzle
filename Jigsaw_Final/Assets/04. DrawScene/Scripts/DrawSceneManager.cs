using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum DRAWMODE
{
    Draw,
    Erase
}
public class DrawSceneManager : MonoBehaviour, IColorPicker
{
    public static DrawSceneManager Inst;

    public SpriteRenderer sp;

    [SerializeField]DRAWMODE drawMode;

    public void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(gameObject);
        cam = Camera.main;

        picker.setIColorPicker(this);
        drawMode = DRAWMODE.Draw;
    }

    private void Start()
    {
        CheckBtnStatus();

        sp.sprite = UTILS.CreateSpriteFromTexture2D(UTILS.RescaleTextureByHeight(GameData.Inst.puzzleTexture, 1632));

        spriteArea = UTILS.getSpritesArea(sp);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;


        switch (drawMode)
        {
            case DRAWMODE.Draw:
                DrawMouse();
                break;
            case DRAWMODE.Erase:
                EraseMouse();
                break;
        }
    }

    #region 그리기 관련
    private Vector3 PrevPos = Vector3.zero; // 0,0,0 position variable
    int lineCount = 0;

    Camera cam;//Gets Main Camera

    public Stack<drawCommand> lineLis = new Stack<drawCommand>();
    public Stack<drawCommand> undoLis = new Stack<drawCommand>();

    Vector3[] spriteArea;

    bool isMouseDown;

    public DrawCapture drawCapture;

    LineRenderer[] lines = new LineRenderer[10];
    int[] positionCounts = new int[10];
    void DrawMouse()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 touchPos = cam.ScreenToWorldPoint(Input.touches[i].position);
            touchPos = (Vector2)touchPos;

            if (touchPos.x > spriteArea[0].x && touchPos.x < spriteArea[1].x && touchPos.y > spriteArea[0].y && touchPos.y < spriteArea[1].y)
            {
                switch (Input.touches[i].phase)
                {
                    case TouchPhase.Ended:
                        lines[i] = null;
                        break;
                    default:
                        if (lines[i] == null)
                        {
                            lineLis.Push(new drawCommand(COMMAND.ADD, createLine(touchPos, i)));
                            CheckBtnStatus();
                        }
                        else
                        {
                            connectLine(touchPos, i);
                            CheckBtnStatus();
                        }
                        break;
                }
            }
            else
            {
                lines[i] = null;
            }
        }
    }

    /// <summary>
    /// ERASE모드일때 Ray를 쏴서 맞으면, 맞은 대상의 LineRenderer가 있는지 확인해서 있다면 그 게임오브젝트를 지움.
    /// </summary>
    void EraseMouse()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 touchPos = cam.ScreenToWorldPoint(Input.touches[i].position);
            touchPos = (Vector2)touchPos;

            if (touchPos.x > spriteArea[0].x && touchPos.x < spriteArea[1].x && touchPos.y > spriteArea[0].y && touchPos.y < spriteArea[1].y)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, transform.forward, 15f);

                    int sortingOrder = -1;
                    GameObject lineToDelete = null;

                    for (int hitCount = 0; hitCount < hits.Length; hitCount++)
                    {
                        if (hits[hitCount].collider != null)
                        {
                            if (hits[hitCount].transform.TryGetComponent<LineRenderer>(out LineRenderer line))
                            {
                                if (line.sortingLayerName.Equals("LineLayer") && line.sortingOrder > sortingOrder)
                                {
                                    lineToDelete = line.gameObject;

                                }
                            }
                        }
                    }

                    if (lineToDelete != null)
                    {
                        lineToDelete.GetComponent<LineRenderer>().sortingLayerName = "UndoLayer";

                        lineLis.Push(new drawCommand(COMMAND.DELETE, lineToDelete));
                    }



                }
                else
                {
                    lines[i] = null;
                }
            }
        }
    }
    /// <summary>
    /// LineRendrer를 생성하고, Color와 Size등의 Option을 넣어준 뒤에, 충돌감지를 위한 EdgeCollider를 추가해주는 함수.
    /// </summary>
    /// <param name="mousePos"></param>
    /// <param name="idx"></param>
    /// <returns></returns>
    GameObject createLine(Vector3 mousePos, int idx)
    {
        positionCounts[idx] = 2;
        GameObject line = new GameObject("Line");
        LineRenderer lineRend = line.AddComponent<LineRenderer>();
        EdgeCollider2D col = line.AddComponent<EdgeCollider2D>();
        col.edgeRadius = drawSize/2;
        //라인을 생성할 때 선을 그리지 않고 점만 찍으면, Collider 검출이 되지 않기 때문에 점을 처음 생성 할 때 해당 위치에 Circle Collider를 추가해준다.
        line.AddComponent<CircleCollider2D>().radius = drawSize/2;


        line.transform.parent = transform;
        line.transform.position = mousePos;
        line.transform.position = Vector3.right * mousePos.x + Vector3.up * mousePos.y + Vector3.forward * lineCount;
        col.offset = (-1) * Vector2.right * mousePos.x + (-1) * Vector2.up * mousePos.y;

        lineRend.material = defaultMaterial;
        lineRend.sortingOrder = lineCount;
        lineRend.sortingLayerName = "LineLayer";
        //size 설정
        lineRend.startWidth = drawSize;
        lineRend.endWidth = drawSize;
        //부드러운 정도 설정
        lineRend.numCornerVertices = 0;
        lineRend.numCapVertices = 5;
        //color 설정
        lineRend.startColor = curColor;
        lineRend.endColor = curColor;


        lineRend.SetPosition(0, mousePos);
        lineRend.SetPosition(1, mousePos);

        lines[idx] = lineRend;

        lineCount++;

        CheckBtnStatus();
        setEdgeCollider(lines[idx]);

        return line;
    }

    /// <summary>
    /// 이미 존재하는 LineRenderer에 점을 추가하여 선을 이어그려주는 함수.
    /// </summary>
    /// <param name="mousePos"></param>
    /// <param name="idx"></param>
    void connectLine(Vector3 mousePos, int idx)
    {
        if (PrevPos != null && Mathf.Abs(Vector3.Distance(PrevPos, mousePos)) >= 0.001f)
        {
            PrevPos = mousePos;
            positionCounts[idx]++;
            lines[idx].positionCount = positionCounts[idx];
            lines[idx].SetPosition(positionCounts[idx] - 1, mousePos);
            setEdgeCollider(lines[idx]);
        }

    }

    /// <summary>
    /// 그림을 지우기 위해 Eraser를 사용할 때, Line GameObject를 가져오게 위해 LineRerer의 각 점들을 받아와, 해당 점을 기반으로
    /// 똑같은 모양의 EdgeCollider를 만들어 줍니다.
    /// </summary>
    /// <param name="lineRend"></param>
    void setEdgeCollider(LineRenderer lineRend)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRend.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRend.GetPosition(point);
            edges.Add(Vector2.right * lineRendererPoint.x + Vector2.up * lineRendererPoint.y);
        }

        lineRend.GetComponent<EdgeCollider2D>().SetPoints(edges);
    }
    #endregion


    #region 버튼 관련

    #region 속성 바꾸기
    [Header("Line Property")]
    public float drawSize;
    public Color curColor;
    public Material defaultMaterial;

    public Color[] colors;

    public void Btn_setSize(float size)
    {
        drawMode = DRAWMODE.Draw;
        drawSize = size;
    }
    public void Btn_setColor(int index)
    {
        drawMode = DRAWMODE.Draw;
        curColor = colors[index];
    }

    public ColorPicker picker;
    public void Btn_pickColor()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");
        curColor = picker.selectedColor;
        if (!picker.gameObject.activeInHierarchy)
        {
            picker.OpenColorPicker();
        }

        else
        {
            picker.CloseColorPIcker();
        }
    }


    public void endColorPicker()
    {
        curColor = picker.selectedColor;
        drawMode = DRAWMODE.Draw;

        picker.gameObject.SetActive(false);
    }
    #endregion

    //public DrawCapture drawCapture;

    public GameObject Panel_SaveDone;
    public Image PanelImage;
    public Image BtnImage;
    public void Btn_Capture()
    {
        Panel_SaveDone.SetActive(true);
        PanelImage.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        BtnImage.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);

        UTILS.savePicture(drawCapture.Capture(sp));
        SoundManager.Inst.PlaySFX("SFX_Select");




        PanelImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        BtnImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void Btn_OpenGallary()
    {
        SoundManager.Inst.PlaySFX("SFX_Select");
        //여기바꿔야함
        NativeGallery.GetImageFromGallery((file) =>
            {
                Debug.Log("Open Gallery");
            });

        SceneManager.LoadScene("01. StartScene");
    }


    public void Btn_Undo()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");
        drawCommand temp = lineLis.Pop();
        undoLis.Push(temp);

        if (temp.command == COMMAND.ADD)
        {
            temp.obj.GetComponent<LineRenderer>().sortingLayerName = "UndoLayer";
        }
        else
        {
            temp.obj.GetComponent<LineRenderer>().sortingLayerName = "LineLayer";
        }
        CheckBtnStatus();
    }

    public void Btn_Redo()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawRedo");
        if (undoLis.Count != 0)
        {
            drawCommand temp = undoLis.Pop();
            lineLis.Push(temp);

            if (temp.command == COMMAND.ADD)
            {
                temp.obj.GetComponent<LineRenderer>().sortingLayerName = "LineLayer";
            }
            else
            {
                temp.obj.GetComponent<LineRenderer>().sortingLayerName = "UndoLayer";
            }
            CheckBtnStatus();
        }
    }

    public void Btn_Eraser()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");

        drawMode = DRAWMODE.Erase;
    }

    public Button undoBtn;
    public Button redoBtn;
    public void CheckBtnStatus()
    {
        if (lineLis.Count == 0) undoBtn.interactable = false;
        else undoBtn.interactable = true;

        if (undoLis.Count == 0) redoBtn.interactable = false;
        else redoBtn.interactable = true;
    }

    void IColorPicker.setPickerColor(Color c)
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");
        curColor = c;
    }


    #endregion
}

public enum COMMAND
{
    ADD,
    DELETE
}
public class drawCommand
{
    public COMMAND command;
    public GameObject obj;

    public drawCommand(COMMAND command, GameObject obj)
    {
        this.command = command;
        this.obj = obj;
    }
}
