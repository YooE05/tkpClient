using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{
    [SerializeField] private AuthorithationView _authView;

    private AuthRestManager _restManager;
    private InitUserData _userData;
    private DataController _dataController;

    private void Awake()
    {
        _dataController = FindObjectOfType<DataController>();
        _restManager = new AuthRestManager();
    }

    public void OnRegisterClick()
    {

    }


    [ContextMenu("Login")]
    public void OnLogInClick()
    {
        UserCreditionals _userCreds = _authView.GetLoginData();

        //�������� ����� ��������
        Login(_userCreds, "users/login");
    }

    [ContextMenu("Register")]
    public void RegistrateUser()
    {
        UserRegistrationData _userRegData = _authView.GetRegistrationData();

        //�������� ����� ��������
        Register(_userRegData, "users/create");
    }

    public void Login(UserCreditionals enterDataModel, string rootAddition)
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
        {   //��������� ����� ��������
            _userData = res;
            //�������� ������ � ���� ����������
            _dataController.SetUserData(_userData);
            //��������� ����� � ����
            SceneManager.LoadScene("MenuScreen");
         //   Debug.Log(_userData.crntlvl);
        })
        .Catch(err =>
        {
            //��������� ����� ��������
            Debug.Log(err);
        });
    }

    public void Register(UserRegistrationData enterDataModel, string rootAddition)
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
            //��������� ����� ��������
            _userData = res;
            //��������� �� ����� �����������
        })
        .Catch(err => {
            //��������� ����� ��������
            Debug.Log(err); 
        });
    }

}
