using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthorithationView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingView;
    [SerializeField] Button[] _menuButtons;

    public GameObject _authPanel;

    public InputField _logUsername;
    public InputField _logPasswordInput;


    internal UserCreditionals GetLoginData()
    {
        return new UserCreditionals(_logUsername.text, _logPasswordInput.text);
    }
    internal void ShowPanel()
    {
        ResetView();
        _authPanel.SetActive(true);
    }
    internal void HidePanel()
    {
        _authPanel.SetActive(false);
    }

    public void InitView(UserCreditionals initCreds)
    {
       // _logUsername.text = "";
     //   _logPasswordInput.text = "";

        if (initCreds.username != null && initCreds.password != null)
        {
            _logUsername.text = initCreds.username;
            _logPasswordInput.text = initCreds.password;
        }

        _authPanel.SetActive(true);
        _loadingView.SetActive(false);
        EnableMenuButtons();
    }

    private void ResetView()
    {
       // _logUsername.text = "";
     //   _logPasswordInput.text = "";

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
}


