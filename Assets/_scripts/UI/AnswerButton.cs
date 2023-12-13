using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public string value; //before or after

    public Sprite selectedSprite;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    private Image highlightImage;
    private Sprite defaultSprite;

    //private GameController gameController;
    private Button button;

    private void Awake()
    {
        highlightImage = transform.Find("HighlightImage").GetComponent<Image>();
        defaultSprite = highlightImage.sprite;
    }
    // Use this for initialization
    private void Start()
    {
        //gameController = FindObjectOfType<GameController>();
        button = GetComponent<Button>();

        Clear();
        Activate();
    }
    public void HandleClick()
    {
        HighlightAsSelected();
        Deactivate();

        //gameController.AnswerButtonClicked(this, value);
    }

    public void Activate()
    {
        if (button != null && !button.interactable)
            button.interactable = true;
    }
    public void Deactivate()
    {
        button.interactable = false;
    }
    public bool isInteractable()
    {
        return button.interactable;
    }

    bool answerIsCorrectAnimShown;
    
    public void HighlightAsSelected()
    {
        highlightImage.gameObject.SetActive(true);
        highlightImage.sprite = selectedSprite;
    }
    public void HighlightAsCorrect()
    {
        highlightImage.gameObject.SetActive(true);
        highlightImage.sprite = correctSprite;
    }
    public void HighlightAsWrong()
    {
        highlightImage.gameObject.SetActive(true);
        highlightImage.sprite = wrongSprite;
    }

    public void Clear()
    {
        highlightImage.gameObject.SetActive(false);
    }

}