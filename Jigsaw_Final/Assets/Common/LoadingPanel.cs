using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoadingPanel : MonoBehaviour
{
    //0Àº µÞ¹è°æ, 
    public Image touchImage;
    public Image Background;
    public GameObject Panel;

    public Sprite[] textSprite;
    public Sprite[] interactSprite;

    int interactSpriteIdx;

    private void Start()
    {
        interactSpriteIdx = Random.Range(0, interactSprite.Length);

    }

    public void FadeIn()
    {
        Panel.SetActive(true);
    }

    public void FadeOut()
    {
        Panel.SetActive(false);
    }

    public void onClick()
    {
        interactSpriteIdx += Random.Range(0, interactSprite.Length - 1);
        interactSpriteIdx %= interactSprite.Length;

        touchImage.sprite = interactSprite[interactSpriteIdx];
    }
}
