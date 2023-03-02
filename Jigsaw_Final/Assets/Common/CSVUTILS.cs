using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVUTILS : MonoBehaviour
{
    public static List<string> LoadData(string fileName)
    {
        //������ Path ����
        string filePath = Application.dataPath + "\\" + fileName;
        List<string> resultList = new List<string>();

        TextReader tr = new StreamReader(filePath);
        if (tr == null)
        {
            return null;
        }


        string line = tr.ReadLine();//ù���� Properties�̹Ƿ� ���� ó������

        while (line != null)
        {
            line = tr.ReadLine();
            if (line == null) break;
            resultList.Add(line);
        }

        tr.Close();

        return resultList;
    }
}
