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
            CommonUI.Inst.gameObject.SetActive(false);
            SceneManager.LoadScene("01. StartScene");
        }
        else
        {
            Time.timeScale = 1;
            StartCoroutine(CO_LoadScene("04. DrawScene"));
        }
    }

    IEnumerator CO_LoadScene(string SceneName)
    {
        yield return null;
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneName);

        CommonUI.Inst.startLoading();
    }

}
