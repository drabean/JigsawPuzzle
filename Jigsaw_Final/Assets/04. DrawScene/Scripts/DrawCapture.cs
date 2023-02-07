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

        RenderTexture.active = cam.targetTexture;//������� ĸ���� ī�޶� ����


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);


        Texture2D final = new Texture2D(width, height);
        //texture �� unity���� y��ǥ�� ���̷� ���Ͽ�, y������ ������ ������ ���� �ùٸ��� ����.

        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //�����, ����Ƽ �����Ϳ����� ���� ���� �Ʒ��� �����. ���� ��ü ���� ���̤���������ϤӤ�������
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);
        final.Apply();

        cam.gameObject.SetActive(false);
        textObject.SetActive(true);

        return final;
    }

}
