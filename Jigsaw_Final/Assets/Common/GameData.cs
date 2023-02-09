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

    //읽어온 원본 텍스쳐
    public Texture2D originTexture;

    public Texture2D puzzleTexture;

    public void Awake()
    {

        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
