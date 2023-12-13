using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;


public class Quiz
{
    [fsProperty] private Dictionary<string, RoundData> rounds = new Dictionary<string, RoundData>();
    [fsProperty] private int activeRoundIndex;

    public int RoundsCount { get => rounds.Count; }

    public Quiz()
    {
    }
    public Quiz(Dictionary<string, RoundData> rounds)
    {
        this.rounds = rounds;
    }

    public void setRounds(Dictionary<string, RoundData> rounds)
    {
        this.rounds = rounds;
    }

    public int GetActiveRoundIndex()
    {
        return activeRoundIndex;
    }
    public RoundData GetActiveRoundData()
    {
        if (activeRoundIndex >= rounds.Count || activeRoundIndex < 0)
            return null;
        else
            return rounds["round" + activeRoundIndex];
    }

    public void NextRound()
    {
        activeRoundIndex++;
    }
    public void AddRound(string key, RoundData data)
    {
        rounds.Add(key, data);
    }

    public void AddRoundRange(string key, RoundData data)
    {
        rounds.Add(key, data);
    }

    public RoundData GetRoundDataByIndex(int index)
    {
        string key = "round" + index;
        if (rounds.ContainsKey(key))
            return rounds[key];
        else
            return null;
    }

    public override string ToString()
    {
        return $"Quiz data : [rounds = [{Utils.CollectionUtils.DictionaryToString(rounds)}]]";
    }
}
