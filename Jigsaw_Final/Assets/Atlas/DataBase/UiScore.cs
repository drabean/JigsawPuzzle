using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingText = null;
    [SerializeField] private TextMeshProUGUI nickNameText = null;
    [SerializeField] private TextMeshProUGUI socreText = null;
    [SerializeField] private Image Image = null;

    [SerializeField] private Sprite[] characters = null;
    [SerializeField] private Sprite[] gameSprite = null;
    [SerializeField] private Sprite[] rankingbyType = null;

    [SerializeField] private Image rangkingBackground = null;
    [SerializeField] private Image gameImagae = null;
    [SerializeField] private Image userPorfile = null;


    public int charNum = 0;


    public string GetRankingText { get { return rankingText.text; } private set { } }
    public string GetNickNameText { get { return nickNameText.text; } private set { } } 
    public string GetSocreText { get { return socreText.text; } private set { } }


    public void init(int ranking,string nickname,string score,int charater,int gameNum)
    {
        socreText.text = score.ToString();

        if(ranking!=0)
            rankingText.text = ranking.ToString();
        else
        {
            rankingText.text = "¾øÀ½";
            rangkingBackground.sprite = rankingbyType[3];
        }

        nickNameText.text = nickname;

        userPorfile.sprite = characters[charater];

        if (ranking == 1)
            rangkingBackground.sprite = rankingbyType[0];
        else if(ranking ==2)
            rangkingBackground.sprite = rankingbyType[1];
        else if (ranking == 3)
            rangkingBackground.sprite = rankingbyType[2];


        gameImagae.sprite = gameSprite[gameNum];

    }





    public void ChangeColor(Color color)
    {
        Image.color = color;
    }

   


}
