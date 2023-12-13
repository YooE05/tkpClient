using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    //Assign you firebase project api key here or in Awake method
    private string projectApiKey = FirebaseProjectConfigurations.PROJECT_API_KEY;

    private string userId;
    private string idToken;

    public  string UserId { get => userId; }
    public  string IdToken { get => idToken; }

    private void Awake()
    {
        projectApiKey = FirebaseProjectConfigurations.PROJECT_API_KEY;
    }

    //refactor
    //update in next version
    public bool HasActiveSession()
    {
        if (string.IsNullOrEmpty(FirebaseManager.Instance.Auth.GetRefreshToken()))
            return false;

        string idToken;
        string userId;
#if UNITY_EDITOR
        idToken = PlayerPrefs.GetString("idToken");
        userId = PlayerPrefs.GetString("userId");
#elif UNITY_WEBGL && !UNITY_EDITOR
        idToken = HttpCookie.GetCookie("idToken");
        userId = HttpCookie.GetCookie("userId");
#endif
        
        if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(userId))
            return true;

        return false;
    }

    
    public void RefreshToken(string refreshToken, Action onSuccess, Action onFailed)
    {
        RestClient.Post<FirebaseRefreshTokenResponse>(new RequestHelper
        {
            Uri = "https://securetoken.googleapis.com/v1/token?key=" + projectApiKey,
            ContentType = "application/x-www-form-urlencoded",
            SimpleForm = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            }
        }).Then(response =>
        {
            SetRefreshToken(response.refresh_token);
            SetIdToken(response.id_token);
            SetLocalId(response.user_id);

            Debug.Log($"[Auth] Tokens was updated and set. IdToken - {idToken}, refreshToken - {GetRefreshToken()}");
            onSuccess.Invoke();
        }).Catch(exception =>
        {
            Debug.Log($"Exception while refreshing token. Exception - {exception.Message}");
            onFailed.Invoke();
        });
    }

    public void SignInEmailPassword(string email, string password, Action<FirebaseSignResponse> onSignedIn, Action<Exception> onFailed)
    {
        string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        string url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + projectApiKey;
        RestClient.Post<FirebaseSignResponse>(url, json).Then(
            response =>
            {
                Debug.Log($"User signed in successfully. LocalId - ${response.localId}, IdToken - {response.idToken}");
                
                userId = response.localId;
                idToken = response.idToken;

                onSignedIn(response);
            }).Catch(error =>
            {
                Debug.LogError("Failed to sign in");
                onFailed(error);
            });
    }
    public void SignUpEmailPassword(string email, string password, Action<FirebaseSignResponse> onSignedUp, Action<Exception> onFailed)
    {
        string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        string url = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + projectApiKey;
        RestClient.Post<FirebaseSignResponse>(url, json).Then(
            response =>
            {
                Debug.Log($"User signed up successfully. LocalId - " + response.localId);
                
                userId = response.localId;
                idToken = response.idToken;

                onSignedUp(response);
            }).Catch(error =>
            {
                Debug.LogError("Failed to sign up");
                onFailed(error);
            });
    }
    public void SendPasswordResetEmail(string email, Action<bool> onSent)
    {
        string json = "{\"email\":\"" + email + "\",\"requestType\":\"PASSWORD_RESET\"}";
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(json);

        RestClient.Post<FirebaseSignResponse>(new RequestHelper
        {
            Uri = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + projectApiKey,
            ContentType = "application/json",
            BodyRaw = rawData,
            Headers = new Dictionary<string, string>
            {
                {"X-Firebase-Locale", "Ru" }
            }
        }).Then(response =>
        {
            Debug.Log($"Password reset email sent!");
            onSent(true);
        }).Catch(exception =>
        {
            Debug.LogError($"Failed to send password reset email! Message - {exception.Message}");
            onSent(false);
        });
    }
    public void LogOut()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#elif UNITY_WEBGL && !UNITY_EDITOR
        HttpCookie.RemoveCookie("idToken");
        HttpCookie.RemoveCookie("userId");
        HttpCookie.RemoveCookie("refreshToken");
#endif
    }
    //Temp method for working with firebase emulator
    
    //public void CheckIsSignedIn(Action<bool> onChecked)
    //{
    //    idToken = PlayerPrefs.GetString("idToken");
    //    userId = PlayerPrefs.GetString("userId");

    //    FirebaseManager.Instance.Database.GetObject<object>(FirebaseProjectConfigurations.PLATFORM_DATABASE_ROOT_PATH, $"authTest", (json) =>
    //    {
    //        Debug.Log($"Success testing connection. User is signed in. Response: {json}");
    //        onChecked(true);
    //    }, (exception) =>
    //    {
    //        Debug.LogError($"Exception while testing connection. Message - {exception.Message}");
    //        onChecked(false);
    //    });
    //}

    public void CheckDatabaseAccess(Action<bool> onChecked, Action onFailed)
    {
        FirebaseManager.Instance.Database.GetObject<object>(FirebaseProjectConfigurations.PLATFORM_DATABASE_ROOT_PATH, $"authTest", (json) =>
        {
            Debug.Log($"Success testing connection. User is signed in. Response: {json}");
            onChecked(true);
        }, (exception) =>
        {
            Debug.LogError($"Exception while testing connection. Message - {exception.Message}");
            onChecked(false);
        });
    }
    public void TryUpdateToken()
    {

    }

    public string GetRefreshToken()
    {
#if UNITY_EDITOR
        return PlayerPrefs.GetString("refreshToken");
#elif UNITY_WEBGL && !UNITY_EDITOR
        return HttpCookie.GetCookie("refreshToken");
#endif
    }
    public void SetRefreshToken(string token)
    {
#if UNITY_EDITOR
        PlayerPrefs.SetString("refreshToken", token);
#elif UNITY_WEBGL && !UNITY_EDITOR
        HttpCookie.SetCookie("refreshToken", token, string.Empty, "/", string.Empty, string.Empty);
#endif
    }
    public void SetLocalId(string id)
    {
#if UNITY_EDITOR
        PlayerPrefs.SetString("userId", id);
#elif UNITY_WEBGL && !UNITY_EDITOR
        HttpCookie.SetCookie("userId", id, string.Empty, "/", string.Empty, string.Empty);
#endif
        this.userId = id;
    }
    public void SetIdToken(string idToken)
    {
#if UNITY_EDITOR
        PlayerPrefs.SetString("idToken", idToken);
#elif UNITY_WEBGL && !UNITY_EDITOR
        HttpCookie.SetCookie("idToken", idToken, string.Empty, "/", string.Empty, string.Empty);
#endif
        this.idToken = idToken;
    }
}

[System.Serializable]
public class FirebaseSignResponse
{
    public string localId;
    public string idToken;
    public string refreshToken;
}

[System.Serializable]
public class FirebaseRefreshTokenResponse
{
    public string access_token;
    public string expires_in;
    public string token_type;
    public string refresh_token;
    public string id_token;
    public string user_id;
    public string project_id;
}
