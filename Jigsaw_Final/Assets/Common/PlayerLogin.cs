using UnityEngine;
using System.IO;
using System.Text;

public class PlayerLogin
{
    /// <summary>
    /// 파일을 저장할 path를 불러옵니다.
    /// </summary>
    /// <returns></returns>
    static string getFilePath()
    {
        string path = Application.persistentDataPath;

        string[] tok = path.Split("/");

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tok.Length - 2; i++)
        {
            sb.Append(tok[i]);
            sb.Append("/");
        }
        sb.Append("PlayerData.csv");

        Debug.Log(sb.ToString());

        return sb.ToString();
    }
    /// <summary>
    /// persistentDataPath의 PlayerData로부터 Player의 Name을 가져옵니다. 이미 생성되어 있는 파일이 없었다면, null을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public static string getPlayerData()
    {
        string path = getFilePath();

        Debug.Log(path);
        TextReader tr;

        try
        {
            tr = new StreamReader(path);

            //첫번째는 colomn 넣을 예정이라 2번째거를 가져옴
            string[] tok = tr.ReadLine().Split(",");

            tr.Close();

            return tok[1];
        }
        catch
        {
            Debug.Log("noData");
            return "testPlayer";
        }


    }
    /// <summary>
    /// PlayerData.csv 파일에 플레이어 이름을 저장합니다.
    /// </summary>
    /// <param name="name"></param>
    public static void setPlayedData(string name)
    {
        string path = getFilePath();

        TextWriter tw = new StreamWriter(path);

        tw.WriteLine("PlayerName" + "," + name);
        Debug.Log("Save Complete");

        tw.Close();
    }

    /// <summary>
    /// 현재 저장되어있는 PlayerData.csv를 삭제합니다.
    /// </summary>
    public static void deletePlayerData()
    {
        string path = getFilePath();

        File.Delete(path);

    }
}