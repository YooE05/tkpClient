using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuizJudge
{
    List<IQuizPlayer> Players { get; set; }
    public void RegisterPlayer(IQuizPlayer player);
    //public void OnPlayerReady(IQuizPlayer player);
    public void DetermineRoundWinner();
    public IQuizPlayer DetermineGameWinner();
    public void PostGameResult();
    public void OnPlayerGameLeft(IQuizPlayer player);
}
