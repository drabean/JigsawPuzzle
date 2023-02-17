using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTMP;
    [SerializeField] Text scoreText;
    [SerializeField] Text rewardStarText;

    //highscore일때만 보여주는거
    public GameObject HighScoreImage;


    public int rewardStar;


    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerName = "UI";


        scoreText.text = GameData.Inst.score.ToString("N2");
        //이름바꿔줘야함
        //nameTMP.text = GameData.Inst.;
        //HighScore일때 처리 해아함
        bool isHighScore = false;
        HighScoreImage.SetActive(isHighScore);


        //보상 변경은 여기서 
        switch(GameData.Inst.score)
        {
            case (< 40):
                rewardStar = 12;
                break;
            case (<50):
                rewardStar = 8;
                break;
            case (<70):
                rewardStar = 5;
                break;

        }

        rewardStarText.text = "X"+rewardStar;
        //reward 실제 변경 함수는 이쪽에 추가
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
