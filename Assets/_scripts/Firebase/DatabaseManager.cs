using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.ServerSentEvents;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using BestHTTP;

/* Bug list
 * 1. Requests not working with 'Params = parameters'. Adding parameters to url by concatenating. Is problem on client or firebase?
 * 2. GetJson return not correct json data. Like {\"id\" : 5} (with backslashes)
 */

/*
 * TODO
 * 1. Почисти некоторые методы используя новый метод BuildUrl
 */

public class DatabaseManager : MonoBehaviour
{
    //Assign root path of your realtime database here or in Awake method
    private static string root;

    private void Awake()
    {
        root = FirebaseProjectConfigurations.REALTIME_DATABASE_ROOT_PATH;
    }

    /* GET */
    public void GetObject<T>(string path, Action<T> onGet, Action<Exception> onFailed = null)
    {
        GetObject<T>(DatabaseManager.root, path, onGet, onFailed);
    }
    public void GetObject<T>(string root, string path, Action<T> onGet, Action<Exception> onFailed = null)
    {
        string url = root + "/" + path + ".json";


        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.GetArray<T>(url, (exception, response, notUsed) =>
        {
            if (exception != null)
            {
                Debug.LogError($"[Database manager] Exception while getting data dy url - {url}. Exception - {exception.Message}");
                onFailed(exception);
            }
            else
            {
                Debug.Log($"[Database manager] Json data was get by url - {url}");
                T parsed = (T)StringSerializationAPI.Deserialize(typeof(T), response.Text);
                onGet(parsed);
            }
        });
    }
    //Warning. refactor. Bad realisation. returns json with like {\"id\":5}
    public void GetJson(string path, Action<string> onGet, Action<Exception> onFailed = null)
    {
        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "GET",
            Timeout = 15,
            Params = parameters
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Json data was get by url - {url}");
            string jsonResponse = System.Text.Encoding.UTF8.GetString(response.Data);
            onGet(jsonResponse);
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Error while getting data by url - {url}");
            onFailed(exception);
        });
    }
    public void GetValue(string path, Action<string> onGet, Action<Exception> onFailed = null)
    {
        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "GET",
            Timeout = 15,
            Params = parameters
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Data was get by url - {url}");
            onGet(response.Text);
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Error while getting data by url - {url}. Message - {exception.Message}");
            onFailed(exception);
        });
    }

    /* PUT */
    public void PutObject<T>(string path, T @object, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        PutJson(path, StringSerializationAPI.Serialize(typeof(T), @object), onSuccess, onFailed);
    }
    public void PutJson(string path, string json, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(json);

        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "PUT",
            Params = parameters,
            ContentType = "text/plain",
            BodyRaw = rawData
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Success putting data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while putting data by url - {url}");
            onFailed?.Invoke(exception);
        });
    }
    public void PutValue(string path, object value, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        if (value is string)
            value = "\"" + value + "\"";

        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(value.ToString());
        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "PUT",
            ContentType = "text/plain",
            Headers = { { "X-HTTP-Method-Override", "PUT" } },
            BodyRaw = rawData
    }).Then(response =>
        {
            Debug.Log($"[Database manager] Success putting data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while putting data by url - {url}");
            onFailed?.Invoke(exception);
        });
    }
    public IEnumerator PutRequest(string path)
    {
        byte[] dataToPut = System.Text.Encoding.UTF8.GetBytes("\"Hello, This is a test\"");
        UnityWebRequest uwr = UnityWebRequest.Put(BuildUrl(path), IntToByteArray(100));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
    public IEnumerator PutRequest2()
    {
        // this string is the data that needs to be send to the server
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(20.ToString());
        string APIUrl = BuildUrl("newtest/test");
        // The headers settings in order for the server to accept the PUT request
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        headers["X-HTTP-Method-Override"] = "PUT";
        // the postData and header settings are send to the server
        WWW api = new WWW(APIUrl, postData, headers);

        yield return api;

        // the response of the server can be read in the Debug Log
        Debug.Log(api.text);
    }
    public void PutValueBestHttp(string path, object value, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        string url = BuildUrl(path);
        var request = new BestHTTP.HTTPRequest(new Uri(url), methodType: BestHTTP.HTTPMethods.Post, callback: OnRequestFinished);
        request.RawData = System.Text.Encoding.UTF8.GetBytes("test");
        request.Send();
    }
    private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        Debug.Log("BestHttp response as text - " + resp.DataAsText);
    }
    /* POST */
    public void PostObject<T>(string path, T @object, Action onSuccess = null, Action<Exception> onFailed = null)
    {

    }
    public void PostJson(string path, string json, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        string url = BuildUrl(path);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(json);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "POST",
            ContentType = "application/json", //by default
            BodyRaw = rawData
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Success patch data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while patch data by url - {url}");
            onFailed?.Invoke(exception);
        });
    }

    /* PATCH */
    public void PatchObject<T>(string path, T @object, Action onSuccess = null, Action<Exception> onFailed = null)
    {

    }
    public void PatchJson(string path, string json, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        string url = BuildUrl(path);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(json);


        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "PATCH",
            ContentType = "text/plain",
            BodyRaw = rawData
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Success patch data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while patch data by url - {url}");
            onFailed?.Invoke(exception);
        });

    }
    public void PatchValue(string path, string value, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        string url = BuildUrl(path);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(value);


        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "PATCH",
            ContentType = "text/plain",
            BodyRaw = rawData
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Success patch data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while patch data by url - {url}");
            onFailed?.Invoke(exception);
        });

    }
    /* DELETE */
    public void Delete(string path, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        string url = BuildUrl(path);

        RestClient.Request(new RequestHelper
        {
            Uri = url,
            Method = "DELETE"
        }).Then(response =>
        {
            Debug.Log($"[Database manager] Success deleting data by url - {url}");
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError($"[Database manager] Exception while deleting data by url - {url}");
            onFailed?.Invoke(exception);
        });
    }
    
    /* TRANSACTION */
    public void MakeTransactionInt(string path, int value, Action onSuccess = null, Action<Exception> onFailed = null)
    {
        //edit. add realisation
        //byte[] rawData = System.Text.Encoding.UTF8.GetBytes("\"" + value + "\"");
        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        RestClient.Get(new RequestHelper
        {
            Uri = url,
            Headers = new Dictionary<string, string> { { "X-Firebase-ETag", "true" } }
        }).Then(response =>
        {
            string Etag = response.Request.GetResponseHeader("ETag");
            //refactor. bad code. Не понял как сделать PUT запрос и положить туда int
            int counter = 0;

            if (response.Request.downloadHandler.text != "null")
                counter = int.Parse(response.Request.downloadHandler.text);

            counter += value;
            Debug.Log("Temp counter value:");
            byte[] rawData = System.Text.Encoding.UTF8.GetBytes(counter.ToString());

            RequestHelper requestHelper = new RequestHelper
            {
                Uri = url,
                Headers = new Dictionary<string, string> { { "if-Match", Etag } },
                Method = "PUT",
                ContentType = "text/plain",
                BodyRaw = rawData
            };

            return RestClient.Request(requestHelper);
        }).Then(response =>
        {
            Debug.Log("Transaction write to online was success. Response - " + response.ToString());
            onSuccess?.Invoke();
        }).Catch(exception =>
        {
            Debug.LogError("Error while writing transaction. Message - " + exception.Message);
            onFailed?.Invoke(exception);
        });
    }

    /* LISTEN */
    public EventSource ListenForValueChanged(string path, Action<string> onNewMessage, Action<Exception> onError = null)
    {
        string url = BuildUrl(path);

        EventSource listener = new EventSource(new Uri(url), 1);

        void OnMessage(EventSource eventSource, Message message)
        {
            if (message.Data == null || message.Data == "null")
                return;

            string value = JSONNode.Parse(message.Data)["data"].Value;

            if (value == null || value == "null")
                return;

            onNewMessage(value);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        listener.On("put", OnMessage);
        listener.On("patch", OnMessage);
#elif UNITY_EDITOR || UNITY_STANDALONE
        listener.OnMessage += OnMessage;
#endif

        listener.OnError += (eventSource, message) =>
        {
            Debug.LogError($"Error while listening {listener.Uri}. Message - {message}");
            onError?.Invoke(new Exception(message));
        };

        listener.OnOpen += (eventSource) => { Debug.Log($"Listener {eventSource.ConnectionKey} opened!"); };

        listener.Open();

        return listener;
    }

    //helpers
    private string BuildUrl(string path)
    {
        string url = root + "/" + path + ".json";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            parameters.Add("ns", FirebaseProjectConfigurations.PROJECT_ID);
        if (FirebaseManager.Instance.Auth.IdToken != null)
            parameters.Add("auth", FirebaseManager.Instance.Auth.IdToken);

        //refactor. fix bug 1 and delete
        if (parameters.Count > 0)
            url += "?" + RequestParamsToString(parameters);

        return url;
    }
    private string RequestParamsToString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
    public bool IsNumericType(object o)
    {
        switch (Type.GetTypeCode(o.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }
    public byte[] IntToByteArray(int value)
    {
        byte[] intBytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(intBytes);

        return intBytes;
    }
    //public async Task<string> GetDataAsync()
    //{
    //    RestClient.Get(new RequestHelper
    //    {
    //        Uri = "",
    //        Headers = new Dictionary<string, string> { { "X-Firebase-ETag", "true" } }
    //    }).Then(response =>
    //    {
    //        return "";
    //    });
    //}
}
