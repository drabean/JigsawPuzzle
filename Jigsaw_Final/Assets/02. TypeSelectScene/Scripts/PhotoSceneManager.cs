using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotoSceneManager : MonoBehaviour
{
    public static PhotoSceneManager Inst;

    [SerializeField] CutBox cutBox;
    public SpriteRenderer sp;

    private void Awake()
    {
        if (Inst == null) Inst = this;
        else Destroy(gameObject);


        //Texture2D tex = UTILS.RescaleTextureByHeight(GameData.Inst.originTexture, 1632);

        //Rect rect = new Rect(0, 0, tex.width, tex.height);

        sp.sprite = UTILS.CreateSpriteFromTexture2D(GameData.Inst.originTexture);

        if((float)GameData.Inst.originTexture.width / GameData.Inst.originTexture.height >= 1.3f)
        {
            Camera.main.orthographicSize = GameData.Inst.originTexture.width * 0.5f;
        }
        else
        {
            Camera.main.orthographicSize = GameData.Inst.originTexture.height * 0.65f;
        }

        cutBox.transform.localScale = Camera.main.orthographicSize * Vector3.one * 0.06f;
        cutBox.GetComponent<LineRenderer>().startWidth = cutBox.transform.localScale.x * 0.1f;
    }

    private void Start()
    {
        setProperty();
    }

    public void setProperty()
    {
        if (GameData.Inst == null) return;

        switch (GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                cutBox.initCutBox((float)2 / 3);
                break;
            case DIFFICULTY.NORMAL:
                cutBox.initCutBox((float)3 / 4);
                break;
            case DIFFICULTY.HARD:
                cutBox.initCutBox((float)4 / 5);
                break;
            case DIFFICULTY.MASTER:
                cutBox.initCutBox((float)5 / 6);
                break;
        }

    }



    //회전
    public void Btn_Rotate()
    {
        sp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (sp.transform.rotation.eulerAngles.z + 90) % 360));

        cutBox.setCutBox();
    }
    //가로 flip
    public void Btn_FlipHorizontal()
    {
        sp.transform.localScale = Vector3.right + sp.transform.localScale.y * (-1) * Vector3.up + Vector3.forward;
    }
    //세로flip
    public void Btn_FlipVertical()
    {
        sp.transform.localScale = sp.transform.localScale.x * (-1) * Vector3.right + Vector3.up + Vector3.forward;
    }


    public void Btn_MakePuzzle()
    {
        Texture2D newTex = cutBox.Capture();


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

        LoadSceneManager.LoadSceneAsync("03. GameScene");
    }
}
