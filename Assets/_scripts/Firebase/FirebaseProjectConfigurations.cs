using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectBuildType
{
    Debug,
    Emulator,
    Release
}

public class FirebaseProjectConfigurations : MonoBehaviour
{
    public ProjectBuildType Build;
    public int RealtimeDatabasePort = 9001;
    public int CloudFunctionsPort = 5001;
    public int AuthPort = 9099;
    public string NextSceneToLoad;

    public static ProjectBuildType PROJECT_BUILD { get => projectBuild; }
    public static bool Initialized { get => initialized; }
    public static string PROJECT_API_KEY { get => GetProjectApiKey(); }
    public static string PROJECT_ID { get => GetProjectId(); }
    public static string REALTIME_DATABASE_ROOT_PATH { get => GetRealtimeDatabaseRootPath(); }
    public static string PLATFORM_DATABASE_ROOT_PATH { get => GetPlatformDatabaseRootPath(); }

    public static int REALTIME_DATABASE_PORT { get => realtimeDatabasePort; }
    public static int CLOUD_FUNCTIONS_PORT { get => cloudFunctionsPort; }
    public static int AUTH_PORT { get => authPort; }

    private void Awake()
    {
        if (string.IsNullOrEmpty(AppConfigurations.GameName))
        {
            Debug.LogError("Game name was not set! Go to _scipts -> Controllers -> AppConfigurations");
            return;
        }

        DontDestroyOnLoad(gameObject);

        projectBuild = Build;

        realtimeDatabasePort = RealtimeDatabasePort;
        cloudFunctionsPort = CloudFunctionsPort;
        authPort = AuthPort;

        initialized = true;

        if (!string.IsNullOrEmpty(NextSceneToLoad))
            UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneToLoad);
    }

    public static string GetDatabaseUrlByPath(string path)
    {
        switch (projectBuild)
        {
            case ProjectBuildType.Debug:
                return realtime_database_root_path_release + "/" + path + ".json";
            case ProjectBuildType.Release:
                return realtime_database_root_path_release + "/" + path + ".json";
            case ProjectBuildType.Emulator:
                return platform_database_root_path_emulator + "/" + path + ".json" + "?ns=" + project_id_emulator;
            default:
                return null;
        }
    }

    private static ProjectBuildType projectBuild = ProjectBuildType.Debug;
    private static bool initialized = false;
    private static int realtimeDatabasePort = 9000;
    private static int cloudFunctionsPort = 5001;
    private static int authPort = 9099;

    private const string project_api_key_release = ""; //edit to "<Release-Api-Key>"
    private const string project_api_key_debug = "AIzaSyDLSe_JOBl0ia-nXV6Uh0g9doDFBGdVTN8"; //edit to "<Debug-Api-Key>"
    private const string project_api_key_emulator = "project_api_key";

    private const string project_id_release = "";
    private const string project_id_debug = "dzgamesdebug";
    private const string project_id_emulator = "dzgamesdebugproject";

    private static string realtime_database_root_path_release = "" + AppConfigurations.GameName;
    private static string realtime_database_root_path_debug = "https://dzgamesdebug-default-rtdb.firebaseio.com/" + AppConfigurations.GameName;
    private static string realtime_database_root_path_emulator = "http://localhost:9003/" + AppConfigurations.GameName;

    private static string platform_database_root_path_release = "";
    private static string platform_database_root_path_debug = "https://dzgamesdebug-default-rtdb.firebaseio.com";
    private static string platform_database_root_path_emulator = "http://localhost:9003";
 
    private static string GetProjectApiKey()
    {
        switch (projectBuild)
        {
            case ProjectBuildType.Debug:
                return project_api_key_debug;
            case ProjectBuildType.Release:
                return project_api_key_release;
            case ProjectBuildType.Emulator:
                return project_api_key_emulator;
            default:
                return null;
        }
    }
    private static string GetProjectId()
    {
        switch (projectBuild)
        {
            case ProjectBuildType.Debug:
                return project_id_debug;
            case ProjectBuildType.Release:
                return project_id_release;
            case ProjectBuildType.Emulator:
                return project_id_emulator;
            default:
                return null;
        }
    }
    private static string GetRealtimeDatabaseRootPath()
    {
        switch (projectBuild)
        {
            case ProjectBuildType.Debug:
                return realtime_database_root_path_debug;
            case ProjectBuildType.Release:
                return realtime_database_root_path_release;
            case ProjectBuildType.Emulator:
                return $"http://localhost:{realtimeDatabasePort}/{AppConfigurations.GameName}";
            default:
                return null;
        }
    }
    private static string GetPlatformDatabaseRootPath()
    {
        switch (projectBuild)
        {
            case ProjectBuildType.Debug:
                return platform_database_root_path_debug;
            case ProjectBuildType.Release:
                return platform_database_root_path_release;
            case ProjectBuildType.Emulator:
                return platform_database_root_path_emulator;
            default:
                return null;
        }
    }

}
