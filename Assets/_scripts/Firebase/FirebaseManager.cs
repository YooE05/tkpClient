using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseManager database;
    private FunctionsManager functions;
    private AuthManager auth;

    private static FirebaseManager instance = null;
    public static FirebaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FirebaseManager)) as FirebaseManager;
                InitializeManagers();
            }

            if (instance == null)
            {
                var obj = new GameObject("FirebaseManager");
                instance = obj.AddComponent<FirebaseManager>();
                InitializeManagers();
            }

            //if (ManagersNotInitialized())
            //    InitializeManagers();

            return instance;
        }
    }

    public  DatabaseManager Database { get => database; }
    public  FunctionsManager Functions { get => functions;  }
    public  AuthManager Auth { get => auth; }

    public bool AllManagersInitialized { get => database != null && functions != null && auth != null; }

    private static void InitializeManagers()
    {
        if (instance.database == null)
            instance.database = instance.GetComponentInChildren<DatabaseManager>();
        if (instance.functions == null)
            instance.functions = instance.GetComponentInChildren<FunctionsManager>();
        if (instance.auth == null)
            instance.auth = instance.GetComponentInChildren<AuthManager>();
    }

    //private static bool ManagersNotInitialized()
    //{
    //    return Instance.Database != null || Instance.Functions != null || Instance.Auth != null;
    //}

    private void Awake()
    {
        instance = FindObjectOfType(typeof(FirebaseManager)) as FirebaseManager;
        InitializeManagers();

        DontDestroyOnLoad(gameObject);
    }
    private void OnApplicationQuit()
    {
        instance = null;
    }

}
