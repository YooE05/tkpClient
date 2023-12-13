using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Article : MonoBehaviour
{
    public string selfArticle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "trap")
        {
            collision.gameObject.SetActive(false);
        }
    }
    public void SetArticleText(string correctArticleText)
    {
        selfArticle = correctArticleText;

        if (selfArticle == "none")

        { selfArticle = ""; }

        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = selfArticle;

    }

}
