using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    [SerializeField] Text scoreText;
    [SerializeField] Text rewardStarText;

    //highscore일때만 보여주는거
    public GameObject HighScoreImage;


    //보상 별
    public int rewardStar;


    public Canvas endSceneCanvas;
    public Canvas fadeCanvas;

    public InGameRanking inGameRanking = null;

    private void Start()
    {
        endSceneCanvas.worldCamera = Camera.main;
        endSceneCanvas.sortingLayerName = "UI";

        fadeCanvas.worldCamera = Camera.main;
        fadeCanvas.sortingLayerName = "UI";

        scoreText.text = GameData.Inst.score.ToString("N2");
        //이름바꿔줘야함
        //nameTMP.text = GameData.Inst.;
        //HighScore일때 처리 해아함
        bool isHighScore = false;
        HighScoreImage.SetActive(isHighScore);

        List<string> rewardTable = CSVUTILS.LoadData("rewardTable");

        string[] rewards = rewardTable[(int)GameData.Inst.difficulty + 1].Split(",");

        if (GameData.Inst.score < int.Parse(rewards[1]))
        {
            rewardStar = 12;
        }
        else if (GameData.Inst.score < int.Parse(rewards[2]))
        {
            rewardStar = 8;
        }
        else
        {
            rewardStar = 5;
        }


        rewardStarText.text = "X"+rewardStar;

        inGameRanking.EndingSceneSequnce(GameData.Inst.score,rewardStar,3,GameData.Inst.difficulty);
    }

    public void ExitRoom()
    {
        if (GameData.Inst.type == GAMETYPE.DRAW)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("01. StartScene");
        }
        else
        {
            Time.timeScale = 1;
            LoadSceneManager.LoadSceneAsync("04. DrawScene");
        }
    }


}
