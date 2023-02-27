using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CutBox : MonoBehaviour
{
    public Transform[] vertexs;
    public Camera cam;


    LineRenderer lineRend;
    Vector2 pos1;
    Vector2 pos2;
    Vector2 pos3;
    Vector2 pos4;

    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        changeBoxLine();
    }


    public void initCutBox(float ratio)
    {
        CutBox_Vertex vertex0 = vertexs[0].GetComponent<CutBox_Vertex>();
        CutBox_Vertex vertex1 = vertexs[1].GetComponent<CutBox_Vertex>();

        vertex0.setRatio(ratio);
        vertex1.setRatio(ratio);

        setCutBox();

    }
    /// <summary>
    /// �̵�����, ���� �� ����.
    /// </summary>
    /// <param name="ratio"></param>
    public void setCutBox()
    {
        CutBox_Vertex vertex0 = vertexs[0].GetComponent<CutBox_Vertex>();
        CutBox_Vertex vertex1 = vertexs[1].GetComponent<CutBox_Vertex>();

        vertex0.setLimit(UTILS.getSpritesArea(PhotoSceneManager.Inst.sp));
        vertex1.setLimit(UTILS.getSpritesArea(PhotoSceneManager.Inst.sp));

        vertex0.initpos();
        vertex1.initpos();
        vertex0.initpos();

        GetComponentInChildren<CutBox_Center>().setLimit(UTILS.getSpritesArea(PhotoSceneManager.Inst.sp));

    }

    //Cutbox�� �ڸ��� ������ �����ֱ� ���� LineRenderer �� ����.
    void changeBoxLine()
    {
        Vector2 rectMin = Vector2.Min(vertexs[0].position, vertexs[1].position);
        Vector2 rectMax = Vector2.Max(vertexs[0].position, vertexs[1].position);

        pos1 = rectMin;
        pos2 = Vector2.right * rectMin.x + Vector2.up * rectMax.y;
        pos3 = rectMax;
        pos4 = Vector2.right * rectMax.x + Vector2.up * rectMin.y;

        lineRend.SetPosition(0, pos1);
        lineRend.SetPosition(1, pos2);
        lineRend.SetPosition(2, pos3);
        lineRend.SetPosition(3, pos4);
        lineRend.SetPosition(4, pos1);
    }


    /// <summary>
    /// ������ ������ ������ ĸ���ϴ� �Լ�.
    /// </summary>
    /// <returns></returns>
    public Texture2D Capture()
    {
        //ĸ�Ŀ� ī�޶� ON
        cam.gameObject.SetActive(true);

        //Main Camera�� ���� ������ �ﵵ�� ����
        cam.transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;

        //ĸ�Ŀ� ī�޶� ������ �ȵǴ� ������Ʈ ��Ȱ��ȭ
        lineRend.enabled = false;
        vertexs[0].gameObject.SetActive(false);
        vertexs[1].gameObject.SetActive(false);

        //ĸ�Ŀ� ī�޶� Render
        cam.Render();

        RenderTexture.active = cam.targetTexture;//������� ĸ���� ī�޶� ����


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);

        //ĸ���� �̹����� ������ Texture ����
        Texture2D final = new Texture2D(width, height);

        //texture �� unity���� y��ǥ�� ���̷� ���Ͽ�, y������ ������ ������ ���� �ùٸ��� ����.
        //������ Texture�� ĸ�Ŀ� ī�޶�� ĸ���� �̹����� ������ �κ��� ������.
        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //�����, ����Ƽ �����Ϳ����� ���� ���� �Ʒ��� �����. ���� ��ü ���� ���̤���������ϤӤ�������
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);
        final.Apply();

        //�ʱ�ȭ.
        cam.gameObject.SetActive(false);

        lineRend.enabled = true;
        vertexs[0].gameObject.SetActive(true);
        vertexs[1].gameObject.SetActive(true);



        return final;
    }
}
