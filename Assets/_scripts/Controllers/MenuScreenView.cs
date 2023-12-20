using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuScreenView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentLevel;

    [SerializeField] private GameObject _userLeaderViewPrefab;
    [SerializeField] private GameObject _leaderboardGapPref;
    [SerializeField] private GameObject _crntUserLeaderViewPrefab;

    [SerializeField] private GameObject _leaderboardScreen;
    [SerializeField] private GameObject _leaderboardContent;

    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private Button[] _menuButtons;
    [SerializeField] private GameObject _loadingView;

    public void InitView(int levelNum)
    {
        currentLevel.text = (Mathf.Clamp(levelNum, 0, Mathf.Infinity) + 1).ToString();

        HideLoadingView();
        OpenMainScreen();
    }
    public void OnQuitClicked()
    {
        Application.Quit();
    }


    public void OpenMainScreen()
    {
        _leaderboardScreen.SetActive(false);
        _settingsPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    public void ShowLoadingView()
    {
        _loadingView.SetActive(true);
        DisableMenuButtons();
    }
    public void HideLoadingView()
    {
        _loadingView.SetActive(false);
        EnableMenuButtons();
    }
    private void EnableMenuButtons()
    {
        foreach (var btn in _menuButtons)
        {
            btn.interactable = true;
        }
    }
    private void DisableMenuButtons()
    {
        foreach (var btn in _menuButtons)
        {
            btn.interactable = false;
        }
    }


    internal UserProfileInfoView GetUserView()
    {
        _settingsPanel.SetActive(true);
        return _settingsPanel.transform.FindDeepChild("UserProfileInfoView").GetComponent<UserProfileInfoView>();
    }
    internal void ShowOptions()
    {
        _mainPanel.SetActive(false);
        _settingsPanel.SetActive(true);
    }
    internal void HideOptions()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }


    internal void ShowLeaderBoard()
    {
        _mainPanel.SetActive(false);
        _leaderboardScreen.SetActive(true);
    }
    internal void HideLeaderBoard()
    {
        _mainPanel.SetActive(true);
        _leaderboardScreen.SetActive(false);
    }
    internal void SetUpLeaderboard(LeaderBoardUser[] leaders)
    {
        for (int i = 1; i < leaders.Length; i++)
        {

            GameObject userProgressView = leaders[i].place == leaders[0].place ?
                                                                            Instantiate(_crntUserLeaderViewPrefab, _leaderboardContent.transform) :
                                                                            Instantiate(_userLeaderViewPrefab, _leaderboardContent.transform);

            userProgressView.GetComponent<UserLeaderboardView>().loadViewData(leaders[i]);

        }

        if (leaders[0].place > 10)
        {
            Instantiate(_leaderboardGapPref, _leaderboardContent.transform);

            GameObject userProgressView = Instantiate(_crntUserLeaderViewPrefab, _leaderboardContent.transform);
            userProgressView.GetComponent<UserLeaderboardView>().loadViewData(leaders[0]);
        }
    }
    internal void ClearLeaders()
    {
        foreach (Transform leaderItem in _leaderboardContent.transform)
        {
            Destroy(leaderItem.gameObject);
        }
    }

}
