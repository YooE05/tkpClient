using FullSerializer;
using System.Collections.Generic;

public class UserProgressData
{
    [fsProperty] private string name;
    [fsProperty] private string surname;
    [fsProperty] private int points;
    [fsProperty] private int currentLevel;
    [fsProperty] private Dictionary<string, int> pointsDict;

    public UserProgressData(string name, string surname, int score, int level)
    {
        this.name = name;
        this.surname = surname;
        this.points = score;
        this.currentLevel = level;
    }

    public string Name { get => name; set => name = value; }
    public string Surname { get => surname; set => surname = value; }
    public int Points { get => points; set => points = value; }
    public int Level { get => currentLevel; set => currentLevel = value; }
    public Dictionary<string, int> PointsDict { get => pointsDict; set => pointsDict = value; }

    public override string ToString()
    {
        return $"[name - {name}, points - {points}]";
    }
}
