using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Phrase : MonoBehaviour
{
    //delete
    // public enum articleTypes { a, the, none }
    //public articleTypes correctArticle;
    public string correctArticle;
    enum prasesOrientations { vertical, horizontal }
    [SerializeField] prasesOrientations prasesOrientation;

    [HideInInspector] public bool haveFirstPart = false;
    [HideInInspector] public bool haveSecondPart = true;

    [SerializeField] GameObject firstPart;
    [SerializeField] GameObject secondPart;

    public string firstPartText;
    public string secondPartText;

    public int firstPartSize = 1;
    public int secondPartSize = 1;

    [HideInInspector] public bool haveArticle = false;
    [HideInInspector] public bool isPassed = false;
    [HideInInspector] public bool isFirstCorrectAnswer = false;

    PlayerHealth playerHealth;

    int cellSize = 1;
    private void Awake()
    {
        haveArticle = false;
        isPassed = false;
        isFirstCorrectAnswer = true;

        playerHealth = FindObjectOfType<PlayerHealth>();
        firstPart.SetActive(false);
        secondPart.SetActive(false);
       // GameEvents.current.OnExitTriggerEnter += CheckPhrase—orrectness;
    }

    private void OnDestroy()
    {
       // GameEvents.current.OnExitTriggerEnter -= CheckPhrase—orrectness;
    }

    //–≈‘¿ “Œ– - ÒÓÂ‰ËÌËÚ¸ ‰‚‡ ÏÂÚÓ‰‡ ‚ Ó‰ËÌ
    public void setUpFirstPart(int partSize, string partText)
    {
        firstPartSize = partSize;
        firstPartText = partText;

        firstPart.GetComponentInChildren<Canvas>().gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f / firstPartSize, 1);
        firstPart.GetComponentInChildren<TextMeshProUGUI>().text = partText;
        firstPart.SetActive(true);
        if (prasesOrientation == prasesOrientations.horizontal)
        {
            firstPart.transform.localScale = new Vector3(firstPartSize, cellSize, 0);
            firstPart.transform.position = new Vector3(gameObject.transform.position.x - cellSize * (float)(firstPartSize / 2.0 + 0.5), gameObject.transform.position.y, 0);
        }
        else
        {
            firstPart.transform.localScale = new Vector3(1, firstPartSize, cellSize);
            firstPart.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + cellSize * (float)(firstPartSize / 2.0 + 0.5), 0);
        }
    }

    public void setUpSecondPart(int partSize, string partText)
    {
        secondPartSize = partSize;
        secondPartText = partText;

        secondPart.GetComponentInChildren<Canvas>().gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f / secondPartSize, 1);
        secondPart.GetComponentInChildren<TextMeshProUGUI>().text = secondPartText;
        secondPart.SetActive(true);

        if (prasesOrientation == prasesOrientations.horizontal)
        {
            secondPart.transform.localScale = new Vector3(secondPartSize, cellSize, 0);
            secondPart.transform.position = new Vector3(gameObject.transform.position.x + cellSize * (float)(secondPartSize / 2.0 + 0.5), gameObject.transform.position.y, 0);
        }
        else
        {
            secondPart.transform.localScale = new Vector3(1, secondPartSize, cellSize);
            secondPart.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - cellSize * (float)(secondPartSize / 2.0 + 0.5), 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "article")
        {
            haveArticle = true;
            string enterArticle = collision.gameObject.GetComponent<Article>().selfArticle.ToString();

            if (enterArticle == correctArticle.ToString())
            {

                isPassed = true;

            }
            else
            {
                isPassed = false;
            }
        }


        #region OldCode
        /*if (collision.gameObject.tag == "article")
        {
            string enterArticle = collision.gameObject.GetComponent<Article>().selfArticle.ToString();
            SpriteRenderer articleRenderer = collision.gameObject.GetComponent<SpriteRenderer>();


            if (enterArticle == correctArticle.ToString())
            {
                articleRenderer.color = Color.green;
                GameController gameController = gameObject.GetComponentInParent<GameController>();

                isPassed = true;

                gameController.correctAnswers++;
                if (gameController.correctAnswers == gameController.articlesCount)
                {
                    gameController.ShowTheDoor(true);
                }
            }
            else
            {
                articleRenderer.color = Color.red;
                GameEvents.current.TakeDamage();
            }
        }*/
        #endregion
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "article")
        {
            haveArticle = false;
            isPassed = false;
          /*  string exitArticle = collision.gameObject.GetComponent<Article>().selfArticle.ToString();

            if (exitArticle == correctArticle.ToString())
            {
                
            }*/
        }

        #region OldCode
        /*if (collision.gameObject.tag == "article")
        {

            string exitArticle = collision.gameObject.GetComponent<Article>().selfArticle.ToString();
            SpriteRenderer articleRenderer = collision.gameObject.GetComponent<SpriteRenderer>();

            articleRenderer.color = Color.yellow;

            if (exitArticle == correctArticle.ToString())
            {
                isPassed = false;

                GameController gameController = gameObject.GetComponentInParent<GameController>();

                gameController.correctAnswers--;
                gameController.ShowTheDoor(false);

            }
        }*/
        #endregion
    }

    //‚˚Á˚‚‡Ú¸ ËÁ GameController
    public void CheckPhrase—orrectness()
    {
        if(haveArticle)
        {
            if(isPassed)
            {
                if(isFirstCorrectAnswer)
                {
                    GameEvents.current.IncreasePoints();
                    isFirstCorrectAnswer = false;
                }

            }
            else
            {
                GameEvents.current.TakeDamage();
            }
        }
    }
}
