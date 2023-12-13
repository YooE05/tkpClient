

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FullSerializer;
using System;

[System.Serializable]
public class UserData
{
    [fsProperty] private string id;
    [fsProperty] private string status;
    [fsProperty] private string userClass;

    [fsProperty] private UserProgressData progressData;
    [fsProperty] private StatisticsData statistics;

    [fsProperty] private Dictionary<string, string> sessions = new Dictionary<string, string>();

    [fsProperty] private Sprite profilePhotoSprite;
    [fsProperty] private string profilePhoto;

    public UserData()
    {
    }
    public UserData(string id)
    {
        this.id = id;
    }
    public UserData(string id, UserProgressData progressData, StatisticsData statisticsData)
    {
        this.id = id;
        this.progressData = progressData;
        this.statistics = statisticsData;
    }

    public void setId(string id)
    {
        this.id = id;
    }
    public void setStatus(string status)
    {
        this.status = status;
    }
    public void setUserClass(string _class)
    {
        this.userClass = _class;
    }
    public void setProfilePhotoUrl(string url)
    {
        this.profilePhoto = url;
    }
    public void setProfilePhotoSprite(Sprite photo)
    {
        this.profilePhotoSprite = photo;
    }
    public void setSessions(Dictionary<string, string> sessions)
    {
        this.sessions = sessions;
    }
    public void setProgressData(UserProgressData progressData)
    {
        this.progressData = progressData;
    }
    public void setStatistics(StatisticsData statistics)
    {
        this.statistics = statistics;
    }
    public void updatePublicData(UserProgressData progressData, StatisticsData statistics)
    {
        this.progressData = progressData;
        this.statistics = statistics;
    }

    public void updateProfilePhoto(Sprite newPhoto)
    {
        profilePhotoSprite = newPhoto;
    }
    public void updateProfilePhotoUrl(string url)
    {
        profilePhoto = url;
    }

    public void CopyFrom(UserData data)
    {
        setId(data.id);
        setStatus(data.status);
        setUserClass(data.userClass);
        setSessions(data.sessions);
        setProfilePhotoUrl(data.profilePhoto);
        updatePublicData(data.progressData, data.statistics);
        updateProfilePhoto(data.profilePhotoSprite);
    }

    [fsIgnore] public string Id { get => id;}
    [fsIgnore] public UserProgressData ProgressData { get => progressData;}
    //internal SessionData[] Sessions { get => sessions; set => sessions = value; }
    [fsIgnore] public StatisticsData Statistics { get => statistics;  }
    [fsIgnore] public Sprite ProfilePhoto { get => profilePhotoSprite;}

    [fsIgnore] public string ProfilePhotoUrl
    {
        get => profilePhoto;
        set => profilePhoto = value;
        //set => updateProfilePhoto(value);
    }
    [fsIgnore] public string FullName { get => progressData.Name + " " + progressData.Surname; }
    [fsIgnore] public string Name { get => progressData.Name; }
    [fsIgnore] public string Surname { get => progressData.Surname; }
    [fsIgnore] public string UserClass { get => userClass; }
    [fsIgnore] public string Status { get => status; }

    public override string ToString()
    {
        return string.Format("UserData : [id = {0}, progressData = {1}, statistics = {2}]", 
            id, progressData, statistics);
    }
}
