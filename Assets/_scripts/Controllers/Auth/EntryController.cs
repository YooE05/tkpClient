using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum AuthRegErrors { signIn, Reg }

public class EntryController : MonoBehaviour
{
    [SerializeField] private AuthorithationView _authView;
    [SerializeField] private RegistrationView _regView;

    [SerializeField] Text _errorMessageTxt;

    private RestManager _restManager;
    private InitUserData _userData;
    private DataController _dataController;

    private void Awake()
    {
        _dataController = FindObjectOfType<DataController>();
        _restManager = new RestManager();
    }

    private void Start()
    {
        _errorMessageTxt.text = "";
        //����� �� ������� ����� � ������, ���� ����� ��� �������
        _authView.InitView(GetInitUserCreds());
        OnGoToLogin();
    }

    public void OnGoToRegistration()
    {
        _authView.HidePanel();
        _regView.ShowPanel();
    }
    public void OnGoToLogin()
    {
        _regView.HidePanel();
        _authView.ShowPanel();

    }



    public void OnLogInClick()
    {
        UserCreditionals _userCreds = _authView.GetLoginData();

        //�������� ����� ��������
        _authView.ShowLoadingView();
        Login(_userCreds, "users/login");
    }
    private void Login(UserCreditionals enterDataModel, string rootAddition)
    {
        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + rootAddition,
            Params = new Dictionary<string, string> { },
            Body = enterDataModel,
            EnableDebug = true
        };

        RestClient.Post<InitUserData>(_restManager.currentRequest)
        .Then(res =>
        {

            SaveLocalUserCreds(enterDataModel);

            //��������� ����� ��������
            _authView.HideLoadingView();

            _userData = res;
            //�������� ������ � ���� ����������
            _dataController.SetUserData(_userData);
            //��������� ����� � ����
            SceneManager.LoadScene("MenuScreen");
            //   Debug.Log(_userData.crntlvl);
        })
        .Catch(err =>
        {
            var error = err as RequestException;
            //��������� ����� ��������
            _authView.HideLoadingView();

            if (error.StatusCode == 404)
            {
                _errorMessageTxt.text = "incorrect username or password";
            }
            else if (error.StatusCode == 400)
            {
                _errorMessageTxt.text = "wrong password";
            }
            Debug.Log(err);
        });
    }

    public void OnConfirmRegistrationClick()
    {
        UserRegistrationData _userRegData = _regView.GetRegistrationData();

        //��������� �� ������ �� ������ � �� ����� �� ����� ��������

        //�������� ����� ��������
        _regView.ShowLoadingView();
        Register(_userRegData, "users/create");
    }
    private void Register(UserRegistrationData enterDataModel, string rootAddition)
    {
        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + rootAddition,
            Params = new Dictionary<string, string> { },
            Body = enterDataModel,
            EnableDebug = true
        };

        RestClient.Post(_restManager.currentRequest)
        .Then(res =>
        {
            //��������� ����� ��������
            _regView.HideLoadingView();

            OnGoToLogin();
            //��������� �� ����� �����������
        })
        .Catch(err =>
        {
            //��������� ����� ��������
            _regView.HideLoadingView();

            var error = err as RequestException;
            if (error.StatusCode == 400)
            {
                _errorMessageTxt.text = "incorrect enter data";
            }
            else if (error.StatusCode == 404)
            {
                _errorMessageTxt.text = "user already exist";
            }

        });
    }


    private void SaveLocalUserCreds(UserCreditionals creds)
    {
        PlayerPrefs.SetString("username", creds.username);
        PlayerPrefs.SetString("password", creds.password);
    }
    private UserCreditionals GetInitUserCreds()
    {
        UserCreditionals initCreds = new UserCreditionals(PlayerPrefs.GetString("username"), PlayerPrefs.GetString("password"));

        if (initCreds.username != null)
        { return initCreds; }
        else
        { return null; }

    }
}
