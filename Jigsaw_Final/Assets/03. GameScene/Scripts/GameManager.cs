using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    public Generator_Board boardGenerator;

    public Timer timer;

    private void Start()
    {
        Inst = this;

        boardGenerator.startGeneratingPuzzle();

        setProperty();

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

    [ContextMenu("ClearTest")]
    public void StartDraw()
    {
        Debug.Log("CLEAR!");

        timer.stop();

        //소수점 1자리까지만
        GameData.Inst.score = (int)(timer.curTime * 100) / 100f;

        SceneManager.LoadScene("Additive_EndScene", LoadSceneMode.Additive);

    }
}
