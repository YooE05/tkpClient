using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEndView : MonoBehaviour
{
    public Text pointsText;
    public Text raitingText;

    public Sprite winSprite;
    public Sprite loseSprite;

    private Text panelHeader;
    private Text headerText;
    private Text messageText;

    public void Init()
    {
        panelHeader = transform.FindDeepChild("PanelHeader").GetComponent<Text>();
        headerText = transform.FindDeepChild("Header").GetComponent<Text>();
        messageText = transform.FindDeepChild("Message").GetComponent<Text>();

        Hide();
    }
    public void UpdateView(GameResult result, int points, int positionInLeaderboard, bool opponentLeftSession = false)
    {
        if (opponentLeftSession)
        {
            panelHeader.text = "ПОБЕДА";
            headerText.text = "Ваш соперник покинул сессию! Вы победили!";
            messageText.text = "Заработано";

            transform.FindDeepChild("LoseWinImage").GetComponent<Image>().sprite = winSprite;
            pointsText.text = points.ToString();
            raitingText.text = positionInLeaderboard.ToString();

            return;
        }

        switch (result)
        {
            case GameResult.WIN:
                panelHeader.text = "ПОБЕДА";
                headerText.text = "Поздравляем! Вы победили!";
                messageText.text = "Заработано";

                transform.FindDeepChild("LoseWinImage").GetComponent<Image>().sprite = winSprite;
                break;
            case GameResult.LOSE:
                panelHeader.text = "ПОРАЖЕНИЕ";
                headerText.text = "Ой! Вы проиграли!";
                messageText.text = "Заработано";

                //transform.FindDeepChild("LoseWinImage").GetComponent<Image>().sprite = loseSprite;
                transform.FindDeepChild("LoseWinImage").gameObject.SetActive(false);
                break;
            case GameResult.DRAW:
                panelHeader.text = "НИЧЬЯ";
                headerText.text = "У вас ничья!";
                messageText.text = "Заработано";

                //transform.FindDeepChild("LoseWinImage").GetComponent<Image>().sprite = loseSprite;
                transform.FindDeepChild("LoseWinImage").gameObject.SetActive(false);
                break;
            default:
                break;
        }

        pointsText.text = points.ToString();
        raitingText.text = positionInLeaderboard.ToString();
    }
    public void Show()
    {
        transform.SetAsLastSibling();
    }
    public void Hide()
    {
        transform.SetAsFirstSibling();
    }
}
