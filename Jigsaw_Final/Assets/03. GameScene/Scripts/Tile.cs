using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile
{
    #region Static 변수들( 타일 하나당 크기, padding 크기 등)

    // 실제 타일 사이즈
    public static int TileSize = 200;
    //퍼즐 튀어나오는 부분을 위한 Padding
    public static int Padding = TileSize/5;

    // 베지어 커브 생성을 위한 point.
    public static readonly List<Vector2> BezierPoints = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(35, 15),
            new Vector2(47, 13),
            new Vector2(45, 5),
            new Vector2(48, 0),
            new Vector2(25, -5),
            new Vector2(15, -18),
            new Vector2(36, -20),
            new Vector2(64, -20),
            new Vector2(85, -18),
            new Vector2(75, -5),
            new Vector2(52, 0),
            new Vector2(55, 5),
            new Vector2(53, 13),
            new Vector2(65, 15),
            new Vector2(100, 0)
        };

    // bezierPoints를 기반으로 생성된 point의 리스트.
    public static List<Vector2> BezCurve = BezierCurve.PointList2(BezierPoints, 0.0001f);

    
    public static readonly Color InitColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);//퍼즐 초기화 색(투명)
    public static readonly Color EdgeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);//퍼즐 모서리의 색
    #endregion

    #region Enums
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
    public enum PosNegType
    {
        POS,
        NEG,
        NONE,
    }
    #endregion

    public Texture2D FinalCut { get; private set; }//잘라낸 퍼즐모양.

    //불러온 원본 텍스쳐
    private Texture2D baseTex;

    //상하좌우 각 4 방향에 대한 직선/오목/볼록 값
    private PosNegType[] mPosNeg = new PosNegType[4]
    {
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE
    };

    //flood fill에 사용하는 visited 배열
    private bool[,] pixelVisited;

    //flood fill에 사용하는 방문 스택
    private Stack<Vector2Int> visitStack = new Stack<Vector2Int>();

    //이 tile의 퍼즐 판에서의 위치.
    public int xIndex = 0;
    public int yIndex = 0;

    #region Public Methods

    // 생성자에서 baseTex를 받음.
    public Tile(Texture2D tex)
    {
        int tileSizeWithPadding = 2 * Padding + TileSize;

        baseTex = tex;

        //만들 퍼즐모양 텍스쳐
        FinalCut = new Texture2D(tileSizeWithPadding, tileSizeWithPadding, TextureFormat.ARGB32, false);

        // FinalCut을 우선 초기화해줌.(투명색)
        for (int i = 0; i < tileSizeWithPadding; ++i)
        {
            for (int j = 0; j < tileSizeWithPadding; ++j)
            {
                FinalCut.SetPixel(i, j, InitColor);
            }
        }
    }

    /// <summary>
    /// 4분위중 해당 방향을 직선/오목/볼록중 하나로 설정
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    public void SetPosNegType(Direction dir, PosNegType type)
    {
        mPosNeg[(int)dir] = type;
    }

    /// <summary>
    /// 해당 타일의 dir 방향 모양을 반환합니다.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public PosNegType GetPosNegType(Direction dir)
    {
        return mPosNeg[(int)dir];
    }

    /// <summary>
    /// 세팅된 퍼즐 모양과 원본 Texture를 기반으로 새로운 퍼즐모양 Texture를 생성
    /// </summary>
    public void Apply()
    {
        FloodFillInit();
        FloodFill();
        setOutline();
        FinalCut.Apply();
    }
    #endregion

    #region Tile로부터 실제 오브젝트 생성 스크립트
    /// <summary>
    /// Tile 스크립트로부터 실제 퍼즐조각 게임 오브젝트를 생성하는 함수
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static GameObject CreatePuzzlePeaceFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "PuzzlePeace_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

        obj.transform.position = new Vector3(tile.xIndex * TileSize, tile.yIndex * TileSize, 0.0f);

        SpriteRenderer sp = obj.AddComponent<SpriteRenderer>();

        // final cut 에서 sprite를 생성
        sp.sprite = UTILS.CreateSpriteFromTexture2D(tile.FinalCut, 0, 0, Padding * 2 + TileSize, Padding * 2 + TileSize, 1, SpriteMeshType.Tight);
        sp.sortingOrder = 1;
        obj.AddComponent<BoxCollider2D>().size = Vector2.one * TileSize;

        obj.AddComponent<TileMovement>().tile = tile;
        return obj;
    }

    /// <summary>
    /// Tile 스크립트로부터 반투명한 뒷배경(퍼즐) 오브젝트 생성
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static GameObject CreatePuzzlePeaceBackFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "BackPeace_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

        obj.transform.position = new Vector3(tile.xIndex * TileSize, tile.yIndex * TileSize, 0.0f);

        SpriteRenderer sp = obj.AddComponent<SpriteRenderer>();

        // final cut 에서 sprite를 생성
        sp.sprite = UTILS.CreateSpriteFromTexture2D(tile.FinalCut, 0, 0, Padding * 2 + TileSize, Padding * 2 + TileSize, 1, SpriteMeshType.Tight);
        sp.sortingOrder = -1;
        sp.color = new Color(1, 1, 1, 0.3f);
        return obj;
    }
    #endregion
    //퍼즐조각을 잘라낼 원본 이미지에서 x,y 위치의 픽셀을 복사해 저장.
    void SetPixel(int x, int y)
    {
        Color c = baseTex.GetPixel(x + xIndex * TileSize, y + yIndex * TileSize);
        FinalCut.SetPixel(x, y, c);
    }

    List<Vector2Int> outlinePoints = new List<Vector2Int>();

    //퍼즐조각을 잘라낼 기준이 될 선을 그림.
    void FloodFillInit()
    {
        int tileSizeWithPadding = 2 * Padding + TileSize;

        pixelVisited = new bool[tileSizeWithPadding, tileSizeWithPadding];

        for (int i = 0; i < tileSizeWithPadding; ++i)
        {
            for (int j = 0; j < tileSizeWithPadding; ++j)
            {
                pixelVisited[i, j] = false;
            }
        }

        for (int i = 0; i < mPosNeg.Length; ++i)
        {
            //각 4방향에 대해 curve 그리기.
            outlinePoints.AddRange(CreateCurve((Direction)i, mPosNeg[i]));
            
        }
        outlinePoints.Distinct();
        // curve에 겹치는 픽셀 좌표들을 이미 방문한것으로 판정해서, 커브 이후 영역들을 칠하지 않게 만듬
        for (int i = 0; i < outlinePoints.Count; ++i)
        {
            pixelVisited[(int)outlinePoints[i].x, (int)outlinePoints[i].y] = true;

        }

        //탐색큐에 중앙좌표 넣어서 중앙부터 칠하기 시작.
        Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);

        pixelVisited[start.x, start.y] = true;
        visitStack.Push(start);
    }
    //선을 기준으로 Flood Fill을 통해 퍼즐조각 텍스쳐를 생성
    void FloodFill()
    {
        int width_height = Padding * 2 + TileSize;

        //flood fill을 통해 퍼즐을 복사해올 영역 구함
        while (visitStack.Count > 0)
        {
            //스택에서 좌표 하나 가지고옴
            Vector2Int curPos = visitStack.Pop();

            //해당 좌표에 실제로 픽셀 채우는부분
            int curX = curPos.x;
            int curY = curPos.y;
            SetPixel(curPos.x, curPos.y);

            //좌표 오른쪽 검사
            int checkX = curX + 1;
            int checkY = curY;
            if (checkX < width_height)
            {
                if (!pixelVisited[checkX, checkY])
                {
                    pixelVisited[checkX, checkY] = true;
                    visitStack.Push(new Vector2Int(checkX, checkY));
                }
            }

            //왼쪽
            checkX = curX - 1;
            checkY = curY;
            if (checkX >= 0)
            {
                if (!pixelVisited[checkX, checkY])
                {
                    pixelVisited[checkX, checkY] = true;
                    visitStack.Push(new Vector2Int(checkX, checkY));
                }
            }

            // 위쪽
            checkX = curX;
            checkY = curY + 1;
            if (checkY < width_height)
            {
                if (!pixelVisited[checkX, checkY])
                {
                    pixelVisited[checkX, checkY] = true;
                    visitStack.Push(new Vector2Int(checkX, checkY));
                }
            }

            // 아래쪽
            checkX = curX;
            checkY = curY - 1;
            if (checkY >= 0)
            {
                if (!pixelVisited[checkX, checkY])
                {
                    pixelVisited[checkX, checkY] = true;
                    visitStack.Push(new Vector2Int(checkX, checkY));
                }
            }
        }
    }

    List<Vector2Int> CreateCurve(Direction dir, PosNegType type)
    {
        List<Vector2> points = new List<Vector2>(BezCurve);
        //BezierCurve가 x가 0~100인 구간에 대해서 생성되었으므로, tileSize/100만큼 BezierCurve의 값을 키워준다.

        for(int i = 0; i < BezCurve.Count;i++)
        {
            points[i] *= TileSize / 100;
        }

        switch (dir)
        {
            case Direction.UP:
                if (type == PosNegType.POS)
                {
                    TranslatePoints(points, new Vector3(Padding, Padding + TileSize, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    InvertY(points);
                    TranslatePoints(points, new Vector3(Padding, Padding + TileSize, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    points.Clear();
                    for (int i = 0; i <TileSize; ++i)
                    {
                        points.Add(new Vector2(i + Padding, Padding + TileSize));
                    }
                }
                break;

            case Direction.RIGHT:
                if (type == PosNegType.POS)
                {
                    SwapXY(points);
                    TranslatePoints(points, new Vector3(Padding + TileSize, Padding, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    InvertY(points);
                    SwapXY(points);
                    TranslatePoints(points, new Vector3(Padding + TileSize, Padding, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    points.Clear();
                    for (int i = 0; i < TileSize; ++i)
                    {
                        points.Add(new Vector2(Padding + TileSize, i + Padding));
                    }
                }
                break;

            case Direction.DOWN:
                if (type == PosNegType.POS)
                {
                    InvertY(points);
                    TranslatePoints(points, new Vector3(Padding, Padding, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    TranslatePoints(points, new Vector3(Padding, Padding, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    points.Clear();
                    for (int i = 0; i < TileSize; ++i)
                    {
                        points.Add(new Vector2(i + Padding, Padding));
                    }
                }
                break;

            case Direction.LEFT:
                if (type == PosNegType.POS)
                {
                    InvertY(points);
                    SwapXY(points);
                    TranslatePoints(points, new Vector3(Padding, Padding, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    SwapXY(points);
                    TranslatePoints(points, new Vector3(Padding, Padding, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    points.Clear();
                    for (int i = 0; i < TileSize; ++i)
                    {
                        points.Add(new Vector2(Padding, i + Padding));
                    }
                }
                break;
        }

        List<Vector2Int> lis = new List<Vector2Int>();
        for(int i = 0; i < points.Count;i++)
        {
            lis.Add(Vector2Int.FloorToInt(points[i]));
        }

        lis = UTILS.removeDuplicates(lis);
        return lis;
    }

    //outline 생성 및 퍼즐 텍스쳐 자체에 AntiAliasing 적용
    void setOutline()
    {
        for (int i = 0; i < outlinePoints.Count; i++)
        {
            FinalCut.SetPixel((int)outlinePoints[i].x, (int)outlinePoints[i].y, EdgeColor);

            //Edge 인접 픽셀에 연한색 넣어주기(퍼즐 조각 자체에 AntiAliasing 적용)
            for (int difX = -1; difX < 2; difX++)
            {
                for (int difY = -1; difY < 2; difY++)
                {
                    if (difX == 0 && difY == 0) continue;

                    Color c = FinalCut.GetPixel((int)outlinePoints[i].x + difX, (int)outlinePoints[i].y + difY);

                    //아예 clear인 외각선 바깥부분일경우, 반투명 회색으로 채워준다.
                    if (c == Color.clear) c = new Color(0.6f, 0.6f, 0.6f, 0.2f);
                    else
                    {
                        //퍼즐의 안부분
                        if (c.a == 1)
                        {
                            c = new Color(c.r - 0.15f, c.g - 0.15f, c.b -0.15f);
                        }
                        //외각선 바깥부분
                        else
                        {
                            c = new Color(c.r - 0.3f, c.g - 0.3f, c.b - 0.3f, c.a + 0.3f);
                        }
                    }

                    FinalCut.SetPixel((int)outlinePoints[i].x+difX, (int)outlinePoints[i].y+difY, c);
                }
            }
        }
    }

    #region BezierCurve Points의 위치 / 각도 변경
    static void TranslatePoints(List<Vector2> iList, Vector2 offset)
    {
        for (int i = 0; i < iList.Count; ++i)
        {
            iList[i] += offset;
        }
    }

    static void InvertY(List<Vector2> iList)
    {
        for (int i = 0; i < iList.Count; ++i)
        {
            iList[i] = new Vector2(iList[i].x, -iList[i].y);
        }
    }

    static void SwapXY(List<Vector2> iList)
    {
        for (int i = 0; i < iList.Count; ++i)
        {
            iList[i] = new Vector2(iList[i].y, iList[i].x);
        }
    }
    #endregion
}

