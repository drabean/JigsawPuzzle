using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CommonUI : MonoBehaviour
{
    public static CommonUI Inst;

    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public string startSceneName = "01. StartScene";
    public string MainPackageName = "com.DefaultCompany.MinigameTownTest";


    public GameObject UIPanel;
    public static bool isSFXOn => isSFXOn;
    public static bool isBGMOn => isBGMOn;


    bool SFXOn;
    bool BGMOn;

    [SerializeField] Sprite[] TogleImages;
    [SerializeField] Image Image_BGM;
    [SerializeField] Image Image_SFX;
    public void Btn_Setting()
    {
        UIPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Btn_Restart()
    {
        SceneManager.LoadScene(startSceneName);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void Btn_Exit()
    {
#if UNITY_ANDROID
        if(!IsAppInstalled(MainPackageName))
        {
            Debug.Log("앱이 깔려있지 않습니다");
            return;
        }
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject pm = jo.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", MainPackageName);

        jo.Call("startActivity", intent);


        Application.Quit();
#else
        Debug.Log("NOT SUPPORTED IN EDITOR");
#endif
    }
    bool IsAppInstalled(string bundleID)
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;

        //if the app is installed, no errors. Else, doesn't get past next line
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);
        }
        catch (System.Exception ex)
        {
            Debug.Log("exception" + ex.Message);
            //여기에서 앱이 설치 되지 않았을때의 예외처리.
        }

        return (launchIntent == null ? false : true);
#else

        Debug.LogError("NOT SUPPORTED IN EDITOR");
        return false;
#endif

    }
    public void Btn_Help()
    {

    }
    public void Btn_BGM()
    {
        if (BGMOn)
        {
            BGMOn = false;
            Image_BGM.sprite = TogleImages[0];
        }
        else
        {
            BGMOn = true;
            Image_BGM.sprite = TogleImages[1];
        }
    }

    public void Btn_SFX()
    {
        if (SFXOn)
        {
            SFXOn = false;
            Image_SFX.sprite = TogleImages[0];
        }
        else
        {
            SFXOn = true;
            Image_SFX.sprite = TogleImages[1];
        }
    }

    public void Btn_Close()
    {
        UIPanel.SetActive(false);
        Time.timeScale = 1f;
    }

}
