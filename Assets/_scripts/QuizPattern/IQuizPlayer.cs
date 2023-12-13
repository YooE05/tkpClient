using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuizPlayer
{
    int PlayerId { get; set; }
    bool IsReady { get; set; }
    IQuizJudge Judge { get; set; } //May not be used
    bool IsAnswered { get; set; }
    object Answer { get; set; }
    int Score { get; set; }
   

    public void SetJudge(IQuizJudge judge); //May not be used
    public void OnAnswered(object answer);
    public void OnNewRound();
    public void OnRoundTimeExpired();
    public void OnRoundEnd(object result);
    public void OnGameEnd(bool playedToTheEnd, object[] args = null);
}