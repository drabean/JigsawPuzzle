using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVUTILS : MonoBehaviour
{
    public static List<string> LoadData(string fileName)
    {
        //������ Path ����
        TextAsset filePath = Resources.Load<TextAsset>(fileName);

        StringReader tr = new StringReader(filePath.text);

        if (tr == null)
        {
            return null;
        }
        List<string> resultList = new List<string>();

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
