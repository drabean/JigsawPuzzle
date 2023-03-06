using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TopUser : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI firstUserNickName = null;
    [SerializeField] private TextMeshProUGUI secondUserNickName = null;
    [SerializeField] private TextMeshProUGUI thirdUserNickName = null;

    [SerializeField] private Image firstUserImage = null;
    [SerializeField] private Image secondtUserImage = null;
    [SerializeField] private Image thirdUserImage = null;

    [SerializeField] private Sprite[] charSprite = null;

    public void SetNickName(string first, string secound, string third)
    {
        firstUserNickName.text = first;
        secondUserNickName.text = secound;
        thirdUserNickName.text = third;
    }

    public void SetImage(int charNum_1, int charNum_2, int charNum_3)
    {
        if (charNum_1 == -1)
            firstUserImage.gameObject.SetActive(false);
        else
        {
            firstUserImage.gameObject.SetActive(true);
            firstUserImage.sprite = charSprite[charNum_1];
        }

        if (charNum_2 == -1)
            secondtUserImage.gameObject.SetActive(false);
        else
        {
            secondtUserImage.gameObject.SetActive(true);
            secondtUserImage.sprite = charSprite[charNum_2];
        }

        if (charNum_3 == -1)
            thirdUserImage.gameObject.SetActive(false);
        else
        {
            thirdUserImage.gameObject.SetActive(true);
            thirdUserImage.sprite = charSprite[charNum_3];
        }

    }

    public void SetNone()
    {
        firstUserImage.sprite = null;
        secondtUserImage.sprite = null;
        thirdUserImage.sprite = null;
    }

}
