using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RegistrationView : MonoBehaviour
{

    [SerializeField] private GameObject _regPanel;
    [SerializeField] private Text _errorMessageTxt;
    [SerializeField] private Button[] _menuButtons;
    [SerializeField] private GameObject _loadingView;

    public InputField _usernameInput;
    public InputField _passwordInput;
    public InputField _nameInput;
    public InputField _surnameInput;

    internal UserRegistrationData GetRegistrationData()
    {
        return new UserRegistrationData(_nameInput.text, _surnameInput.text, _usernameInput.text, _passwordInput.text);
    }

    internal void ShowPanel()
    {
        InitView();
        _regPanel.SetActive(true);
    }
    internal void HidePanel()
    {
        _regPanel.SetActive(false);
    }


    private void InitView()
    {
        _errorMessageTxt.text = "";

        ResetInputValues();
        EnableMenuButtons();
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

    private void ResetInputValues()
    {
        _usernameInput.text = "";
        _passwordInput.text = "";
        _nameInput.text = "";
        _surnameInput.text = "";
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
