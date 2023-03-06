//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UiController : MonoBehaviour
//{
   
//    [SerializeField] private Transform canvars = null;

//    [SerializeField] private RankingUiData rankingPrefab = null;

//    [HideInInspector] public GAMETYPE curType;

//    private RankingUiData rankingBoard = null;
//    public RankingUiData GetRankingUiData { get { return rankingBoard; } private set { } }

//    #region OnClick_Button Event

//    //Click FindPickture Button
//    public void OnClick_GoRankingBoard()
//    {
//        curType = GAMETYPE.MemoryCard;

//        rankingBoard = Instantiate<RankingUiData>(rankingPrefab, canvars.transform);

//        rankingBoard.curGameName.text = "Memory Card";

//        rankingBoard.SetType(curType);

//        rankingBoard.SetActiveBoardCheck();
//    }

//    #endregion
//}
