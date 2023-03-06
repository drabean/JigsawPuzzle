using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI beforeScore = null;
    [SerializeField] private TextMeshProUGUI curScore = null;
    [SerializeField] private TextMeshProUGUI getStarCount = null;

    public GameObject UprangkText = null;
 

    public void SetBeforeScoreText(float score)
    {
        beforeScore.text=score.ToString("F1");
    }

    public void SetCurScore(float score)
    {
        curScore.text = score.ToString();
    }
  
    public void SetStartCount(int count)
    {
        getStarCount.text = $"x {count}";
    }

    public void Actvie_UpRank(bool activeSelf = true)
    {
        UprangkText.SetActive(activeSelf);

    }

}
