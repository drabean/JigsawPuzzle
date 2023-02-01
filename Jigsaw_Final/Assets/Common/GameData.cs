using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Inst;
    public DIFFICULTY difficulty;
    public GAMETYPE type;
    public float score;
    //public PlayerInfo info; 

    public Sprite sp;
    public Texture2D puzzleTexture;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Inst == null) Inst = this;
        else Destroy(gameObject);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
