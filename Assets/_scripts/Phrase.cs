using Mirror;
using UnityEngine;
using TMPro;

public class Phrase : NetworkBehaviour
{
    private WorldInitializer _worldInitializer;
    public string correctArticle;

    enum prasesOrientations
    {
        vertical,
        horizontal
    }

    [SerializeField] prasesOrientations prasesOrientation;

    [SyncVar] [HideInInspector] public bool haveFirstPart = false;
    [SyncVar] [HideInInspector] public bool haveSecondPart = true;

    [SerializeField] GameObject firstPart;
    [SerializeField] GameObject secondPart;

    [SyncVar] public string firstPartText;
    [SyncVar] public string secondPartText;
    [SyncVar] public int firstPartSize = 1;
    [SyncVar] public int secondPartSize = 1;

    [SyncVar] [HideInInspector] public bool haveArticle = false;

    [SyncVar] [HideInInspector] public bool isPassed = false;

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
        
        _worldInitializer = FindObjectOfType<WorldInitializer>();
        // GameEvents.current.OnExitTriggerEnter += CheckPhraseСorrectness;
    }

    private void OnDestroy()
    {
        // GameEvents.current.OnExitTriggerEnter -= CheckPhraseСorrectness;
    }

    public void SetupByOwn()
    {
        if (!string.IsNullOrEmpty(firstPartText))
            SetUpFirstPart(firstPartSize, firstPartText);
        SetUpSecondPart(secondPartSize, secondPartText);
    }

    //РЕФАКТОР - соединить два метода в один
    public void SetUpFirstPart(int partSize, string partText)
    {
        firstPartSize = partSize;
        firstPartText = partText;

        firstPart.GetComponentInChildren<Canvas>().gameObject.GetComponent<RectTransform>().localScale =
            new Vector3(1f / firstPartSize, 1);
        firstPart.GetComponentInChildren<TextMeshProUGUI>().text = partText;
        firstPart.SetActive(true);
        if (prasesOrientation == prasesOrientations.horizontal)
        {
            firstPart.transform.localScale = new Vector3(firstPartSize, cellSize, 0);
            firstPart.transform.position =
                new Vector3(gameObject.transform.position.x - cellSize * (float) (firstPartSize / 2.0 + 0.5),
                    gameObject.transform.position.y, 0);
        }
        else
        {
            firstPart.transform.localScale = new Vector3(1, firstPartSize, cellSize);
            firstPart.transform.position = new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y + cellSize * (float) (firstPartSize / 2.0 + 0.5), 0);
        }
    }

    public void SetUpSecondPart(int partSize, string partText)
    {
        secondPartSize = partSize;
        secondPartText = partText;

        secondPart.GetComponentInChildren<Canvas>().gameObject.GetComponent<RectTransform>().localScale =
            new Vector3(1f / secondPartSize, 1);
        secondPart.GetComponentInChildren<TextMeshProUGUI>().text = secondPartText;
        secondPart.SetActive(true);

        if (prasesOrientation == prasesOrientations.horizontal)
        {
            secondPart.transform.localScale = new Vector3(secondPartSize, cellSize, 0);
            secondPart.transform.position =
                new Vector3(gameObject.transform.position.x + cellSize * (float) (secondPartSize / 2.0 + 0.5),
                    gameObject.transform.position.y, 0);
        }
        else
        {
            secondPart.transform.localScale = new Vector3(1, secondPartSize, cellSize);
            secondPart.transform.position = new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y - cellSize * (float) (secondPartSize / 2.0 + 0.5), 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "article")
        {
            haveArticle = true;
            var exitArticle = collision.gameObject.GetComponent<Article>();
            string enterArticle = exitArticle.selfArticle;

            if (enterArticle == correctArticle)
            {
                isPassed = true;
                exitArticle.TurnCorrect();
                _worldInitializer.CheckAllArticles();
            }
            else
            {
                isPassed = false;
                exitArticle.TurnUsual();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "article")
        {
            haveArticle = false;
            isPassed = false;
            var exitArticle = collision.gameObject.GetComponent<Article>();
            exitArticle.TurnUsual();
        }
    }

//вызывать из GameController
    public void CheckPhraseCorrectness()
    {
        if (haveArticle)
        {
            if (isPassed)
            {
                if (isFirstCorrectAnswer)
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