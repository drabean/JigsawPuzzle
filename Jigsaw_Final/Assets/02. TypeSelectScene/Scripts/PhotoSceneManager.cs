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
    }

    private void Start()
    {
        sp.sprite = GameData.Inst.sp;
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






        //SceneManager.LoadScene("03. GameScene");
        StartCoroutine(CO_LoadScene("03. GameScene"));

    }

    public GameObject Panel_Loading;

    IEnumerator CO_LoadScene(string SceneName)
    {
        yield return null;
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);

        while(!async.isDone)
        {
            Panel_Loading.SetActive(true);

            yield return null;
        }
    }

}
