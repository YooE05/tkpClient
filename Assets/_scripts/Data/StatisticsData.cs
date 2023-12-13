

using FullSerializer;
public class StatisticsData
{
    [fsProperty] private int gamesPlayed;
    [fsProperty] private int winCount;

    public StatisticsData(int gamesPlayed, int winCount)
    {
        this.gamesPlayed = gamesPlayed;
        this.winCount = winCount;
    }

    public int GamesPlayed { get => gamesPlayed;}
    public int WinCount { get => winCount; set => winCount = value; }

    public override string ToString()
    {
        return string.Format("Statistics Data : [gamesPlayed - {0}]", gamesPlayed);
    }
}
