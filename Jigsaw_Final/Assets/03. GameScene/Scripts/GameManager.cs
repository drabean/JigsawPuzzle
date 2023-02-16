using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    public Generator_Board boardGenerator;

    public Timer timer;

    public Animator readyAnim;


    private void Awake()
    {
        Inst = this;

        setProperty();

        boardGenerator.startGeneratingPuzzle();

        StartCoroutine(CO_PuzzleMove());
    }

    IEnumerator CO_PuzzleMove()
    {
        yield return new WaitForSeconds(0.3f);
        readyAnim.SetTrigger("Start");
        yield return new WaitForSeconds(3.0f);
        boardGenerator.initSpritesPositions();
        timer.start();

    }
    public void setProperty()
    {
        if (GameData.Inst == null) return;

        switch (GameData.Inst.difficulty)
        {
            case DIFFICULTY.EASY:
                Camera.main.orthographicSize = 350;
                Camera.main.transform.position = new Vector3(200, 200, -10);
                break;
            case DIFFICULTY.NORMAL:
                Camera.main.orthographicSize = 450;
                Camera.main.transform.position = new Vector3(300, 300, -10);
                break;
            case DIFFICULTY.HARD:
                Camera.main.orthographicSize = 550;
                Camera.main.transform.position = new Vector3(400, 400, -10);
                break;
            case DIFFICULTY.MASTER:
                Camera.main.orthographicSize = 650;
                Camera.main.transform.position = new Vector3(500, 500, -10);
                break;
        }

    }
    
    public void GameOver()
    {
        Debug.Log("CLEAR");
        SoundManager.Inst.PlaySFX("SFX_Clear");
        timer.stop();

        //시간을 소수점 1자리까지만 끊어서 점수로 저장
        GameData.Inst.score = (int)(timer.curTime * 100) / 100f;


        SceneManager.LoadScene("Additive_EndScene", LoadSceneMode.Additive);
    }
}
