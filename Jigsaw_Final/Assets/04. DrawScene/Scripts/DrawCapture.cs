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
    public Texture2D Capture(SpriteRenderer sp)
    {
        setDrawCapture(sp);

        textObject.SetActive(false);

        cam.gameObject.SetActive(true);

        cam.transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;

        cam.Render();

        RenderTexture.active = cam.targetTexture;//여기까지 캡쳐할 카메라 세팅


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);


        Texture2D final = new Texture2D(width, height);
        //texture 와 unity상의 y좌표의 차이로 인하여, y에대해 반전된 영역을 찍어야 올바르게 찍힘.

        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //모바일, 유니티 에디터에서는 위에 말고 아래꺼 써야함. 왜지 대체 왜지 먼이ㅏㄴㅁ라밀하ㅣㅁㅇㅎㅁ
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);
        final.Apply();

        cam.gameObject.SetActive(false);
        textObject.SetActive(true);

        return final;
    }

}
