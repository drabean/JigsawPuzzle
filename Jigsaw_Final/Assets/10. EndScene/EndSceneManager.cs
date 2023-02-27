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

    //highscore�϶��� �����ִ°�
    public GameObject HighScoreImage;


    //���� ��
    public int rewardStar;


    public Canvas endSceneCanvas;
    public Canvas fadeCanvas;
    private void Start()
    {
        endSceneCanvas.worldCamera = Camera.main;
        endSceneCanvas.sortingLayerName = "UI";

        fadeCanvas.worldCamera = Camera.main;
        fadeCanvas.sortingLayerName = "UI";

        scoreText.text = GameData.Inst.score.ToString("N2");
        //�̸��ٲ������
        //nameTMP.text = GameData.Inst.;
        //HighScore�϶� ó�� �ؾ���
        bool isHighScore = false;
        HighScoreImage.SetActive(isHighScore);


        switch(GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                switch(GameData.Inst.score)
                {
                    case < 30:
                        rewardStar = 12;
                        break;
                    case < 40:
                        rewardStar = 8;
                        break;
                    default:
                        rewardStar = 5;
                        break;

                }
                break;
            case DIFFICULTY.NORMAL:
                switch (GameData.Inst.score)
                {
                    case < 45:
                        rewardStar = 12;
                        break;
                    case < 55:
                        rewardStar = 8;
                        break;
                    default:
                        rewardStar = 5;
                        break;

                }
                break;
            case DIFFICULTY.HARD:
                switch (GameData.Inst.score)
                {
                    case < 60:
                        rewardStar = 12;
                        break;
                    case < 70:
                        rewardStar = 8;
                        break;
                    default:
                        rewardStar = 5;
                        break;

                }
                break;
            case DIFFICULTY.MASTER:
                switch (GameData.Inst.score)
                {
                    case < 100:
                        rewardStar = 12;
                        break;
                    case < 120:
                        rewardStar = 8;
                        break;
                    default:
                        rewardStar = 5;
                        break;

                }
                break;

        }

        rewardStarText.text = "X"+rewardStar;
        nameTMP.text = PlayerLogin.getPlayerData();
        //reward ���� ���� �Լ��� ���ʿ� �߰�
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
