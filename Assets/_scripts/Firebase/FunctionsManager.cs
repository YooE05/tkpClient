using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

public class FunctionsManager : MonoBehaviour
{
    Dictionary<string, int> calledFunctionsDict = new Dictionary<string, int>();

    public void CallCloudFunction(string functionName, Dictionary<string, object> @params, Action<FunctionsResponse> onSuccess = null, Action<Exception> onFailed = null)
    {
        Debug.Log($"Starting CallCloudFunction {functionName}");

        string projectId = FirebaseProjectConfigurations.PROJECT_ID;
        string url;
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
        {
            int port = FirebaseProjectConfigurations.CLOUD_FUNCTIONS_PORT;
            url = $"http://localhost:{port}/{projectId}/us-central1/{functionName}";
        }
        else
        {
            url = $"https://us-central1-{projectId}.cloudfunctions.net/{functionName}";
        }

        url += "?" + RequestParamsToString(@params);

        if (!calledFunctionsDict.ContainsKey(functionName))
            calledFunctionsDict.Add(functionName, 1);
        else
            calledFunctionsDict[functionName]++;

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "GET",
            Timeout = 15,
            Headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + FirebaseManager.Instance.Auth.IdToken }
            }
        }).Then(response =>
        {
            Debug.Log($"[Functions manager] Function was called successfully by url - {url}");
            onSuccess(new FunctionsResponse(response.Text, response.StatusCode));
        }).Catch(exception =>
        {
            Debug.LogError($"[Functions manager] Error while calling function data by url - {url}. Message - {exception.Message}");

            //refactor. danger, bad code ?
            if (exception.Message.Contains("401") && calledFunctionsDict[functionName] < 2)
            {
                Debug.Log("Authorization error. Trying update token and call again...");
                RetryCallFunction(functionName, @params, exception, onSuccess, onFailed);
            }
            else
            {
                calledFunctionsDict.Remove(functionName); //refactor bad code ?
                onFailed(exception);
            }
        });
    }
    private void RetryCallFunction(string functionName, Dictionary<string, object> @params, Exception firstException, Action<FunctionsResponse> onSuccess = null, Action<Exception> onFailed = null)
    {
        //try update id token
        FirebaseManager.Instance.Auth.RefreshToken(FirebaseManager.Instance.Auth.GetRefreshToken(), () =>
        {
            //try again call function
            FirebaseManager.Instance.Functions.CallCloudFunction(functionName, @params, onSuccess, onFailed);
        }, () =>
        {
            Debug.LogError("Error while updating refresh token");
            calledFunctionsDict.Remove(functionName);
            onFailed(firstException);
        });
    }
    public void CallCloudFunctionPostObject<T>(string functionName, T obj, Dictionary<string, string> @params, Action<long> onSuccess = null, Action<Exception> onFailed = null)
    {
        Debug.Log($"Starting CallCloudFunction {functionName}");

        string projectId = FirebaseProjectConfigurations.PROJECT_ID;
        string url;
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
        {
            int port = FirebaseProjectConfigurations.CLOUD_FUNCTIONS_PORT;
            url = $"http://localhost:{port}/{projectId}/us-central1/{functionName}";
        }
        else
        {
            url = $"https://us-central1-{projectId}.cloudfunctions.net/{functionName}";
        }

        Debug.Log("Temp " + "Bearer " + FirebaseManager.Instance.Auth.IdToken);

        string body = StringSerializationAPI.Serialize(typeof(T), obj);


        if (!calledFunctionsDict.ContainsKey(functionName))
            calledFunctionsDict.Add(functionName, 1);
        else
            calledFunctionsDict[functionName]++;

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "POST",
            Timeout = 15,
            BodyString = body,
            ContentType = "application/json",
            Params = @params,
            Headers = new Dictionary<string, string>
            {
                //{"Authorization", "Bearer " +  FirebaseManager.Instance.Auth.IdToken }
                //test
                {"Authorization", "Bearer " + (calledFunctionsDict[functionName] == 1 ? "Fake" : FirebaseManager.Instance.Auth.IdToken) }
            }
        }).Then(response =>
        {
            //not working
            //if(response.StatusCode == 401)
            //{
            //    Debug.LogError($"[Functions manager] Error while calling function data by url - {url}. Message - {response.Error}");
            //    onFailed(new Exception(response.Error));
            //}
            //else
            //{
            //    Debug.Log($"[Functions manager] Function was called successfully by url - {url}");
            //    onSuccess(response.StatusCode);
            //}
            Debug.Log($"[Functions manager] Function was called successfully by url - {url}");
            onSuccess(response.StatusCode);

            calledFunctionsDict.Remove(functionName); //refactor bad code
        }).Catch(exception =>
        {
            Debug.Log("error");
            Debug.LogError($"[Functions manager] Error while calling function data by url - {url}. Message - {exception.Message}");

            //refactor. danger, bad code ?
            if (exception.Message.Contains("401") && calledFunctionsDict[functionName] < 2)
            {
                Debug.Log("Authorization error. Trying update token and call again...");
                RetryCallFunctionPostPostObject<T>(functionName, obj, exception, @params, onSuccess, onFailed);
            }
            else
            {
                calledFunctionsDict.Remove(functionName); //refactor bad code ?
                onFailed(exception);
            }
        });
    }
    private void RetryCallFunctionPostPostObject<T>(string functionName, T obj, Exception firstException, Dictionary<string, string> @params, Action<long> onSuccess = null, Action<Exception> onFailed = null)
    {
        //try update id token
        FirebaseManager.Instance.Auth.RefreshToken(FirebaseManager.Instance.Auth.GetRefreshToken(), () =>
        {
            //try again call function
            FirebaseManager.Instance.Functions.CallCloudFunctionPostObject<T>(functionName, obj, @params, onSuccess, onFailed);
        }, () =>
        {
            Debug.LogError("Error while updating refresh token");
            calledFunctionsDict.Remove(functionName);
            onFailed(firstException);
        });
    }

    // Helpers
    IEnumerator Post(string url, string bodyJsonString, Action<long> callback, Action<Exception> onFailed)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError($"[Functions manager] Error while calling function data by url - {url}");
            onFailed(new Exception("Connection error. Status - " + request.responseCode));
        }
        else
        {
            if (request.responseCode < 200 || request.responseCode >= 400)
            {
                Debug.LogError($"[Functions manager] Connection error. Error while calling function data by url - {url}");
                onFailed(new Exception("Error. Status - " + request.responseCode));
            }
            else
            {
                Debug.Log($"[Functions manager] Function was called successfully by url - {url}");
                callback(request.responseCode);
            }
        }
        Debug.Log("Status Code: " + request.responseCode);
    }

    private string RequestParamsToString(Dictionary<string, object> parameters)
    {
        return string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    public class FunctionsResponse
    {
        public string body;
        public long statusCode;

        public FunctionsResponse(string body, long statusCode)
        {
            this.body = body;
            this.statusCode = statusCode;
        }
    }
}


