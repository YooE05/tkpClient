using FullSerializer;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public static class DataParser
{
    private static fsSerializer serializer = new fsSerializer();

    //Пробовал здесь общий метод.
    //public static void ParseData<T>(string jsonData, out T outObject)
    //{
    //    outObject = new T();

    //    var data = fsJsonParser.Parse(jsonData);
    //    object deserialized = null;
    //    serializer.TryDeserialize(data, typeof(UserData), ref deserialized);
    //    userData = deserialized as UserData;

    //    //test

    //    Debug.Log("userData.Name - " + userData.Name);
    //}

    //not working. refactor read documentation about "Advanced Customization with [fsObjectProcessor]" ?
    //public static void ParseUserData(string jsonData, out UserData userData)
    //{
    //    userData = new UserData();

    //    var data = fsJsonParser.Parse(jsonData);
    //    object deserialized = null;
    //    serializer.TryDeserialize(data, typeof(UserData), ref deserialized);
    //    userData = deserialized as UserData;

    //    //test
    //}

    public static void ParsePlatformUserData(string jsonData, out PlatformUserData platformUserData)
    {
        //edit
        platformUserData = null;
    }
    public static void ParseUserData(string jsonData, out UserData userData)
    {
        userData = new UserData();

        JSONNode userDataJsonObj = JSONNode.Parse(jsonData);

        userData.setId(userDataJsonObj["private"]["id"].Value);
        
        //getting progress data and statistics
        object deserialized = null;
        serializer.TryDeserialize(fsJsonParser.Parse(userDataJsonObj["public"]["progressData"].ToString()), typeof(UserProgressData), ref deserialized);
        UserProgressData upd = deserialized as UserProgressData;
        serializer.TryDeserialize(fsJsonParser.Parse(userDataJsonObj["public"]["statistics"].ToString()), typeof(StatisticsData), ref deserialized);
        StatisticsData sd = deserialized as StatisticsData;
        userData.updatePublicData(upd, sd);

        //getting class data
        userData.setUserClass(userDataJsonObj["public"]["userClass"].Value.ToString());
        userData.setStatus(userDataJsonObj["public"]["status"].Value.ToString());

        //getting profilePhotoUrl (private)
        userData.setProfilePhotoUrl(userDataJsonObj["public"]["profilePhoto"].Value.ToString()); //Так как без Value ставит кавычки в начале и конце
    }
    public static void ParsePublicUserData(string publicUserJsonData, UserData userData)
    {
        JSONNode publicUserDataJsonObj = JSONNode.Parse(publicUserJsonData);

        //parsing progress and statistics data
        object deserialized = null;
        serializer.TryDeserialize(fsJsonParser.Parse(publicUserDataJsonObj["progressData"].ToString()), typeof(UserProgressData), ref deserialized);
        UserProgressData upd = deserialized as UserProgressData;
        serializer.TryDeserialize(fsJsonParser.Parse(publicUserDataJsonObj["statistics"].ToString()), typeof(StatisticsData), ref deserialized);
        StatisticsData sd = deserialized as StatisticsData;
        userData.updatePublicData(upd, sd);

        //getting class data
        userData.setUserClass(publicUserDataJsonObj["userClass"].Value.ToString());
        userData.setStatus(publicUserDataJsonObj["status"].Value.ToString());

        //getting profilePhotoUrl (private)
        userData.setProfilePhotoUrl(publicUserDataJsonObj["profilePhoto"].Value.ToString()); //Так как без Value ставит кавычки в начале и конце
    }
    public static void ParseLeaderboardData(string jsonData, out LeaderboardData leaderboardData)
    {
        Debug.Log($"Starting parsing leaderboard data: {jsonData}");
        JSONNode leaderboardJsonObj = JSONNode.Parse(jsonData);

        List<UserData> users = new List<UserData>();
        foreach (var nextUserNode in leaderboardJsonObj)
        {
            UserData next = new UserData();

            //loading progress data
            object deserialized = null;
            serializer.TryDeserialize(fsJsonParser.Parse(nextUserNode.Value["progressData"].ToString()), typeof(UserProgressData), ref deserialized);
            UserProgressData upd = deserialized as UserProgressData;
            serializer.TryDeserialize(fsJsonParser.Parse(nextUserNode.Value["statistics"].ToString()), typeof(StatisticsData), ref deserialized);
            StatisticsData sd = deserialized as StatisticsData;
            next.updatePublicData(upd, sd);

            next.ProfilePhotoUrl = nextUserNode.Value["profilePhoto"].Value;

            next.setId(nextUserNode.Key);

            //Debug.Log("[Debug] Next leaderboard user was parsed. Result - " + next);

            users.Add(next);
        }

        leaderboardData = new LeaderboardData(users.ToArray());
    }
    public static void ParseSessionData(string jsonData, out SessionData sessionData)
    {
        object deserialized = null;
        serializer.TryDeserialize(fsJsonParser.Parse(jsonData), typeof(SessionData), ref deserialized);
        sessionData = deserialized as SessionData;
    }
    public static void ParseQuizData(string jsonData, out Quiz quiz)
    {
        object deserialized = null;
        serializer.TryDeserialize(fsJsonParser.Parse(jsonData), typeof(Quiz), ref deserialized);
        quiz = deserialized as Quiz;
    }

    public static TasksData ParseTasksData(string jsonData)
    {
        object deserialized = null;
        serializer.TryDeserialize(fsJsonParser.Parse(jsonData), typeof(TasksData), ref deserialized);
        return deserialized as TasksData;
    }

    public static List<NotificationData> ParseNotificationsData(string jsonData)
    {
        List<NotificationData> notifications = new List<NotificationData>();

        JSONNode notificationsJsonObj = JSONNode.Parse(jsonData);
        if (notificationsJsonObj == null)
        {
            Debug.Log("No notification. Return");
            return notifications; 
        }
        foreach (var notification in notificationsJsonObj)
        {
            NotificationData n = JsonUtility.FromJson<NotificationData>(notification.Value.ToString());
            n.setKey(notification.Key);

            //Debug.Log("[Debug] Parsed notification - " + n);
            notifications.Add(n);
        }

        //Debug.Log("[Debug] All notifications parsed:");
        Utils.CollectionUtils.ListToString<NotificationData>(notifications);

        return notifications;
    }
}
