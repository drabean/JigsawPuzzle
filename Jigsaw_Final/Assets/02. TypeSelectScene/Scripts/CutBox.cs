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
    /// 이동범위, 비율 등 설정.
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

    //Cutbox로 자르는 범위를 보여주기 위한 LineRenderer 값 변경.
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
    /// 선택한 범위를 실제로 캡쳐하는 함수.
    /// </summary>
    /// <returns></returns>
    public Texture2D Capture()
    {
        //캡쳐용 카메라 ON
        cam.gameObject.SetActive(true);

        //Main Camera와 같은 범위를 찍도록 만듬
        cam.transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;

        //캡쳐용 카메라에 찍히면 안되는 오브젝트 비활성화
        lineRend.enabled = false;
        vertexs[0].gameObject.SetActive(false);
        vertexs[1].gameObject.SetActive(false);

        //캡쳐용 카메라 Render
        cam.Render();

        RenderTexture.active = cam.targetTexture;//여기까지 캡쳐할 카메라 세팅


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);

        //캡쳐한 이미지를 저장할 Texture 생성
        Texture2D final = new Texture2D(width, height);

        //texture 와 unity상의 y좌표의 차이로 인하여, y에대해 반전된 영역을 찍어야 올바르게 찍힘.
        //생성한 Texture에 캡쳐용 카메라로 캡쳐한 이미지의 지정된 부분을 복사함.
        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //모바일, 유니티 에디터에서는 위에 말고 아래꺼 써야함. 왜지 대체 왜지 먼이ㅏㄴㅁ라밀하ㅣㅁㅇㅎㅁ
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);
        final.Apply();

        //초기화.
        cam.gameObject.SetActive(false);

        lineRend.enabled = true;
        vertexs[0].gameObject.SetActive(true);
        vertexs[1].gameObject.SetActive(true);



        return final;
    }
}
