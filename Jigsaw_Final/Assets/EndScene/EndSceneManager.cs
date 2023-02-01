using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;

    private void Start()
    {
        Time.timeScale = 0;
        scoreTMP.text = GameData.Inst.score.ToString("N2");
    }

    public void ExitRoom()
    {
        if (GameData.Inst.type == GAMETYPE.DRAW)
        {
            Time.timeScale = 1;
            Destroy(CommonUI.Inst.gameObject);
            SceneManager.LoadScene("01. StartScene");
        }
        else
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("04. DrawScene");
        }
    }
}
