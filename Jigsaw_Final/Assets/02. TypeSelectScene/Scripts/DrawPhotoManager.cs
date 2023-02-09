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

    public DrawCapture drawCapture;

    void DrawMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.3f));
        mousePos = mousePos.x * Vector3.right + mousePos.y * Vector3.up;

        if (mousePos.x > spriteArea[0].x && mousePos.x < spriteArea[1].x && mousePos.y > spriteArea[0].y && mousePos.y < spriteArea[1].y)
        {
            /*
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
            */
            if (Input.GetMouseButton(0))
            {
                if (curLine == null)
                {
                    lineLis.Push(new drawCommand(COMMAND.ADD, createLine(mousePos)));
                    CheckBtnStatus();
                }
                else
                {
                    connectLine(mousePos);
                    CheckBtnStatus();
                }
            }
            else
            {
                curLine = null;
            }
        }
        else
        {
            curLine = null;
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

    WaitForSeconds waitErase = new WaitForSeconds(0.1f);

    IEnumerator CO_EraseMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);

        eraser.transform.position = mousePosition;
        yield return waitErase;

        eraser.OverlapCollider(filter, results);

        int check = -1;
        GameObject targetLine = null;

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].TryGetComponent<LineRenderer>(out LineRenderer line))
            {
                if (line.sortingLayerName.Equals("LineLayer"))
                {
                    if (line.sortingOrder > check)
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

        for (int point = 0; point < lineRenderer.positionCount; point++)
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

    public GameObject Panel_SaveDone;
    public void Btn_Save()
    {
        Texture2D newTex = drawCapture.Capture(sp);
        UTILS.savePicture(newTex);

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

    public void Btn_MakePuzzle()
    {
        LoadSceneManager.LoadSceneAsync("03. GameScene");
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
        curColor = c;
    }

    #endregion
}
