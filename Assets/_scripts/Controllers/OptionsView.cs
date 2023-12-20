using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OptionsView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingView;
    [SerializeField] private Text _errorMessageTxt;
    [SerializeField] private Button[] _menuButtons;

    [SerializeField] private TextMeshProUGUI _nameSurnameTxt;

    [SerializeField] private InputField _oldPasswordInput;
    [SerializeField] private InputField _newPasswordInput;
    [SerializeField] private InputField _nameInput;
    [SerializeField] private InputField _surnameInput;

    [SerializeField] private GameObject _mainOptPanel;
    [SerializeField] private GameObject _chNameOptPanlel;
    [SerializeField] private GameObject _chPasswOptPanlel;

    internal UserPersonalData GetPersonalData()
    {
        return new UserPersonalData(_nameInput.text, _surnameInput.text);
    }

    internal string GetNewPassword()
    {
        return _newPasswordInput.text;
    }

    public void InitView(string name, string surname)
    {
        _errorMessageTxt.text = "";

        _nameSurnameTxt.text = name + " " + surname;

        _oldPasswordInput.text = "";
        _newPasswordInput.text = "";
        _nameInput.text = name;
        _surnameInput.text = surname;

        //ResetInputValues();
        EnableMenuButtons();
    }

    private void ResetInputValues()
    {
        _oldPasswordInput.text = "";
        _newPasswordInput.text = "";
        _nameInput.text = "";
        _surnameInput.text = "";
    }

    public void ShowMainOptionsPanel()
    {
        _chNameOptPanlel.SetActive(false); 
        _chPasswOptPanlel.SetActive(false); 

        _mainOptPanel.SetActive(true);
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
