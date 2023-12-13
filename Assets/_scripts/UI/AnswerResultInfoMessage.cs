using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerResultInfoMessage : MonoBehaviour
{
    public Text resultText;

    private string correctAnswerTextColor = "#0C5800";
    private string wrongAnswerTextColor = "#960000";

    public void Show(bool isAnswerCorrect)
    {
        gameObject.SetActive(true);

        resultText.text = isAnswerCorrect ? "Правильно" : "Неправильно";
        
        string textColorHTMLString = isAnswerCorrect ? correctAnswerTextColor : wrongAnswerTextColor;
        ColorUtility.TryParseHtmlString(textColorHTMLString, out Color c);
        resultText.color = c;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
