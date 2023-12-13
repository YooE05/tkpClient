
using FullSerializer;
public class NotificationData
{
    [fsProperty] private string key;
    [fsProperty] private string type;

    [fsProperty] private string gameStatus;
    [fsProperty] private string opponentId;
    [fsProperty] private string friendId;

    public string Key { get => key; }
    public string FriendId { get => friendId;  }
    public string Type { get => type;}
    public string GameStatus { get => gameStatus;}
    public string OpponentId { get => opponentId; }

    public void setKey(string key)
    {
        this.key = key;
    }

    public override string ToString()
    {
        return $"Notification : [key={key}, type={type}, friendId={friendId}, gameStatus={gameStatus}]";
    }
}
