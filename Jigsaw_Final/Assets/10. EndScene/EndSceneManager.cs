using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;

    public int rewardStar;

    private void Start()
    {
        Time.timeScale = 0;
        scoreTMP.text = GameData.Inst.score.ToString("N2");


        switch(GameData.Inst.score)
        {
            case (< 50):
                rewardStar = 5;
                break;
            case (<70):
                rewardStar = 8;
                break;
            case (<90):
                rewardStar = 12;
                break;

        }    


    }

    public void ExitRoom()
    {
        if (GameData.Inst.type == GAMETYPE.DRAW)
        {
            Time.timeScale = 1;
            CommonUI.Inst.gameObject.SetActive(false);
            SceneManager.LoadScene("01. StartScene");
        }
        else
        {
            Time.timeScale = 1;
            LoadSceneManager.LoadSceneAsync("04. DrawScene");
        }
    }


}
