using UnityEngine;
using FullSerializer;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardData
{
    [fsProperty] private UserData[] allUsers;

    public UserData[] AllUsers { get => allUsers; set => allUsers = value; }
    public LeaderboardData(UserData[] allUsers)
    {
        this.allUsers = allUsers;
    }
    public UserData GetUserByName(string name)
    {
        UserData user = null;
        foreach (var u in allUsers)
        {
            if (u.ProgressData.Name == name)
            {
                user = u;
                break;
            }
        }

        return user;
    }

    //edit. delete this if your game sorts users in server
    public void SortDescending()
    {
        List<UserData> usersList = new List<UserData>();
        usersList.AddRange(allUsers);
        List<UserData> sortedUsersList = usersList.OrderByDescending(o => o.ProgressData.Points).ToList();

        allUsers = sortedUsersList.ToArray();
    }
    //public void SortAscending()
    //{
    //    List<UserData> usersList = new List<UserData>();
    //    usersList.AddRange(allUsers);
    //    List<UserData> sortedUsersList = usersList.OrderBy(o => o.ProgressData.Points).ToList();

    //    allUsers = sortedUsersList.ToArray();
    //}

    public override string ToString()
    {
        string result = "LeaderBoard - " + Utils.CollectionUtils.ArrayToString(allUsers);
        return result;
    }
}
