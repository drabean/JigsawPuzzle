using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCapture : MonoBehaviour
{
    public Transform[] vertexs;
    public Camera cam;


    public void setDrawCapture(SpriteRenderer sp)
    {
        vertexs [0].position = UTILS.getSpritesArea(sp)[0];
        vertexs [1].position = UTILS.getSpritesArea(sp)[1];
    }

    public GameObject textObject;

    /// <summary>
    /// spriteRender 骨是亜 鉢檎拭 左戚澗 幻滴a
    /// </summary>
    /// <param name="sp"></param>
    /// <returns></returns>
    public Texture2D Capture(SpriteRenderer sp)
    {
        setDrawCapture(sp);

        textObject.SetActive(false);

        cam.gameObject.SetActive(true);

        cam.transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;

        cam.Render();

        RenderTexture.active = cam.targetTexture;//食奄猿走 銚団拝 朝五虞 室特


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);


        Texture2D final = new Texture2D(width, height);
        //texture 人 unity雌税 y疎妊税 託戚稽 昔馬食, y拭企背 鋼穿吉 慎蝕聖 啄嬢醤 臣郊牽惟 啄毘.

        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //乞郊析, 政艦銅 拭巨斗拭辞澗 是拭 源壱 焼掘襖 潤醤敗. 訊走 企端 訊走 胡戚たいけ虞腔馬びけしぞけ
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);

        final.Apply();

        cam.gameObject.SetActive(false);
        textObject.SetActive(true);

        return final;
    }

}
