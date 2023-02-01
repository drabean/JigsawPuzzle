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

        if (GameData.Inst.type != GAMETYPE.DRAW)
        {
            GameData.Inst.type = GAMETYPE.DRAW;
            sp.sprite = UTILS.CreateSpriteFromTexture2D(UTILS.RescaleTextureByWidth(GameData.Inst.puzzleTexture, 2448));
        }
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

    private LineRenderer curLine;  //Line which draws now
    EdgeCollider2D curCol;
    private int positionCount = 2;  //Initial start and end position
    private Vector3 PrevPos = Vector3.zero; // 0,0,0 position variable
    int lineCount = 0;

    Camera cam;//Gets Main Camera

    public Stack<drawCommand> lineLis = new Stack<drawCommand>();
    public Stack<drawCommand> undoLis = new Stack<drawCommand>();

    Vector3[] spriteArea;

    bool isMouseDown;

    public DrawCapture drawCapture;

    void DrawMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.3f));
        mousePos = mousePos.x * Vector3.right + mousePos.y * Vector3.up;

        if (mousePos.x > spriteArea[0].x && mousePos.x < spriteArea[1].x &&
            mousePos.y > spriteArea[0].y && mousePos.y < spriteArea[1].y)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseDown = true;
                lineLis.Push(new drawCommand(COMMAND.ADD, createLine(mousePos)));
                if (undoLis.Count != 0) undoLis.Clear();
                CheckBtnStatus();
            }
            else if (Input.GetMouseButton(0))
            {
                if (!isMouseDown) return;
                connectLine(mousePos);
                CheckBtnStatus();
            }
        }
        else
        {
            isMouseDown = false;
        }
    }

    public CircleCollider2D eraser;
    ContactFilter2D filter = new ContactFilter2D().NoFilter();
    List<Collider2D> results = new List<Collider2D>();


    void EraseMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            StartCoroutine(CO_EraseMouse());
        }
    }

    WaitForSeconds waitErase  = new WaitForSeconds(0.1f);

    IEnumerator CO_EraseMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);

        eraser.transform.position = mousePosition;
        yield return waitErase;

        eraser.OverlapCollider(filter, results);

        int check = -1;
        GameObject targetLine = null;

        for(int i = 0; i < results.Count; i++)
        {
            if(results[i].TryGetComponent<LineRenderer>(out LineRenderer line))
            {
                if(line.sortingLayerName.Equals("LineLayer"))
                {
                    if(line.sortingOrder > check)
                    {
                        targetLine = line.gameObject;
                        check = line.sortingOrder;
                    }
                }
            }
        }
        if (targetLine != null)
        {
            targetLine.GetComponent<LineRenderer>().sortingLayerName = "UndoLayer";
            lineLis.Push(new drawCommand(COMMAND.DELETE, targetLine));
        }

    }

    GameObject createLine(Vector3 mousePos)
    {
        positionCount = 2;
        GameObject line = new GameObject("Line");
        LineRenderer lineRend = line.AddComponent<LineRenderer>();
        EdgeCollider2D col = line.AddComponent<EdgeCollider2D>();
        line.AddComponent<CircleCollider2D>().radius = 10.0f;


        line.transform.parent = transform;
        line.transform.position = mousePos;
        line.transform.position = Vector3.right * mousePos.x + Vector3.up * mousePos.y + Vector3.forward * lineCount;
        col.offset = (-1)*Vector2.right * mousePos.x + (-1) * Vector2.up * mousePos.y;

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
        //neRend.

        lineRend.SetPosition(0, mousePos);
        lineRend.SetPosition(1, mousePos);

        curLine = lineRend;
        curCol = col;

        lineCount++;

        CheckBtnStatus();
        setEdgeCollider(curLine, curCol);

        return line;
    }

    void connectLine(Vector3 mousePos)
    {
        if (PrevPos != null && Mathf.Abs(Vector3.Distance(PrevPos, mousePos)) >= 0.001f)
        {
            PrevPos = mousePos;
            positionCount++;
            curLine.positionCount = positionCount;
            curLine.SetPosition(positionCount - 1, mousePos);
            setEdgeCollider(curLine, curCol);
        }

    }

    void setEdgeCollider(LineRenderer lineRenderer, EdgeCollider2D edge)
    {
        List<Vector2> edges = new List<Vector2>();

        for(int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(Vector2.right * lineRendererPoint.x + Vector2.up * lineRendererPoint.y);
        }

        edge.SetPoints(edges);
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

    public GameObject Panel_CaptureDone;
    public void Btn_Capture()
    {

#if UNITY_EDITOR
        string fileLocation = "Assets/Captures/";   // 파일의 경로 지정
#elif UNITY_ANDROID
        string fileLocation = $"/storage/emulated/0/DCIM/{Application.productName}/";   // 파일의 경로 지정
#endif
        string timeName = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");          // 날짜 설정
        string fileName = "Picture" + timeName + ".png";                                // 파일의 이름 지정
                                                                                        // string filePath = fileLocation + fileName;
        string filePath = fileLocation + fileName;


        drawCapture.setDrawCapture(sp);
        byte[] imageData = drawCapture.Capture(sp).EncodeToPNG();
#if UNITY_EDITOR
        File.WriteAllBytes(filePath, imageData);

#elif UNITY_ANDROID
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(imageData, Application.productName, fileName, (success, path) => Debug.Log("Media save result: " + success + " " + path));
#endif
            // Panel_CaptureDone.SetActive(true);



            Time.timeScale = 0;
            SceneManager.LoadScene("Additive_EndScene", LoadSceneMode.Additive);
        

    }



    public void Btn_Undo()
    {
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
        if(undoLis.Count != 0)
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
        drawMode = DRAWMODE.Erase;
    }

    public void Btn_CaptureDone()
    {
        Panel_CaptureDone.SetActive(true);
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
