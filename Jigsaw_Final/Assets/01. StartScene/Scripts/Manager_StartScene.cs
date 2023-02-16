using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_StartScene : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Inst.PlayBGM("BGM_SelectScene");
    }
    public void Click_Easy()
    {
        Debug.Log("Easy");
        GameData.Inst.difficulty = DIFFICULTY.EASY;
        SceneManager.LoadScene("02. TypeSelectScene");

        SoundManager.Inst.PlaySFX("SFX_ChangeScene");
    }

    public void Click_Normal()
    {
        Debug.Log("Normal");
        GameData.Inst.difficulty = DIFFICULTY.NORMAL;
        SceneManager.LoadScene("02. TypeSelectScene");

        SoundManager.Inst.PlaySFX("SFX_ChangeScene");
    }

    public void Click_Hard()
    {

        Debug.Log("Hard");
        GameData.Inst.difficulty = DIFFICULTY.HARD;
        SceneManager.LoadScene("02. TypeSelectScene");

        SoundManager.Inst.PlaySFX("SFX_ChangeScene");
    }

    public void Click_Master()
    {
        Debug.Log("Master");
        GameData.Inst.difficulty = DIFFICULTY.MASTER;
        SceneManager.LoadScene("02. TypeSelectScene");

        SoundManager.Inst.PlaySFX("SFX_ChangeScene");
    }
}
