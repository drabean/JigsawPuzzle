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
    /// 戚疑骨是, 搾晴 去 竺舛.
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

    //Cutbox稽 切牽澗 骨是研 左食爽奄 是廃 LineRenderer 葵 痕井.
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
    /// 識澱廃 骨是研 叔薦稽 銚団馬澗 敗呪.
    /// </summary>
    /// <returns></returns>
    public Texture2D Capture()
    {
        //銚団遂 朝五虞 ON
        cam.gameObject.SetActive(true);

        //Main Camera人 旭精 骨是研 啄亀系 幻給
        cam.transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;

        //銚団遂 朝五虞拭 啄備檎 照鞠澗 神崎詮闘 搾醗失鉢
        lineRend.enabled = false;
        vertexs[0].gameObject.SetActive(false);
        vertexs[1].gameObject.SetActive(false);

        //銚団遂 朝五虞 Render
        cam.Render();

        RenderTexture.active = cam.targetTexture;//食奄猿走 銚団拝 朝五虞 室特


        Vector2 maxPos = Vector2.Max(vertexs[0].position, vertexs[1].position);
        Vector2 minPos = Vector2.Min(vertexs[0].position, vertexs[1].position);

        minPos = cam.WorldToScreenPoint(minPos);
        maxPos = cam.WorldToScreenPoint(maxPos);

        int width = (int)(maxPos.x - minPos.x);
        int height = (int)(maxPos.y - minPos.y);

        //銚団廃 戚耕走研 煽舌拝 Texture 持失
        Texture2D final = new Texture2D(width, height);

        //texture 人 unity雌税 y疎妊税 託戚稽 昔馬食, y拭企背 鋼穿吉 慎蝕聖 啄嬢醤 臣郊牽惟 啄毘.
        //持失廃 Texture拭 銚団遂 朝五虞稽 銚団廃 戚耕走税 走舛吉 採歳聖 差紫敗.
        final.ReadPixels(new Rect(minPos.x, minPos.y, width, height), 0, 0);
        //乞郊析, 政艦銅 拭巨斗拭辞澗 是拭 源壱 焼掘襖 潤醤敗. 訊走 企端 訊走 胡戚たいけ虞腔馬びけしぞけ
        //final.ReadPixels(new Rect(minPos.x, cam.pixelHeight - maxPos.y, width, height), 0, 0);
        final.Apply();

        //段奄鉢.
        cam.gameObject.SetActive(false);

        lineRend.enabled = true;
        vertexs[0].gameObject.SetActive(true);
        vertexs[1].gameObject.SetActive(true);



        return final;
    }
}
