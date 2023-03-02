using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using System.IO;
public class DrawPhotoManager : MonoBehaviour, IColorPicker
{

    public SpriteRenderer sp;

    [SerializeField] DRAWMODE drawMode;

    public void Awake()
    {
        cam = Camera.main;

        picker.setIColorPicker(this);
        drawMode = DRAWMODE.Draw;
    }

    private void Start()
    {
        CheckBtnStatus();

        switch (GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                sp.transform.localScale = new Vector3(150, 100, 10);
                break;
            case DIFFICULTY.NORMAL:
                sp.transform.localScale = new Vector3(133, 100, 10);
                break;
            case DIFFICULTY.HARD:
                sp.transform.localScale = new Vector3(125, 100, 10);
                break;
            case DIFFICULTY.MASTER:
                sp.transform.localScale = new Vector3(120, 100, 10);
                break;
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

    #region �׸��� ����
    private Vector3 PrevPos = Vector3.zero; // 0,0,0 position variable
    int lineCount = 0;

    Camera cam;//Gets Main Camera

    public Stack<drawCommand> lineLis = new Stack<drawCommand>();
    public Stack<drawCommand> undoLis = new Stack<drawCommand>();

    Vector3[] spriteArea;

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
    /// ERASE����϶� Ray�� ���� ������, ���� ����� LineRenderer�� �ִ��� Ȯ���ؼ� �ִٸ� �� ���ӿ�����Ʈ�� ����.
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

                    if(lineToDelete != null)
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
    /// LineRendrer�� �����ϰ�, Color�� Size���� Option�� �־��� �ڿ�, �浹������ ���� EdgeCollider�� �߰����ִ� �Լ�.
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
        col.edgeRadius = drawSize / 2;
        //������ ������ �� ���� �׸��� �ʰ� ���� ������, Collider ������ ���� �ʱ� ������ ���� ó�� ���� �� �� �ش� ��ġ�� Circle Collider�� �߰����ش�.
        line.AddComponent<CircleCollider2D>().radius = drawSize / 2;


        line.transform.parent = transform;
        line.transform.position = mousePos;
        line.transform.position = Vector3.right * mousePos.x + Vector3.up * mousePos.y + Vector3.forward * lineCount;
        col.offset = (-1) * Vector2.right * mousePos.x + (-1) * Vector2.up * mousePos.y;

        lineRend.material = defaultMaterial;
        lineRend.sortingOrder = lineCount;
        lineRend.sortingLayerName = "LineLayer";
        //size ����
        lineRend.startWidth = drawSize;
        lineRend.endWidth = drawSize;
        //�ε巯�� ���� ����
        lineRend.numCornerVertices = 0;
        lineRend.numCapVertices = 5;
        //color ����
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
    /// �̹� �����ϴ� LineRenderer�� ���� �߰��Ͽ� ���� �̾�׷��ִ� �Լ�.
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
    /// �׸��� ����� ���� Eraser�� ����� ��, Line GameObject�� �������� ���� LineRerer�� �� ������ �޾ƿ�, �ش� ���� �������
    /// �Ȱ��� ����� EdgeCollider�� ����� �ݴϴ�.
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


    #region ��ư ����

    #region �Ӽ� �ٲٱ�
    [Header("Line Property")]
    public float drawSize;
    public Color curColor;
    public Material defaultMaterial;

    public Color[] colors;

    /// <summary>
    /// LineRenderer�� �����Ҷ� Line�� �β�.
    /// </summary>
    /// <param name="size"></param>
    public void Btn_setSize(float size)
    {
        drawMode = DRAWMODE.Draw;
        drawSize = size;
    }
    /// <summary>
    /// LineRenderer�� �����Ҷ� Line�� ����.
    /// </summary>
    /// <param name="index"></param>
    public void Btn_setColor(int index)
    {
        drawMode = DRAWMODE.Draw;
        curColor = colors[index];
    }

    public ColorPicker picker;
    /// <summary>
    /// ColorPicker�� ���� ��ư.
    /// </summary>
    public void Btn_pickColor()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");

        curColor = picker.selectedColor;
        if (!picker.gameObject.activeInHierarchy)
        {
            picker.OpenColorPicker();
            drawMode = DRAWMODE.Draw;
        }

        else
        {
            picker.CloseColorPIcker();
            drawMode = DRAWMODE.Draw;
        }
    }

    /// <summary>
    /// ColorPicker �󿡼� ���콺�� ������, ���콺�� �� ��ġ�� ������ ������
    /// </summary>
    public void endColorPicker()
    {
        curColor = picker.selectedColor;
        drawMode = DRAWMODE.Draw;

        picker.gameObject.SetActive(false);
    }
    #endregion

    //public DrawCapture drawCapture;

    public GameObject Panel_SaveDone;
    /// <summary>
    /// ȭ��󿡼� sprite ������ŭ ĸ���ؼ� ���Ϸ� ����
    /// </summary>
    public void Btn_Save()
    {
        SoundManager.Inst.PlaySFX("SFX_Select");

        Texture2D newTex = drawCapture.Capture(sp);

        savePicture(newTex);

        switch (GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                newTex = UTILS.RescaleTexture(newTex, Tile.TileSize * 3, Tile.TileSize * 2);
                break;
            case DIFFICULTY.NORMAL:
                newTex = UTILS.RescaleTexture(newTex, Tile.TileSize * 4, Tile.TileSize * 3);
                break;
            case DIFFICULTY.HARD:
                newTex = UTILS.RescaleTexture(newTex, Tile.TileSize * 5, Tile.TileSize * 4);
                break;
            case DIFFICULTY.MASTER:
                newTex = UTILS.RescaleTexture(newTex, Tile.TileSize * 6, Tile.TileSize * 5);
                break;
        }

        GameData.Inst.puzzleTexture = newTex;
        Panel_SaveDone.SetActive(true);

    }

    public void savePicture(Texture2D tex)
    {

#if UNITY_EDITOR
        string fileLocation = "Assets/Captures/";   // ������ ��� ����
#elif UNITY_ANDROID
        string fileLocation = $"/storage/emulated/0/DCIM/{Application.productName}/";   // ������ ��� ����
#endif
        string timeName = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");          // ��¥ ����
        string fileName = "Picture" + timeName + ".png";                                // ������ �̸� ����
                                                                                        // string filePath = fileLocation + fileName;
        string filePath = fileLocation + fileName;

        if (!Directory.Exists(fileLocation)) Directory.CreateDirectory(fileLocation);

        byte[] imageData = tex.EncodeToPNG();
#if UNITY_EDITOR
        File.WriteAllBytes(filePath, imageData);

#elif UNITY_ANDROID
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(imageData, Application.productName, fileName, (success, path) => Debug.Log("Media save result: " + success + " " + path));
#endif
    }

    /// <summary>
    /// GameScene���� �̵�
    /// </summary>
    public void Btn_MakePuzzle()
    {
        SoundManager.Inst.PlaySFX("SFX_Select");
        LoadSceneManager.LoadSceneAsync("03. GameScene");
    }



    /// <summary>
    /// �ǵ�����
    /// </summary>
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

    /// <summary>
    /// �ٽ��ϱ�
    /// </summary>
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

    /// <summary>
    /// ���� mode�� Erase�� �ٲ������μ�, ������ ��ġ�ؼ� ����� ���� ����.
    /// </summary>
    public void Btn_Eraser()
    {
        SoundManager.Inst.PlaySFX("SFX_DrawUndo");
        drawMode = DRAWMODE.Erase;
    }



    /// <summary>
    /// ���� undo�Ұ� �ִ���, redo�Ұ� �ִ����� �Ǵ��Ͽ� undo, redo��ư ��Ȱ��ȭ������
    /// </summary>
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
        drawMode = DRAWMODE.Draw;
    }

    #endregion
}
