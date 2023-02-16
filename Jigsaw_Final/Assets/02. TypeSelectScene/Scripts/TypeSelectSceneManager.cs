using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class TypeSelectSceneManager: MonoBehaviour
{
    private void Start()
    {
        SoundManager.Inst.PlayBGM("BGM_Game");
    }
    public void Btn_Draw()
    {
        SoundManager.Inst.PlaySFX("SFX_ClickBtn");

        GameData.Inst.type = GAMETYPE.DRAW;
        SceneManager.LoadScene("02_1. DrawScene");

    }

    public void Btn_Camera()
    {
        SoundManager.Inst.PlaySFX("SFX_ClickBtn");

        GameData.Inst.type = GAMETYPE.CAMERA;
        Camera_Open();
    }

    public void Btn_Select()
    {
        SoundManager.Inst.PlaySFX("SFX_ClickBtn");
        
        GameData.Inst.type = GAMETYPE.SELECT;
        Gallery_Open();
    }

    /// <summary>
    /// 갤러리 오픈 후, 선택한 사진의 Path를 반환하는 함수.
    /// </summary>
    public void Gallery_Open()  // 갤러리 진입
    {
        try
        {
            NativeGallery.GetImageFromGallery((file) =>
            {
                FileInfo select = new FileInfo(file);

                if (select.Length > 50000000) return;

            // 불러오기
            if (!string.IsNullOrEmpty(file)) LoadImage(file);
            });
        }
        catch
        {
            Debug.Log("선택안합");
        }
    }

    /// <summary>
    /// 카메라 오픈 후, 찍은 사진의 Path를 반환하는 함수.
    /// </summary>
    /// 
    public void Camera_Open()
    {
        try
        {


            NativeCamera.TakePicture((file) =>
            {
                FileInfo select = new FileInfo(file);

                if (select.Length > 50000000) return;

            // 불러오기
            if (!string.IsNullOrEmpty(file)) LoadImage(file);
            });
        }
        catch
        {
            Debug.Log("선택안합");
        }

    }

    /// <summary>
    /// path를 기반으로 texture 로딩.
    /// </summary>
    /// <param name="path"></param>
    void LoadImage(string path)  // 갤러리에서 사진 선별 후 변환
    {
        byte[] fileData = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(fileData);
        //cameraImage.texture = tex;

        GameData.Inst.originTexture = tex;

        LoadSceneManager.LoadSceneAsync("02_23. SelectPhotoScene");

    }
}
