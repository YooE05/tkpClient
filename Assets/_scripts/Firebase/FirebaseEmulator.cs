using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FirebaseEmulator : MonoBehaviour
{
    public string NextSceneToLoad = "DataLoadingScreen";
    public GameObject AuthView;

    private int registerdUsersCount = 0;
    private int usersCount = 2;

   

    public void SignAsUser(int id)
    {
        if(id == 1)
            FirebaseManager.Instance.Auth.SetLocalId("fyx1hktpSHRmwkPLEylnAnIXuOK2");
        else
            FirebaseManager.Instance.Auth.SetLocalId("DS8I8LYxqoP5XumyhHcvyw7YTfV2");

        GoToNextScene();
    }

    private void Awake()
    {
        if (FirebaseProjectConfigurations.PROJECT_BUILD != ProjectBuildType.Emulator)
            Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        Debug.Log("Start of emulator");
        if (!AuthView.activeInHierarchy)
            AuthView.SetActive(true);

        PlayerPrefs.DeleteAll();

        while (!(FirebaseManager.Instance.AllManagersInitialized || FirebaseProjectConfigurations.Initialized))
            yield return null;

        //edit. Add registration and auth in updated project
        CreateNewUser("fyx1hktpSHRmwkPLEylnAnIXuOK2", "User 1", "Surname", 2, () => {
            registerdUsersCount++; 
        }, (e) => { Debug.LogError($"Failed create user. Message: {e.Message}"); });

        CreateNewUser("DS8I8LYxqoP5XumyhHcvyw7YTfV2", "User 2", "Surname", 2, () => { registerdUsersCount++; }, (e) => { Debug.LogError($"Failed create user. Message: {e.Message}"); });

        CreateTestUsers();

        while (registerdUsersCount < usersCount)
            yield return null;

        AuthView.transform.FindDeepChild("Log").GetComponent<Text>().text += "\r\nUsers created";
    }
    private void CreateNewUser(string userId, string name, string surname, int @class, Action onCreated, Action<Exception> onFailed)
    {
        //Dictionary<string, string> @params = new Dictionary<string, string>()
        //{
        //    {"userId", userId },
        //    {"name", name },
        //    {"surname", surname },
        //    {"userClass", "class" + @class }
        //};

        PlatformUserData platformUserData = new PlatformUserData(userId, name, surname, $"class{@class}", null);
        FirebaseManager.Instance.Functions.CallCloudFunctionPostObject("CreateNewUserInPlatform", platformUserData, null, (statusCode) => onCreated(), onFailed);
    }

    //edit. update
    private void CreateTestUsers()
    {
        CreateNewUser("TestUser3", "User 3", "Surname", 2, () => { }, (e) => { });
        PlatformUserData platformUserData = new PlatformUserData("TestUser3", "User 3", "Surname", "class2", null);
        Dictionary<string, string> args = new Dictionary<string, string>() { { "game", "timeofhistory" } };
        FirebaseManager.Instance.Functions.CallCloudFunctionPostObject<PlatformUserData>("CreateNewUserGameData", platformUserData, args, (statusCode) =>
        {
            Debug.Log("User successfully created in game");
        }, (exception) =>
        {
            Debug.LogError("Error while calling CreateNewUserGameData cloud function. Message - " + exception.Message);
        });
    }
    private void GoToNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneToLoad);
    }
}

public class DZGamesPlatformUser
{
    public string userId;
    public string name;
    public string surname;
    public int userClass;

    public DZGamesPlatformUser(string userId, string name, string surname, int userClass)
    {
        this.userId = userId;
        this.name = name;
        this.surname = surname;
        this.userClass = userClass;
    }
}
