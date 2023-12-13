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
    private string userId;
    private string idToken;

    public UnityEngine.UI.Slider progressBar;
    private int progress;

    private InitUserData _initUserData;

    private UserData userData;
    private LeaderboardData leaderboardData;


    private EventSource menuDashboardListener;


    private Stack<NotificationData> notificationPool;

    public InstructionData instructionData = new InstructionData();

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetUserData(InitUserData data)
    {
        _initUserData = data;
    }
    public int GetUserLevel()
    {
      return  _initUserData.crntLvl;
    }

  
    //Events
    public void OnMenuScreenLoaded()
    {

    }
    public void OnMenuScreenLeft()
    {
        if (menuDashboardListener != null)
            menuDashboardListener.Close();
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


    //User/Opponent data
    public UserProgressData GetUserProgressData()
    {
        return userData.ProgressData;
    }
    public StatisticsData GetUserStatisticsData()
    {
        return userData.Statistics;
    }
    public string GetUserId()
    {
        return userData.Id;
    }
    public UserData GetUserData()
    {
        return userData;
    }

    //Leaderboard data
    public LeaderboardData GetLeaderboardData()
    {
        return leaderboardData;
    }

    public int GetUserPositionInLeaderboard(string userId)
    {
        //temp. edit
        return 100;
    }

    public void DownloadTop10(Action<LeaderboardData> onLoaded)
    {
        Dictionary<string, object> args = new Dictionary<string, object>() { { "game", AppConfigurations.GameName } };
        FirebaseManager.Instance.Functions.CallCloudFunction("DownloadTop10Leaderboard", args, (data) =>
        {
            DataParser.ParseLeaderboardData(data.body, out leaderboardData);
            //edit. uncomment if you sorting leaderboard on client side
            leaderboardData.SortDescending();
            onLoaded(leaderboardData);
        }, (exception) =>
        {
            Debug.LogError("Error while downloading leaderboard data");
        });

    }
    public void DownloadTop10(Action<LeaderboardData> onLoaded, string mode, Action onFailed = null)
    {

        Dictionary<string, object> args = new Dictionary<string, object>() { { "game", AppConfigurations.GameName }, { "mode", mode } };

        FirebaseManager.Instance.Functions.CallCloudFunction("DownloadTop10LeaderboardUpdated", args, (data) =>
        {
            DataParser.ParseLeaderboardData(data.body, out leaderboardData);
            //edit. uncomment if you sorting leaderboard on client side
            //leaderboardData.SortDescending(mode);
            onLoaded(leaderboardData);
        }, (exception) =>
        {
            Debug.LogError("Error while downloading leaderboard data");
            onFailed?.Invoke();
        });

    }

   
    

    public void UpdateProgress(int currentLevel, int points, Action onLoaded)
    {
        Dictionary<string, object> @params = new Dictionary<string, object>();
        @params.Add("userId", userData.Id);
        @params.Add("currentLevel", currentLevel);
        @params.Add("points", points);

        FirebaseManager.Instance.Functions.CallCloudFunction("UpdateProgressArticlesGame", @params, (data) =>
        {

            onLoaded.Invoke();
        }, (exception) =>
        {
            Debug.LogError($"Error while calling UpdateProgressArticlesGame cloud function. Message - {exception.Message}");
        });
    }



    #region REST FIREBASE 

    FirebaseCustomYield firebaseCustomYield;
    //Loading main data
    private IEnumerator LoadAllDataREST()
    {
        firebaseCustomYield = new FirebaseCustomYield();

        //Getting Leaderboard Data
        userData = new UserData(userId);
        firebaseCustomYield.onRequestStarted();
        LoadUserData();
        yield return firebaseCustomYield;
        Debug.Log("User data was loaded. User data: " + userData);
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadLeaderboardData();
        yield return firebaseCustomYield;
      
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadOnboardingSprites();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        //edit. add loading settings, friends data, etc. if needed

        OnAllDataLoaded();


    }

        
    void LoadOnboardingSprites()
    {
        StartCoroutine(instructionData.LoadInstruction(userId));
        firebaseCustomYield.onRequestEnd();
    }
    public void LoadUserData()
    {
        FirebaseManager.Instance.Database.GetObject<UserData>($"users/{userData.Id}/public", (data) =>
        {
            userData = data;
            userData.setId(userId);

           /* StartCoroutine(GetSpriteFromURL(userData.ProfilePhotoUrl, (sprite) =>
            {
                userData.setProfilePhotoSprite(sprite);
            }));*/

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading user data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                $"Exception while downloading user data. Message - {exception.Message}");
        });
    }
    public void LoadLeaderboardData()
    {
        FirebaseManager.Instance.Database.GetObject<LeaderboardData>($"leaderboard/allUsers", (data) =>
        {
            leaderboardData = data;

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading leaderboard data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading leaderboard data. Message - " + exception.Message);
        });
    }
 
    private void OnAllDataLoaded()
    {
        Debug.Log("OnAllDataLoaded");

        //edit. uncomment, if game is online
        //RemoveFromSearchers(); //clearing data

        SceneManager.LoadScene("MenuScreen");
    }

    public void DownloadUserDataRest(string id, Action<UserData> onLoaded)
    {
        FirebaseManager.Instance.Database.GetObject<UserData>($"users/{id}/public", (data) =>
        {
            UserData ud = data;
            ud.setId(id);

            /*StartCoroutine(GetSpriteFromURL(ud.ProfilePhotoUrl, (sprite) =>
            {
                currentOpponent = ud;
                currentOpponent.updateProfilePhoto(sprite);

                onLoaded(ud);//refactor. update in next versions. «агружай картинку независимо от данных
            }));*/
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading user data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading user data. Message - " + exception.Message);
        });

        Debug.LogWarning("Downloading opponent data at url - " + $"/users/{id}/public.json?auth={idToken}");
    }

    public void OnLoggedOut()
    {
        //edit. uncomment if game has live online session
        //RemoveFromSearchers(); //clearing data

        FirebaseManager.Instance.Auth.LogOut();

        DestroySelf();
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Other network methods
    private IEnumerator GetSpriteFromURL(string url, Action<Sprite> callback)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error downloading texture. Error - " + www.error);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Warning, "Error downloading texture. Error - " + www.error);
            callback.Invoke(null);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite downloadedSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            callback.Invoke(downloadedSprite);
        }

    }
    #endregion

    private void UpdateProgressBar()
    {
        if (progressBar == null)
            return;

        progress += UnityEngine.Random.Range(0, 100 - progress);
        progressBar.value = progress / 100f;
    }
}
