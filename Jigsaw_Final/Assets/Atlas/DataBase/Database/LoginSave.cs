using System;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class LoginPlayerData
{
    public string id;
}

public class LoginSave
{
    static public void Set(string id)
    {

#if UNITY_EDITOR

        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Saves/";
        string fileName = "/Save01.json";
#elif UNITY_ANDROID
          string path = "/storage/emulated/0/Save";
         string fileName = "/Save.json";   
#endif
        if (Directory.Exists(path) == false) // There's no directory
        {
            Directory.CreateDirectory(path);
        }

        LoginPlayerData data = new LoginPlayerData();

        data.id = id;

        File.WriteAllText(path + fileName, JsonUtility.ToJson(data));
    }



    static public LoginPlayerData Get()
    {
#if UNITY_EDITOR
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Saves/";
        string fileName = "/Save01.json";
#elif UNITY_ANDROID
         string path = "/storage/emulated/0/Save";
         string fileName = "/Save.json"; 
#endif

        LoginPlayerData data = JsonUtility.FromJson<LoginPlayerData>(File.ReadAllText(path + fileName));


        if (data.id == null)
            data.id = path + fileName;

        return data;
    }


    static public void PlayerSet(string data)
    {
        PlayerPrefs.SetString("ID", data);
    }

    static public string PlayerGet()
    {
        string ID = PlayerPrefs.GetString("ID");

        return ID;
    }



    static public void WriteStringToFile(string str, string filename)
    {
#if !WEB_BUILD
        string path = PathForDocumentsFile(filename);
        FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

        StreamWriter sw = new StreamWriter(file);
        sw.WriteLine(str);

        sw.Close();
        file.Close();
#endif
    }


    static public string ReadStringFromFile(string filename)//, int lineIndex )
    {
#if !WEB_BUILD
        string path = PathForDocumentsFile(filename);

        if (File.Exists(path))
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string str = null;
            str = sr.ReadLine();

            sr.Close();
            file.Close();

            return str;
        }
        else
        {
            return path;
        }

#else
            return null;
#endif

    }

    private static string PathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }



}