using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;


public class RestManager<T1, T2>
{
    public string _root = "http://localhost:3000/";

    public string _token = "";

    public RequestHelper currentRequest;

    public void LogMessage(string title, string message)
    {
#if UNITY_EDITOR
        EditorUtility.DisplayDialog(title, message, "Ok");
#else
		Debug.Log(message);
#endif
    }

    /*public void Post(T2 enterDataModel, string rootAddition, out T1 outData)
    {
        currentRequest = new RequestHelper
        {
            Uri = _root + rootAddition,
            Params = new Dictionary<string, string> { },
            Body = enterDataModel,
            EnableDebug = true
        };

        RestClient.Post<T1>(currentRequest)
        .Then(res =>
        {
            outData = res;
        })
        .Catch(err => { outData = default; Debug.Log(err); }) ;
    }*/

}


[System.Serializable]
public class UserRegistrationData
{
    public string username;
    public string password;
    public string name;
    public string surname;
    public UserRegistrationData(string name, string surname, string username, string password)
    {
        this.name = name;
        this.surname = surname;
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class UserCreditionals
{
    public string username;
    public string password;


    public UserCreditionals(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class InitUserData
{
    public string accessToken;
    public string name;
    public string surname;
    public int crntLvl;
}

[System.Serializable]
public class UserPersonalData
{
    public string name;
    public string surname;

    public UserPersonalData(string name, string surname)
    {
        this.name=name;
        this.surname=surname;
    }
}

[System.Serializable]
public class UserProgress
{
    public int crntLvl;
    public string timeStamp;
    public int points;
    public UserProgress(int points, int crntLvl, string timeStamp)
    {
        this.points = points;
        this.crntLvl = crntLvl;
        this.timeStamp = timeStamp;
    }
}


[System.Serializable]
public class LeaderBoardUser
{
    public string name;
    public string surname;
    public string points;
    public int place;



    public LeaderBoardUser(string name, string surname, string  points, int place)
    {
        this.name = name;
        this.surname = surname;
        this.points = points;
        this.place = place;
    }
}

[System.Serializable]
public class ArticleTask
{
    public string difficulty;
    public string firstPhrase;
    public string[] phrases;
    public string articlesCount;
    public string[] articles;


    public ArticleTask(string difficulty, string firstPhrase, string[] phrases, string articlesCount, string[] articles)
    {
        this.difficulty = difficulty;
        this.firstPhrase = firstPhrase;
        this.phrases = phrases;
        this.articlesCount = articlesCount;
        this.articles = articles;
    }
}