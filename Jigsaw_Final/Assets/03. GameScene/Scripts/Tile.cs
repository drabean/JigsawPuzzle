using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile
{
    #region Static ������( Ÿ�� �ϳ��� ũ��, padding ũ�� ��)

    // ���� Ÿ�� ������
    public static int TileSize = 200;
    //���� Ƣ����� �κ��� ���� Padding
    public static int Padding = TileSize/5;

    // ������ Ŀ�� ������ ���� point.
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

    // bezierPoints�� ������� ������ point�� ����Ʈ.
    public static List<Vector2> BezCurve = BezierCurve.PointList2(BezierPoints, 0.0001f);

    
    public static readonly Color InitColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);//���� �ʱ�ȭ ��(����)
    public static readonly Color EdgeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);//���� �𼭸��� ��
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

    public Texture2D FinalCut { get; private set; }//�߶� ������.

    //�ҷ��� ���� �ؽ���
    private Texture2D baseTex;

    //�����¿� �� 4 ���⿡ ���� ����/����/���� ��
    private PosNegType[] mPosNeg = new PosNegType[4]
    {
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE
    };

    //flood fill�� ����ϴ� visited �迭
    private bool[,] pixelVisited;

    //flood fill�� ����ϴ� �湮 ����
    private Stack<Vector2Int> visitStack = new Stack<Vector2Int>();

    //�� tile�� ���� �ǿ����� ��ġ.
    public int xIndex = 0;
    public int yIndex = 0;

    #region Public Methods

    // �����ڿ��� baseTex�� ����.
    public Tile(Texture2D tex)
    {
        int tileSizeWithPadding = 2 * Padding + TileSize;

        baseTex = tex;

        //���� ������ �ؽ���
        FinalCut = new Texture2D(tileSizeWithPadding, tileSizeWithPadding, TextureFormat.ARGB32, false);

        // FinalCut�� �켱 �ʱ�ȭ����.(�����)
        for (int i = 0; i < tileSizeWithPadding; ++i)
        {
            for (int j = 0; j < tileSizeWithPadding; ++j)
            {
                FinalCut.SetPixel(i, j, InitColor);
            }
        }
    }

    /// <summary>
    /// 4������ �ش� ������ ����/����/������ �ϳ��� ����
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    public void SetPosNegType(Direction dir, PosNegType type)
    {
        mPosNeg[(int)dir] = type;
    }

    /// <summary>
    /// �ش� Ÿ���� dir ���� ����� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public PosNegType GetPosNegType(Direction dir)
    {
        return mPosNeg[(int)dir];
    }

    /// <summary>
    /// ���õ� ���� ���� ���� Texture�� ������� ���ο� ������ Texture�� ����
    /// </summary>
    public void Apply()
    {
        FloodFillInit();
        FloodFill();
        setOutline();
        FinalCut.Apply();
    }
    #endregion

    #region Tile�κ��� ���� ������Ʈ ���� ��ũ��Ʈ
    /// <summary>
    /// Tile ��ũ��Ʈ�κ��� ���� �������� ���� ������Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static GameObject CreatePuzzlePeaceFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "PuzzlePeace_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

        obj.transform.position = new Vector3(tile.xIndex * TileSize, tile.yIndex * TileSize, 0.0f);

        SpriteRenderer sp = obj.AddComponent<SpriteRenderer>();

        // final cut ���� sprite�� ����
        sp.sprite = UTILS.CreateSpriteFromTexture2D(tile.FinalCut, 0, 0, Padding * 2 + TileSize, Padding * 2 + TileSize, 1, SpriteMeshType.Tight);
        sp.sortingOrder = 1;
        obj.AddComponent<BoxCollider2D>().size = Vector2.one * TileSize;

        obj.AddComponent<TileMovement>().tile = tile;
        return obj;
    }

    /// <summary>
    /// Tile ��ũ��Ʈ�κ��� �������� �޹��(����) ������Ʈ ����
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static GameObject CreatePuzzlePeaceBackFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "BackPeace_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

        obj.transform.position = new Vector3(tile.xIndex * TileSize, tile.yIndex * TileSize, 0.0f);

        SpriteRenderer sp = obj.AddComponent<SpriteRenderer>();

        // final cut ���� sprite�� ����
        sp.sprite = UTILS.CreateSpriteFromTexture2D(tile.FinalCut, 0, 0, Padding * 2 + TileSize, Padding * 2 + TileSize, 1, SpriteMeshType.Tight);
        sp.sortingOrder = -1;
        sp.color = new Color(1, 1, 1, 0.3f);
        return obj;
    }
    #endregion
    //���������� �߶� ���� �̹������� x,y ��ġ�� �ȼ��� ������ ����.
    void SetPixel(int x, int y)
    {
        Color c = baseTex.GetPixel(x + xIndex * TileSize, y + yIndex * TileSize);
        FinalCut.SetPixel(x, y, c);
    }

    List<Vector2Int> outlinePoints = new List<Vector2Int>();

    //���������� �߶� ������ �� ���� �׸�.
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
            //�� 4���⿡ ���� curve �׸���.
            outlinePoints.AddRange(CreateCurve((Direction)i, mPosNeg[i]));
            
        }
        outlinePoints.Distinct();
        // curve�� ��ġ�� �ȼ� ��ǥ���� �̹� �湮�Ѱ����� �����ؼ�, Ŀ�� ���� �������� ĥ���� �ʰ� ����
        for (int i = 0; i < outlinePoints.Count; ++i)
        {
            pixelVisited[(int)outlinePoints[i].x, (int)outlinePoints[i].y] = true;

        }

        //Ž��ť�� �߾���ǥ �־ �߾Ӻ��� ĥ�ϱ� ����.
        Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);

        pixelVisited[start.x, start.y] = true;
        visitStack.Push(start);
    }
    //���� �������� Flood Fill�� ���� �������� �ؽ��ĸ� ����
    void FloodFill()
    {
        int width_height = Padding * 2 + TileSize;

        //flood fill�� ���� ������ �����ؿ� ���� ����
        while (visitStack.Count > 0)
        {
            //���ÿ��� ��ǥ �ϳ� �������
            Vector2Int curPos = visitStack.Pop();

            //�ش� ��ǥ�� ������ �ȼ� ä��ºκ�
            int curX = curPos.x;
            int curY = curPos.y;
            SetPixel(curPos.x, curPos.y);

            //��ǥ ������ �˻�
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

            //����
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

            // ����
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

            // �Ʒ���
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
        //BezierCurve�� x�� 0~100�� ������ ���ؼ� �����Ǿ����Ƿ�, tileSize/100��ŭ BezierCurve�� ���� Ű���ش�.

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

    //outline ���� �� ���� �ؽ��� ��ü�� AntiAliasing ����
    void setOutline()
    {
        for (int i = 0; i < outlinePoints.Count; i++)
        {
            FinalCut.SetPixel((int)outlinePoints[i].x, (int)outlinePoints[i].y, EdgeColor);

            //Edge ���� �ȼ��� ���ѻ� �־��ֱ�(���� ���� ��ü�� AntiAliasing ����)
            for (int difX = -1; difX < 2; difX++)
            {
                for (int difY = -1; difY < 2; difY++)
                {
                    if (difX == 0 && difY == 0) continue;

                    Color c = FinalCut.GetPixel((int)outlinePoints[i].x + difX, (int)outlinePoints[i].y + difY);

                    //�ƿ� clear�� �ܰ��� �ٱ��κ��ϰ��, ������ ȸ������ ä���ش�.
                    if (c == Color.clear) c = new Color(0.6f, 0.6f, 0.6f, 0.2f);
                    else
                    {
                        //������ �Ⱥκ�
                        if (c.a == 1)
                        {
                            c = new Color(c.r - 0.15f, c.g - 0.15f, c.b -0.15f);
                        }
                        //�ܰ��� �ٱ��κ�
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

    #region BezierCurve Points�� ��ġ / ���� ����
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

