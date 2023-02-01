using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TypeSelectSceneManager: MonoBehaviour
{
    public void Btn_Draw()
    {
        GameData.Inst.type = GAMETYPE.DRAW;
        SceneManager.LoadScene("02_1. DrawScene");
    }

    public void Btn_Camera()
    {
        GameData.Inst.type = GAMETYPE.CAMERA;
    }

    public void Btn_Select()
    {
        GameData.Inst.type = GAMETYPE.SELECT;
        Gallery_Open();
    }

    Sprite loadedSp;

    public void Gallery_Open()  // ������ ����
    {
        NativeGallery.GetImageFromGallery((file) =>
        {
            FileInfo select = new FileInfo(file);

            if (select.Length > 50000000) return;

            // �ҷ�����
            if (!string.IsNullOrEmpty(file)) StartCoroutine(LoadImage(file));
        });
    }

    IEnumerator LoadImage(string path)  // ���������� ���� ���� �� ��ȯ
    {
        yield return null;

        byte[] fileData = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(fileData);
        //cameraImage.texture = tex;

        Rect rect = new Rect(0, 0, tex.width, tex.height);
        loadedSp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
        GameData.Inst.sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
        SceneManager.LoadScene("02_23. SelectPhotoScene");

    }
}
