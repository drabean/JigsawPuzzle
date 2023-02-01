using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator_Board : MonoBehaviour
{
    Texture2D originImage;//원본 이미지.

    //불러온 원본 파일
    Texture2D origin_Padding;
    public GameObject BackBoard;

    public int tileCountX { get; private set; }
    public int tileCountY { get; private set; }

    Tile[,] tilesLis = null;
    GameObject[,] tileObjectsLis = null;

    public void startGeneratingPuzzle()
    {
        origin_Padding = LoadBaseTexture();

        //여기에 퍼즐 뒷배경까지 넣기

        SpriteRenderer sr = BackBoard.GetComponent<SpriteRenderer>();
        sr.sprite = UTILS.CreateSpriteFromTexture2D(origin_Padding);
        sr.sortingOrder = -9;

        BackBoard.transform.position = Vector2.right * ((origin_Padding.width / Tile.TileSize-1) * Tile.TileSize/2) + Vector2.up * ((origin_Padding.height / Tile.TileSize-1) * Tile.TileSize / 2) ;

        CreateJigsawTiles();
    }

    /// <summary>
    /// texture를 불러오고, Padding을 추가한 Sprite를 반환하는 함수입니다.
    /// </summary>
    /// <returns></returns>
    Texture2D LoadBaseTexture()
    {
        originImage = GameData.Inst.puzzleTexture;
        Texture2D newTex = new Texture2D(originImage.width + Tile.Padding * 2, originImage.height + Tile.Padding * 2, TextureFormat.ARGB32, false);

        // newtex를 일단 흰색으로 초기화.
        for (int x = 0; x < originImage.width + Tile.Padding * 2; ++x)
        {
            for (int y = 0; y < originImage.height + Tile.Padding * 2; ++y)
            {
                newTex.SetPixel(x, y, Color.white);
            }
        }

        // padding영역을 제외한 부분에 orinImage의 픽셀들을 복사.
        for (int x = 0; x < originImage.width; ++x)
        {
            for (int y = 0; y < originImage.height; ++y)
            {
                Color color = originImage.GetPixel(x, y);
                color.a = 1.0f;
                newTex.SetPixel(x + Tile.Padding, y + Tile.Padding, color);
            }
        }
        newTex.Apply();

        return newTex;
    }

    /// <summary>
    /// oring_padding으로부터 직소퍼즐 조각들을 만드는 함수
    /// </summary>
    void CreateJigsawTiles()
    {
        Texture2D baseTexture = origin_Padding;

        tileCountX = baseTexture.width / Tile.TileSize;
        tileCountY = baseTexture.height / Tile.TileSize;

        tilesLis = new Tile[tileCountX, tileCountY];

        tileObjectsLis = new GameObject[tileCountX, tileCountY];

        for (int i = 0; i < tileCountX; ++i)
        {
            for (int j = 0; j < tileCountY; ++j)
            {
                Tile tile = new Tile(baseTexture);

                tile.xIndex = i;
                tile.yIndex = j;

                if (i == 0)
                {
                    //가장 왼쪽에 있는 퍼즐 조각의 왼쪽 면을 직선으로 만듬
                    tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
                }
                else
                {
                    //자신의 왼쪽에 있는 타일의 모양을 확인
                    Tile leftTile = tilesLis[i - 1, j];
                    Tile.PosNegType rightOp = leftTile.GetPosNegType(Tile.Direction.RIGHT);
                    tile.SetPosNegType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                if (j == 0)
                {
                    //가장 아래 있는 퍼즐 조각의 아래 면을 직선으로 만듬.
                    tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
                }
                else
                {
                    //자신의 아래 타일의 모양을 확인
                    Tile downTile = tilesLis[i, j - 1];
                    Tile.PosNegType rightOp = downTile.GetPosNegType(Tile.Direction.UP);
                    tile.SetPosNegType(Tile.Direction.DOWN, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                if (i == tileCountX - 1)
                {
                    //가장 오른쪽에 있는 퍼즐 조각의 오른쪽 면을 직선으로 만듬.
                    tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
                }
                else
                {
                    //오른쪽 면의 모양을 랜덤으로 정함.(왼쪽부터 채우기 시작하므로.)
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
                    }
                }

                if (j == tileCountY - 1)
                {
                    //가장 위쪽에 있는 퍼즐 조각의 위쪽 면을 직선으로 만듬.
                    tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NONE);
                }
                else
                {
                    //위쪽 면의 모양을 랜덤으로 정함(아래쪽부터 채우기 시작하므로)
                    int rand = Random.Range(0, 2);

                    if (rand == 0)
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NEG);
                    }
                }

                //만들어진 모양대로  Tile Texture 생성
                tile.Apply();

                tilesLis[i, j] = tile;

                //tile에서 실제 게임 오브젝트를 Instantiate함.
                tileObjectsLis[i, j] = Tile.CreatePuzzlePeaceFromTile(tile);
                tileObjectsLis[i, j].transform.SetParent(transform);

                //tile에서 뒷 배경이 될 퍼즐조각을 Instantiate함.
                Tile.CreatePuzzlePeaceBackFromTile(tile).transform.SetParent(transform);
            }
        }
        initSpritesPositions();
    }


    #region 퍼즐 생성 후 직소퍼즐 초기배치

    //public GameObject[] TilesOffsetPrefab;

    public GameObject[] initPositionsPrefab;
    List<SpriteRenderer> sps = new List<SpriteRenderer>();
    Transform[] tilePositions;
    public void initSpritesPositions()
    {
        foreach(GameObject obj in tileObjectsLis)
        {
            sps.Add(obj.GetComponent<SpriteRenderer>());
        }

        setProperty();

        StartCoroutine(CO_InitSpritesPositions());
    }

    IEnumerator CO_InitSpritesPositions()
    {
        SpriteRenderer temp;

        int randNum;

        //저장된 spriteRenderer의 순서를 섞음
        for (int i = 0; i < sps.Count; i++)
        {
            temp = sps[i];
            randNum = Random.Range(0, sps.Count);
            sps[i] = sps[randNum];
            sps[randNum] = temp;
        }

        


        //1초동안 기다린 뒤에 퍼즐 조각을 퍼트림
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < sps.Count; i++)
        {
            sps[i].sortingOrder = i + 1;
            Vector2 moveVec = Vector2.right * (tilePositions[i+1].position.x) + Vector2.up * (tilePositions[i + 1].position.y);

            sps[i].GetComponent<TileMovement>().Move_Time(moveVec, 0.8f);
        }

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < sps.Count; i++)
        {
            sps[i].GetComponent<TileMovement>().canInteract = true;
        }

    }

    public void setProperty()
    {
        if (GameData.Inst == null) return;

        switch (GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                tilePositions = initPositionsPrefab[0].GetComponentsInChildren<Transform>();
                break;
            case DIFFICULTY.NORMAL:
                tilePositions = initPositionsPrefab[1].GetComponentsInChildren<Transform>();
                break;
            case DIFFICULTY.HARD:
                tilePositions = initPositionsPrefab[2].GetComponentsInChildren<Transform>();
                break;
            case DIFFICULTY.MASTER:
                tilePositions = initPositionsPrefab[3].GetComponentsInChildren<Transform>();
                break;
        }



    }

    #endregion


}
