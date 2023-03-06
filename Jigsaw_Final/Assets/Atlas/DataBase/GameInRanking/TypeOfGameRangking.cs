using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class UserInfo : IComparable
{
    public static int sortingIdx;

    public string id;
    public string password;
    public string nickname;
    public int year;
    public int selectNum;

    public string[] score;
    public int gamemoney;

    public string[] objname;
    public string[] posX;
    public string[] posY;
    public string[] posZ;
    public string[] rotY;
    public string[] grounds_Save;

    public string[] objectcount;
    public string[] shopmaxcount;

    public string[] farmobjname;
    public string[] farminvenobjname;
    public string[] farmobjposX;
    public string[] farmobjposY;

    public int CompareTo(object obj)
    {
        UserInfo data = (UserInfo)obj;

        return float.Parse(this.score[sortingIdx]).CompareTo(float.Parse(data.score[sortingIdx]));
    }

}

public class TypeOfGameRangking : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://CheonJiYun:wldbs3456@cluster0.yyfatob.mongodb.net/?retryWrites=true&w=majority");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    public List<UserInfo> totalScoreData = new List<UserInfo>();

    public bool isProcessing = false;

    public UserInfo loginUser = null;
    public static TypeOfGameRangking Inst;

    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
        else
        {
            Destroy(this);
        }

        database = client.GetDatabase("UserData");
        collection = database.GetCollection<BsonDocument>("UserInfo");
    }

    //전체 유저 리스트를 가지고 오는 함수 호출
    public void CallRangking()
    {
        isProcessing = true;

        GetTotalData_FromDatabase();
    }

    //데이터베이스에서 전체 유저의 리스트를 가지고오는 함수
    public async void GetTotalData_FromDatabase()
    {
        totalScoreData.Clear();

        var allDatasTask = collection.FindAsync(new BsonDocument());
        var datasAwated = await allDatasTask;

        object arrayString;


        foreach (var data in datasAwated.ToList())
        {
            UserInfo user = new UserInfo();

            user.id = (string)data.GetValue("_id");
            user.nickname = (string)data.GetValue("nickname");
            user.selectNum = (int)data.GetValue("selectNum");

            arrayString = (object)data.GetValue("score");
            user.score = ArraySplitSort(arrayString);


            totalScoreData.Add(user);
        }

        isProcessing = false;
    }

    //데이터베이스에 해당 유저를 찾고 해당유저의 정보를 수정
    public async void Save_FromDatabase(string _id,float curScore,int star ,int curGameNum, int curDifNum )
    {
        await GetUserData_FromDatabase(_id);

        isProcessing = true;

        for (int i = 0; i < loginUser.score.Length; i++)
        {
            loginUser.score[i]=loginUser.score[i].Replace(" ",string.Empty);
        }

        loginUser.score[curGameNum * 4 + curDifNum] = curScore.ToString();
        loginUser.gamemoney = loginUser.gamemoney + star;

        BsonDocument find = new BsonDocument { { "_id", _id } };

        collection.ReplaceOne(find, loginUser.ToBsonDocument());

        isProcessing = false;
    }

    //로그인 유저 데이터 가지고 오기
    public async Task<UserInfo> GetUserData_FromDatabase(string _id)
    {
        isProcessing = true;

        BsonDocument find = new BsonDocument { { "_id", _id } };

        var allDatasTask = collection.FindAsync(find);
        var datasAwated = await allDatasTask;

        object arrayString;

        UserInfo user = new UserInfo();

        foreach (var data in datasAwated.ToList())
        {
            user.id = (string)data.GetValue("_id");
            user.password = (string)data.GetValue("password");
            user.gamemoney = (int)data.GetValue("gamemoney");
            user.year = (int)data.GetValue("year");
            user.nickname = (string)data.GetValue("nickname");

            arrayString = (object)data.GetValue("score");
            user.score = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("objname");
            //스트링으로 변환된 배열들의 값을 배열에 넣음
            user.objname = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("posX");
            user.posX = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("posY");
            user.posY = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("posZ");
            user.posZ = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("rotY");
            user.rotY = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("grounds_Save");
            user.grounds_Save = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("objectcount");
            user.objectcount = ArraySplitSort(arrayString);

            arrayString = (object)data.GetValue("shopmaxcount");
            user.shopmaxcount = ArraySplitSort(arrayString);
        }

        loginUser = user;

        isProcessing = false;


        return user;
    }


    private string[] ArraySplitSort(object str)
    {
        string[] words = str.ToString().Split(",");
        char[] charArray = str.ToString().ToCharArray();

        bool indexCheck = false;
        if (words[0] == "BsonNull") return null;

        for (int i = 0; i < charArray.Length; i++)
        {
            if (charArray[i] == ',')
            {
                indexCheck = true;
                break;
            }
        }

        if (indexCheck)
        {

            words[0] = words[0].Split("[")[1];
            words[words.Length - 1] = words[words.Length - 1].Split("]")[0];
        }
        else
        {
            string word = words[0].Split("[")[1];
            words = new string[1];
            words = word.Split("]");

            word = "";
            for (int i = 0; i < words.Length; i++)
            {
                word += words[i];
            }
            words = new string[1];
            words[0] = word;
        }

        return words;
    }


}
