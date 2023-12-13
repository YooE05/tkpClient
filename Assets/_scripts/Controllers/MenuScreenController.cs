using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuScreenController : MonoBehaviour
{
    public Text gamesPlayedText;
    public Text onlinePlayersText;
    public Text raitingValueText;
    public Text pointsValueText;
    public TextMeshProUGUI currentLevel;

    public GameObject userProgressViewPrefab;
    public GameObject userProgressViewWithModePrefab;
    public GameObject leaderboardScreen;
    public GameObject leaderboardContent;

    public GameObject settingsPanel;
    public GameObject settingsContent;

    DataController dataController;

    private void Start()
    {
        dataController = FindObjectOfType<DataController>();

        currentLevel.text = dataController.GetUserLevel().ToString();

        if (Convert.ToInt32(currentLevel.text)<1)
        {
            currentLevel.text = "1";
        }
      
        dataController.OnMenuScreenLoaded();
    }
    private void OnDestroy()
    {
        dataController.OnMenuScreenLeft();
    }

    /* SETTINGS METHODS */
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);

        UserData currentUser = dataController.GetUserData();

        UserProfileInfoView userProfileInfoView = settingsPanel.transform.FindDeepChild("UserProfileInfoView").GetComponent<UserProfileInfoView>();
        userProfileInfoView.LoadInfo(currentUser.ProfilePhoto, currentUser.Name, currentUser.Surname, 0);
    }
    public void OnLogOutClicked()
    {
        dataController.OnLoggedOut();
        SceneManager.LoadScene("Auth");
    }

    /* UI HANDLERS */
    public void OnQuitClicked()
    {
        Application.Quit();
    }

    /* LEADERBOARD METHODS */
    /* Edit
     * Remove this (OnLeaderboardTabClicked and UpdateLeaderboardView with params), 
     * if your game has NO leaderboard with MODES SWITCH
     */
    bool loadingLeaderboard;
    Toggle lastChecked;
    //refactor?
    public void OnLeaderboardTabClicked(Toggle toggle)
    {
        //string gameMode = leaderboardTabGroup.GetFirstActiveToggle().name;
        if (loadingLeaderboard)
            return;

        //if (lastChecked == toggle && toggle.isOn == false)
        if (!toggle.isOn)
            return;

        Debug.LogWarning($"[Temp] Toggle - {toggle.name} value changed");

        lastChecked = toggle;

        loadingLeaderboard = true;
        //disable all toggles
        //foreach (Transform toggle in leaderboardTabGroup.transform)
        //    toggle.GetComponent<Toggle>().interactable = false;

        //Debug.Log(toggle.gameObject.name);
        UpdateLeaderboardView(toggle.gameObject.name, () =>
        {
            loadingLeaderboard = false;

            ////enable all toggles again
            //foreach (Transform toggle in leaderboardTabGroup.transform)
            //    toggle.GetComponent<Toggle>().interactable = true;
        });
    }
    private void UpdateLeaderboardView(string level, Action onUpdated = null)
    {
        ClearLeaders();

        leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(true);
        dataController.DownloadTop10((leaderboard) =>
        {
            leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(false);
            for (int i = 0; i < leaderboard.AllUsers.Length; i++)
            {
                Debug.Log("i = " + i + "Instatiated");
                GameObject userProgressView = Instantiate(userProgressViewWithModePrefab, leaderboardContent.transform);
                userProgressView.GetComponent<UserLeaderboardViewWithModes>().loadViewData(i + 1, leaderboard.AllUsers[i], level);
            }

            onUpdated?.Invoke();
        }, level, onUpdated);
    }

    public void OpenLeaderboard()
    {
        UpdateLeaderboardView();
        leaderboardScreen.SetActive(true);
    }
    private void UpdateLeaderboardView()
    {
        ClearLeaders();

        leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(true);
        dataController.DownloadTop10((leaderboard) =>
        {
            leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(false);
            for (int i = 0; i < leaderboard.AllUsers.Length; i++)
            {
                Debug.Log("i = " + i + "Instatiated");
                GameObject userProgressView = Instantiate(userProgressViewPrefab, leaderboardContent.transform);
                userProgressView.GetComponent<UserLeaderboardView>().loadViewData(i + 1, leaderboard.AllUsers[i]);
            }
        });
    }
    private void ClearLeaders()
    {
        foreach (Transform leaderItem in leaderboardContent.transform)
        {
            Destroy(leaderItem.gameObject);
        }
    }


    /* MULTIPLAYER METHODS */
    public void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }
  /*  public void OnPlayWithFriend()
    {
        //edit. add realization

        Debug.LogWarning("OnPlatWithFriend() clicked. Empty. No Realization");
    }*/

    }
