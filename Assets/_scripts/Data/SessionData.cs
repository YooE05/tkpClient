
using System.Collections.Generic;
using System.Linq;
using FullSerializer;

/// <summary>
/// Also add some session data if needed
/// </summary>
public class SessionData
{
    [fsProperty] private string gameId;
    [fsProperty] private string status;

    [fsProperty] private Dictionary<string, bool> users = new Dictionary<string, bool>();
    
    [fsProperty] private Quiz quiz;
    [fsProperty] private string winner;
    public string Id { get => gameId; }
    public string Status { get => status; }
    public string Winner { get => winner; }
    public Dictionary<string, bool> Users { get => users; set => users = value; }
    public Quiz Quiz { get => quiz; set => quiz = value; }

    public SessionData(string id, string status)
    {
        this.gameId = id;
        this.status = status;
    }
    public SessionData()
    {
    }
    public SessionData(string gameId)
    {
        this.gameId = gameId;
    }

    public void CopyFrom(SessionData data)
    {
        setId(data.Id);
        setStatus(data.status);
        setQuiz(data.quiz);
        setWinner(data.Winner);
    }

    public void setId(string id)
    {
        this.gameId = id;
    }
    public void setStatus(string status)
    {
        this.status = status;
    }
    public void setQuiz(Quiz quiz)
    {
        this.quiz = quiz;
    }
    public void setWinner(string winnerId)
    {
        this.winner = winnerId;
    }

    public override string ToString()
    {
        return $"SessionData : [id = {gameId}, users = {Utils.CollectionUtils.DictionaryKeysToString(users)}]";
    }
}



