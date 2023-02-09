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
        Camera_Open();
    }

    public void Btn_Select()
    {
        GameData.Inst.type = GAMETYPE.SELECT;
        Gallery_Open();
    }

    public void Gallery_Open()  // ������ ����
    {
        try
        {
            NativeGallery.GetImageFromGallery((file) =>
            {
                FileInfo select = new FileInfo(file);

                if (select.Length > 50000000) return;

            // �ҷ�����
            if (!string.IsNullOrEmpty(file)) LoadImage(file);
            });
        }
        catch
        {
            Debug.Log("���þ���");
        }
    }
    public void Camera_Open()
    {
        try
        {


            NativeCamera.TakePicture((file) =>
            {
                FileInfo select = new FileInfo(file);

                if (select.Length > 50000000) return;

            // �ҷ�����
            if (!string.IsNullOrEmpty(file)) LoadImage(file);
            });
        }
        catch
        {
            Debug.Log("���þ���");
        }

    }

    void LoadImage(string path)  // ���������� ���� ���� �� ��ȯ
    {
        byte[] fileData = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(fileData);
        //cameraImage.texture = tex;

        GameData.Inst.originTexture = tex;

        LoadSceneManager.LoadSceneAsync("02_23. SelectPhotoScene");

    }
}
