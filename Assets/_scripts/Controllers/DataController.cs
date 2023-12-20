using BestHTTP.ServerSentEvents;
using Proyecto26;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    private InitUserData _initUserData;
    public ArticleTask[] _tasksArr;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Auth");
    }

    public void SetUserData(InitUserData data)
    {
        _initUserData = data;
    }
    public int GetUserLevel()
    {
      return  _initUserData.crntLvl;
    }

  

    public TasksData CurrentTasksData;
    public void GetLevelTasks(int level, int maxTasksInRoom, Action onLoaded)
    {
        Dictionary<string, object> @params = new Dictionary<string, object>();
        @params.Add("level", level);
        @params.Add("maxTasksInRoom", maxTasksInRoom);

        FirebaseManager.Instance.Functions.CallCloudFunction("GetArticlesLevelData", @params, (data) =>
        {
            CurrentTasksData = DataParser.ParseTasksData(data.body);
            onLoaded.Invoke();
        }, (exception) =>
        {
            Debug.LogError($"Error while calling GetArticlesLevelData cloud function. Message - {exception.Message}");
        });
    }


    internal string GetJwtToken()
    {
        return _initUserData.accessToken;
    }
    internal string GetName()
    {
        return _initUserData.name;
    }
    internal void SetName(string newName)
    {
      _initUserData.name=newName;
    }

    internal string GetSurname()
    {
        return _initUserData.surname;
    }
    internal void SetSurname(string newSurame)
    {
        _initUserData.surname=newSurame;
    }

    internal void IncreaseLevel()
    {
        _initUserData.crntLvl++;
    }
}
