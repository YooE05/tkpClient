using Mirror;
using UnityEngine;
using TMPro;

public class Article : NetworkBehaviour
{
    [SyncVar] public string selfArticle;
    private NetworkColorChanger _colorChanger;
    private SpriteRenderer _spriteRenderer;

    private Color _ususalColor = new Color(0, 0.8383272f, 1f, 1f);
    private Color _correctColor = new Color(0.2544852f, 1f, 0f, 1f);

    private void Awake()
    {
        _colorChanger = GetComponent<NetworkColorChanger>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

        {
            selfArticle = "";
        }

        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = selfArticle;
    }

    public void TurnUsual()
    {
        _colorChanger.ChangeColor(_ususalColor);
    }

    public void TurnCorrect()
    {
        _colorChanger.ChangeColor(_correctColor);
    }
}