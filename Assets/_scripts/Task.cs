using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksData
{
   public Task[] tasks;
}

[System.Serializable]
public class Task
{
    public int countOfArticles;
    public string firstPhrase;
    public string[] articles;
    public string[] phrases;
}
