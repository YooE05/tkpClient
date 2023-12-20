using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuScreenView _menuView;
    [SerializeField] private OptionsView _optionsView;

    [SerializeField] Text _errorMessageTxt;

    private RestManager _restManager;
    private DataController _dataController;

    private void Awake()
    {
        _dataController = FindObjectOfType<DataController>();
        _restManager = new RestManager();
    }

    private void Start()
    {
        _errorMessageTxt.text = "";
        
        _menuView.InitView(_dataController.GetUserLevel());
        _optionsView.InitView(_dataController.GetName(), _dataController.GetSurname()); 
    }

    private void OnDestroy()
    {
      //  _dataController.OnMenuScreenLeft();
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainScreen()
    {
        _menuView.OpenMainScreen();
    }


    /* SETTINGS METHODS */
    public void OpenSettings()
    {
        _menuView.ShowOptions();
    }
    public void OnLogOutClicked()
    { 
        SceneManager.LoadScene("Auth");
    }
    public void OnDeleteClick()
    {
        _optionsView.ShowLoadingView();

        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + "users/deleteuser",
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            EnableDebug = true
        };

        RestClient.Delete(_restManager.currentRequest)
        .Then(res =>
        {
            _optionsView.HideLoadingView();
            OnLogOutClicked();
        })
        .Catch(err =>
        {
            _optionsView.HideLoadingView();
            var error = err as RequestException;
            Debug.Log(error.ServerMessage);

        });
    }

    public void OnChPasswordClick()
    {
        _optionsView.ShowLoadingView();
        _errorMessageTxt.text = "";

       var bodyStr = $"{{\"newPassword\": \"{_optionsView.GetNewPassword()}\" }}";

        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + "users/update/password",
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            BodyString = bodyStr,
            EnableDebug = true
        };


        RestClient.Post(_restManager.currentRequest)
        .Then(res =>
        {
            _optionsView.HideLoadingView();
            //возможно стоит вывести сообщение об успешной замене пароля
            OnLogOutClicked();
        })
        .Catch(err =>
        {
            _optionsView.HideLoadingView();
            _errorMessageTxt.text = "password wasn't updated";
            var error = err as RequestException;
            Debug.Log(error.ServerMessage);

        });
    }

    public void OnChPersonalsClick()
    {
        _optionsView.ShowLoadingView();
        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + "users/update/personal",
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            Body = _optionsView.GetPersonalData(),
            EnableDebug = true
        };

        RestClient.Post(_restManager.currentRequest)
        .Then(res =>
        {
            _optionsView.HideLoadingView();
            _optionsView.ShowMainOptionsPanel();
            _errorMessageTxt.text = " ";
            //возможно стоит вывести сообщение об успешной замене данных

            _dataController.SetName(_optionsView.GetPersonalData().name);
            _dataController.SetSurname(_optionsView.GetPersonalData().surname);

            _optionsView.InitView(_dataController.GetName(), _dataController.GetSurname());
        })
        .Catch(err =>
        {
            _optionsView.HideLoadingView();
            _errorMessageTxt.text = "data wasn't updated";
            var error = err as RequestException;
            Debug.Log(error.ServerMessage);

        });
    }



    /* LEADERBOARD METHODS */

    public void OpenLeaderboard()
    {
        UpdateLeaderboardView("users/leaders");
    }
    private void UpdateLeaderboardView(string rootAddition)
    {
        _menuView.ShowLoadingView();
        _menuView.ClearLeaders();

        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + rootAddition,
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            EnableDebug = true
        };

        RestClient.GetArray<LeaderBoardUser>(_restManager.currentRequest)
        .Then(res =>
        {
            _menuView.HideLoadingView();
            _menuView.ShowLeaderBoard();
            _menuView.SetUpLeaderboard(res);
        })
        .Catch(err =>
        {
            _menuView.HideLoadingView();

            var error = err as RequestException;
            Debug.Log(error.Message);

        });
    }




}

