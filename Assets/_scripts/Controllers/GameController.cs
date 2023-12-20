using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Proyecto26;

public enum GameResult
{
    WIN,
    LOSE,
    DRAW
}
public class GameController : MonoBehaviour
{

    [Header("Main Links")]
    bool isDemo;
    Grid grid;
    [SerializeField] ViewController _viewController;
    DataController _dataController;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    [SerializeField] List<ArticleTask> levelTasks = new List<ArticleTask>();
    List<ArticleTask> DemolevelTasks = new List<ArticleTask>();

    [SerializeField] List<LevelTrapSettings> trapSettings;
    [SerializeField] List<SpritesSettings> spriteSettings;

    [Header("Prefabs")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject phrasePrefab;
    [SerializeField] GameObject articlePrefab;
    [SerializeField] GameObject doorToNextLvl;
    [SerializeField] GameObject trapPrefab;
    [SerializeField] GameObject cannonPrefab;
    [SerializeField] GameObject laserPrefab;

    [Header("RoomsGridProperties")]
    int countOfRoom = 11;
    int x = 10;
    int y = 10;
    int[,] levelGrid;
    struct Neighbours
    {
        public void SetUp()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }
        public bool up;
        public bool down;
        public bool left;
        public bool right;
    }
    [SerializeField] List<Room> roomsList = new List<Room>();
    Dictionary<Vector2, int> roomsCoordinateDictionary = new Dictionary<Vector2, int>();

    int currentRoomIndex;
    Vector2 currentRoomCoordinate;

    int maxCountRoomTasks = 2;
    string[] articlesNamesArray = { "a", "the", "an", "none" };
    List<Article> allArticles = new List<Article>();
    List<Vector2> coordBetweenPhrasesParts = new List<Vector2>();
    List<Vector2> allPhrasesCoordinates = new List<Vector2>();
    [SerializeField] GameObject phraseForPreloadGrid;


    [Header("Other")]
    [HideInInspector] public int tasksCount;
    [HideInInspector] public int articlesCount;
    int letterInBlock = 5;
    //public int cellSize = 1;
    [HideInInspector] public int correctAnswers = 0;
    [SerializeField] float countOfAllPhrases = 0;

    private RestManager _restManager;




    private void Awake()
    {
        _restManager = new RestManager();

        DemolevelTasks.AddRange(levelTasks);
        if (FindObjectOfType<DataController>())
        { isDemo = false; }
        else { isDemo = true; }

        if (!isDemo)
        { _dataController = FindObjectOfType<DataController>(); }

        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    private void Start()
    {
        _viewController.ShowLoadingView();
        GetNewLevelData();

        GameEvents.current.OnExitTriggerEnter += ChangeRoomByExit;
        GameEvents.current.OnPlayerDied += GenerateLevelStruct;
        GameEvents.current.OnIncreasePoints += IncreasePoints;

    }


    private void OnDestroy()
    {
        GameEvents.current.OnIncreasePoints -= IncreasePoints;
        GameEvents.current.OnPlayerDied -= GenerateLevelStruct;
        GameEvents.current.OnExitTriggerEnter -= ChangeRoomByExit;
    }

    public void GetNewLevelData()
    {
        _viewController.HideEndLoadingView();
        if (!isDemo)
        {
            int crntLevel = _dataController.GetUserLevel();
            if (crntLevel < 1)
            {
                crntLevel = 1;
            }
            countOfRoom = (crntLevel >= 11) ? 11 : crntLevel + 1;

            LoadLevelTasks(maxCountRoomTasks, "level");
        }
        else
        {
            countOfRoom = 2;
        }

    }


    private void LoadLevelTasks(int maxTasksInRoom, string rootAddition)
    {
        var bodyStr = $"{{\"maxTasksInRoom\": {maxTasksInRoom} }}";
   
        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + rootAddition,
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            BodyString = bodyStr,
            EnableDebug = true
        };

        RestClient.GetArray<ArticleTask>(_restManager.currentRequest)
        .Then(res =>
        {
            //передать данные в дата контроллер
            _dataController._tasksArr = res;
            GenerateLevelStruct();
        })
        .Catch(err =>
        {
            var error = err as RequestException;
            Debug.Log(error.Message);
            SceneManager.LoadScene("MenuScreen");
        });
    }


    public void GenerateLevelStruct()
    {
        playerMovement.gameObject.SetActive(false);
        playerMovement.canMove = false;
        countOfAllPhrases = 0;
        needChangeRoom = false;
        _viewController.RestartGameView(playerHealth.startHealth);

        playerHealth.currentHealth = playerHealth.startHealth;
        ClearRooms();
        if (!isDemo)
        {
            allArticles.Clear();
            levelTasks.Clear();
            levelTasks.AddRange(_dataController._tasksArr);
        }
        else
        {
            allArticles.Clear();
            levelTasks.Clear();
            levelTasks.AddRange(DemolevelTasks);
        }

        levelGrid = new int[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                levelGrid[i, j] = 0;
            }
        }

        int lastX = x / 2;
        int lastY = y / 2;

        int roomReady = 0;

        //Генерируем матрицу для сетки уровня
        while (roomReady < countOfRoom)
        {
            if (levelGrid[lastX, lastY] == 0)
            {
                levelGrid[lastX, lastY] = 1;
                roomReady++;
            }

            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    {
                        if (lastY < y - 1)
                        { lastY++; }
                    }
                    break;
                case 1:
                    {
                        if (lastX < x - 1)
                        { lastX++; }


                    }
                    break;
                case 2:
                    {
                        if (lastY > 0)
                        { lastY--; }

                    }
                    break;
                case 3:
                    {
                        if (lastX > 0)
                        { lastX--; }

                    }
                    break;
                default:
                    break;
            }

        }


        //Генерируем комнаты исходя от полученной сетки
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {

                if (levelGrid[i, j] == 1)
                {
                    GenerateRoom(i, j);
                }

            }

        }
        currentRoomCoordinate = new Vector2(x / 2, y / 2);
        currentRoomIndex = roomsCoordinateDictionary[currentRoomCoordinate];
        roomsList[currentRoomIndex].gameObject.SetActive(true);
        playerMovement.grid = roomsList[currentRoomIndex].gridGO.GetComponent<Grid>();


        _viewController.HideLoadingView();
        playerMovement.gameObject.SetActive(true);
        Invoke("WaitAfterRespawn", 1);
    }

    //костыль, без задержки коротины в playerMovment двигают игрока после респавна
    public void WaitAfterRespawn()
    {
        playerMovement.canMove = true;
    }

    Neighbours CheckNeighbours(int crntX, int crntY)
    {
        Neighbours crntNeigbours = new Neighbours();
        crntNeigbours.SetUp();

        if (crntX < x - 1)
        {
            if (levelGrid[crntX + 1, crntY] == 1)
            { crntNeigbours.right = true; }
        }
        if (crntX > 0)
        {
            if (levelGrid[crntX - 1, crntY] == 1)
            { crntNeigbours.left = true; }
        }
        if (crntY > 0)
        {
            if (levelGrid[crntX, crntY - 1] == 1)
            { crntNeigbours.down = true; }
        }
        if (crntY < y - 1)
        {
            if (levelGrid[crntX, crntY + 1] == 1)
            { crntNeigbours.up = true; }
        }

        return crntNeigbours;
    }

    public void GenerateRoom(int roomX, int roomY)
    {
        allArticles.Clear();
        allPhrasesCoordinates.Clear();
        coordBetweenPhrasesParts.Clear();

        Room newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, gameObject.transform).GetComponent<Room>();
        grid = newRoom.GetComponentInChildren<Grid>();
        newRoom.gridCoordinate = new Vector2(roomX, roomY);
        newRoom.phraseDirections = "vertical";
        roomsList.Add(newRoom);
        roomsCoordinateDictionary.Add(newRoom.gridCoordinate, roomsList.Count - 1);

        playerMovement.gameObject.transform.position = new Vector3(1, 1, 0);

        grid.ClearGrid();

        articlesCount = 0;

        int minGridX = 15;
        int minGridY = 5;

        int randCountTasks = UnityEngine.Random.Range(1, maxCountRoomTasks + 1);
        if (isDemo)
        { randCountTasks = UnityEngine.Random.Range(1, 3); }

        //цикл по общему количеству заданий 
        for (int i = 0; i < randCountTasks; i++)
        {

            int lenthOfWordPart = 0;
            int phrasesPrefabOffset = 0;
            int articlePrefabOffset = (i + 1) * 2;
            minGridY += 3;


            //old для варианта с разным количеством артиклей на пропуск
            /*bool hasRepeateArticles = false;
            //Проверяем есть ли повторяющиеся артикли
            for (int j = 0; j < levelTasks[0].countOfArticles - 1; j++)
            {
                if (levelTasks[0].articles[j] == levelTasks[0].articles[j + 1])
                {
                    hasRepeateArticles = true;
                    break;
                }
            }*/

            //цикл по общему блоков фраз в задании
            for (int j = 0; j < levelTasks[0].articlesCount; j++)
            {
                countOfAllPhrases++;
                articlesCount++;
                // k = j;

                //определяем длину части фразы, чтобы понять сколько отводить на неё блоков
                if (j == 0 && levelTasks[0].firstPhrase != "")
                {
                    lenthOfWordPart = 1 + levelTasks[0].firstPhrase.Length / letterInBlock;
                    phrasesPrefabOffset += lenthOfWordPart;
                }
                int phraseY = minGridY / randCountTasks / 2 + minGridY / randCountTasks * i + 1;
                int phraseX = minGridX / 4 + phrasesPrefabOffset;

                //спавним префаб фразы
                Phrase crntPhrase = GetInstanceGO(phrasePrefab, phraseX, phraseY, newRoom.phrasesContainer.transform).GetComponent<Phrase>();

                //настраиваем части фразы
                if (j == 0 && levelTasks[0].firstPhrase != "")
                {
                    crntPhrase.setUpFirstPart(lenthOfWordPart, levelTasks[0].firstPhrase);
                    AddPhraseCoordinates(lenthOfWordPart, phraseY, crntPhrase, "first");
                    //k++;
                }
                lenthOfWordPart = CountPhraseLenth(0, j, ref phrasesPrefabOffset);
                crntPhrase.setUpSecondPart(lenthOfWordPart, levelTasks[0].phrases[j]);
                AddPhraseCoordinates(lenthOfWordPart, phraseY, crntPhrase, "second");

                //Сделать так, чтобы фраза спавнилась за комнатой и перемещалась в нужное место только после настройки(Модификация под вопросом)

                crntPhrase.correctArticle = levelTasks[0].articles[j];
                //создаём и настраиваем артикль
                Article crntArticle;
                //спавн четырёх артиклей
                for (int k = 0; k < 4; k++)
                {

                    crntArticle = GetInstanceGO(articlePrefab, -100, -100, newRoom.articlesContainer.transform).GetComponent<Article>();
                    crntArticle.SetArticleText(articlesNamesArray[k]);
                    allArticles.Add(crntArticle);
                }
                #region Old Article Spawn
                /* if (j == 0)
                 {
                     //спавн четырёх артиклей
                     for (int k = 0; k < 4; k++)
                     {

                         crntArticle = GetInstanceGO(articlePrefab, -100, -100, newRoom.articlesContainer.transform).GetComponent<Article>();
                         crntArticle.SetArticleText(articlesNamesArray[k]);
                         allArticles.Add(crntArticle);
                     }

                 }
                 else
                 if (hasRepeateArticles)
                 {
                     crntArticle = GetInstanceGO(articlePrefab, -100, -100, newRoom.articlesContainer.transform).GetComponent<Article>();
                     crntArticle.SetArticleText(crntPhrase.correctArticle);
                     allArticles.Add(crntArticle);

                     crntArticle = GetInstanceGO(articlePrefab, -100, -100, newRoom.articlesContainer.transform).GetComponent<Article>();
                     string needAricle = GetAnotherArticle(crntPhrase);

                     crntArticle.SetArticleText(needAricle);



                     allArticles.Add(crntArticle);
                     //спавн нужного
                     //спавно случайного из оставшихся
                 }
                 else
                 {
                     if (i % 4 == 0 && i != 0)
                     {
                         //спавн четырёх артиклей
                         for (int k = 0; k < 4; k++)
                         {
                             crntArticle = GetInstanceGO(articlePrefab, -100, -100, newRoom.articlesContainer.transform).GetComponent<Article>();
                             //Article crntArticle = GetInstanceGO(articlePrefab, 4 + articlePrefabOffset, 2, newRoom.articlesContainer.transform).GetComponent<Article>();
                             crntArticle.SetArticleText(articlesNamesArray[k]);
                             allArticles.Add(crntArticle);
                         }
                     }
                 }
 */
                #endregion
                phrasesPrefabOffset++;
                articlePrefabOffset++;
            }

            if (phrasesPrefabOffset + 5 > minGridX)
            { minGridX = phrasesPrefabOffset + 5; }

            //костыль - код должен выполняться в клауд функциии !!refactor

            levelTasks.Remove(levelTasks[0]);

        }
        //вариант для квадратного поля
        //grid.gridSide = Mathf.Max(minGridY, minGridX);
        grid.gridSideX = minGridX;
        grid.gridSideY = minGridY;


        Neighbours crntRoomNeighbours = CheckNeighbours(roomX, roomY);
        grid.GenerateGrid(crntRoomNeighbours.left, crntRoomNeighbours.right, crntRoomNeighbours.up, crntRoomNeighbours.down, spriteSettings);
        SetUpPhrasesCells();
        MoveArticles(newRoom);
        ClearSpaceBetweenPhraseParts();

        //выбор случайного набора настроек из scriptable object
        int num = UnityEngine.Random.Range(0, trapSettings.Count);

        if (trapSettings[num].needRandomCount)
        {
            PutTheTraps(newRoom, UnityEngine.Random.Range(1, trapSettings[num].trapCount + 1));
            PutTheLasers(newRoom, UnityEngine.Random.Range(1, trapSettings[num].lasersCount + 1));
            PutTheCannons(newRoom, UnityEngine.Random.Range(1, trapSettings[num].cannonsCount + 1));
        }
        else
        {
            //герерация артиклей
            PutTheTraps(newRoom, trapSettings[num].trapCount);
            //герерация лазеров
            PutTheLasers(newRoom, trapSettings[num].lasersCount);
            //герерация пушек
            PutTheCannons(newRoom, trapSettings[num].cannonsCount);
        }

        grid.ClearExitCells();
        newRoom.gameObject.SetActive(false);

    }

    private void ClearSpaceBetweenPhraseParts()
    {
        Vector2 spaceCoord;
        for (int i = 0; i < coordBetweenPhrasesParts.Count; i++)
        {
            spaceCoord = coordBetweenPhrasesParts[i];
            grid.cellsDictionary[spaceCoord].currentObject = null;
        }
    }

    private string GetAnotherArticle(Phrase crntPhrase)
    {
        string needAricle = articlesNamesArray[UnityEngine.Random.Range(0, 4)];
        if (needAricle == crntPhrase.correctArticle)
        {
            needAricle = GetAnotherArticle(crntPhrase);
        }

        return needAricle;
    }

    private void AddPhraseCoordinates(int lenthOfWordPart, int phraseY, Phrase crntPhrase, string part)
    {
        float midleOfPhrase;
        if (part == "first")
        {
            midleOfPhrase = crntPhrase.gameObject.transform.position.x - grid.cellSize * (float)(lenthOfWordPart / 2.0 + 0.5);
        }
        else
        {
            midleOfPhrase = crntPhrase.gameObject.transform.position.x + grid.cellSize * (float)(lenthOfWordPart / 2.0 + 0.5);
        }

        int startPhraseBlock;
        if (Math.Ceiling(midleOfPhrase) > midleOfPhrase)
        { startPhraseBlock = (int)(Math.Ceiling(midleOfPhrase) - lenthOfWordPart / 2); }
        else
        {
            startPhraseBlock = (int)(midleOfPhrase - (lenthOfWordPart - 1) / 2);
        }
        if (part != "first")
        {
            allPhrasesCoordinates.Add(new Vector2(startPhraseBlock - 1, phraseY));
            coordBetweenPhrasesParts.Add(new Vector2(startPhraseBlock - 1, phraseY));
        }
        for (int k = 0; k < lenthOfWordPart; k++)
        {

            allPhrasesCoordinates.Add(new Vector2(startPhraseBlock + k, phraseY));
        }
    }

    private void MoveArticles(Room crntRoom)
    {
        Vector2 articleCoord;
        for (int i = 0; i < allArticles.Count; i++)
        {
            articleCoord = GetFreeGridCoordinate(crntRoom);
            grid.cellsDictionary[articleCoord].currentObject = allArticles[i].gameObject;
            allArticles[i].transform.position = articleCoord;
        }
    }

    private void PutTheTraps(Room crntRoom, int trapCount)
    {
        Vector2 trapCoord;
        for (int i = 0; i < trapCount; i++)
        {
            trapCoord = GetFreeGridCoordinate(crntRoom, "trap");
            grid.cellsDictionary[trapCoord].currentObject = GetInstanceGO(trapPrefab, trapCoord.x, trapCoord.y, crntRoom.transform);
        }
    }

    float trapRotationAngle;
    private void PutTheLasers(Room crntRoom, int laserCount)
    {
        trapRotationAngle = 0f;
        Vector2 trapCoord;
        for (int i = 0; i < laserCount; i++)
        {
            trapCoord = GetFreeGridCoordinate(crntRoom, "laser");
            grid.cellsDictionary[trapCoord].currentObject = GetInstanceGO(laserPrefab, trapCoord.x, trapCoord.y, crntRoom.transform, trapRotationAngle);
            grid.lasersList.Add(grid.cellsDictionary[trapCoord].currentObject.GetComponentInChildren<LaserLine>());
        }
    }
    private void PutTheCannons(Room crntRoom, int cannonsCount)
    {
        trapRotationAngle = 0f;
        Vector2 trapCoord;
        for (int i = 0; i < cannonsCount; i++)
        {
            trapCoord = GetFreeGridCoordinate(crntRoom, "cannon");
            grid.cellsDictionary[trapCoord].currentObject = GetInstanceGO(cannonPrefab, trapCoord.x, trapCoord.y, crntRoom.transform, trapRotationAngle);

            grid.cannonsList.Add(grid.cellsDictionary[trapCoord].currentObject.GetComponent<Cannon>());

        }

    }
    private void SetUpPhrasesCells()
    {
        Vector2 phraseBlockCoord;
        for (int i = 0; i < allPhrasesCoordinates.Count; i++)
        {
            phraseBlockCoord = allPhrasesCoordinates[i];
            grid.cellsDictionary[phraseBlockCoord].currentObject = phraseForPreloadGrid;
        }
    }

    Vector2 GetFreeGridCoordinate(Room crntRoom, string trapType = "article")
    {
        int i = -100, j = -100;
        switch (trapType)
        {
            case "article":
                {
                    i = UnityEngine.Random.Range(2, grid.gridSideX - 2);
                    j = UnityEngine.Random.Range(2, grid.gridSideY - 2);
                    break;
                }
            case "trap":
                {
                    i = UnityEngine.Random.Range(2, grid.gridSideX - 2);
                    j = UnityEngine.Random.Range(2, grid.gridSideY - 2);
                    break;
                }
            case "cannon":
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {

                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            i = 0;
                            trapRotationAngle = -90f;
                        }
                        else
                        {
                            i = grid.gridSideX;
                            trapRotationAngle = 90f;
                        }
                        j = UnityEngine.Random.Range(2, grid.gridSideY - 2);
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            j = 0;
                            trapRotationAngle = Mathf.Epsilon;
                        }
                        else
                        {
                            j = grid.gridSideY;
                            trapRotationAngle = 180f;
                        }
                        i = UnityEngine.Random.Range(2, grid.gridSideX - 2);
                    }
                    break;
                }
            case "laser":
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        i = 0;
                        trapRotationAngle = -90f;
                    }
                    else
                    {
                        trapRotationAngle = 90f;
                        i = grid.gridSideX;
                    }

                    j = UnityEngine.Random.Range(2, grid.gridSideY - 2);
                    break;
                }
            default:
                break;
        }


        if (grid.cellsDictionary[new Vector2(i, j)].currentObject != null)// || (trapType == "cannon")&& (i == grid.gridSideX/2|| j == grid.gridSideY / 2))
        {

            return GetFreeGridCoordinate(crntRoom, trapType);
        }
        else
        {

            return new Vector2(i, j);
        }

    }

    public void ClearRooms()
    {
        if (roomsList.Count > 1)
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                roomsList[i].gridGO.GetComponent<Grid>().ClearGrid();
                Destroy(roomsList[i].gameObject);
            }
            roomsCoordinateDictionary.Clear();
            roomsList.Clear();
        }
    }
    GameObject GetInstanceGO(GameObject prefab, float xOffset, float yOffset, Transform parentTransform, float rotationAngle = 0f)
    {
        Quaternion quat = Quaternion.Euler(0, 0, rotationAngle);

        float zOffset = (rotationAngle != 0f) ? -2.5f : 0.51f;
        return Instantiate(prefab, new Vector3(xOffset, yOffset, zOffset), quat, parentTransform);
    }
    private int CountPhraseLenth(int i, int j, ref int prefabOffset)
    {
        int lenthOfWordPart = 1 + levelTasks[i].phrases[j].Length / letterInBlock;
        prefabOffset += lenthOfWordPart;
        return lenthOfWordPart;
    }

    bool needChangeRoom;
    void ChangeRoomByExit(Vector2 nextRoomDirection)
    {
        needChangeRoom = true;
        foreach (Phrase phrase in roomsList[currentRoomIndex].gameObject.GetComponentsInChildren<Phrase>())
        {
            phrase.CheckPhraseСorrectness();
        }

        if (needChangeRoom)
        {
            playerMovement.grid.StopCannonsShooting();
            //отключить текущую комнату
            roomsList[currentRoomIndex].gameObject.SetActive(false);

            //включить следующую по направлению
            Vector2 nextRoomCoordinate = currentRoomCoordinate + nextRoomDirection;
            int nextRoomIndex = roomsCoordinateDictionary[nextRoomCoordinate];
            roomsList[nextRoomIndex].gameObject.SetActive(true);
            currentRoomIndex = nextRoomIndex;
            currentRoomCoordinate = nextRoomCoordinate;

            //поменять сетку передвижения игрока на текущую комнату
            playerMovement.grid = roomsList[currentRoomIndex].gridGO.GetComponent<Grid>();

            //переместить игрока в крайнюю точку противоположной стороны
            playerMovement.gameObject.transform.position = GetPlayerCoordinateInNextRoom(nextRoomDirection);
            playerMovement.grid.StartCannonsShooting();
        }

    }
    private Vector2 GetPlayerCoordinateInNextRoom(Vector2 nextRoomDirection)
    {
        int x, y;
        if (nextRoomDirection.x == 0)
        {
            x = playerMovement.grid.gridSideX / 2;
            y = nextRoomDirection.y == 1 ? 0 : playerMovement.grid.gridSideY;
        }
        else
        {
            y = playerMovement.grid.gridSideY / 2;
            x = nextRoomDirection.x == 1 ? 0 : playerMovement.grid.gridSideX;
        }

        return new Vector2(x, y);
    }

    public void ShowTheDoor(bool act)
    {
        //doorToNextLvl.SetActive(act);
    }

    void IncreasePoints()
    {
        //если больше 70 процентов артиклей верны - показать кнопку для перехода на след уровень
        Debug.Log(Convert.ToInt32(_viewController.pointsText.text));
        Debug.Log(countOfAllPhrases * 0.7f);
        if (Convert.ToInt32(_viewController.pointsText.text) >= countOfAllPhrases * 0.7f)
        {
            _viewController.ShowExitButton();
        }

    }
    private bool _isDataUpdating;
    public void EndGame()
    {
        _isDataUpdating = true;

        _viewController.ShowGameEndPanel(playerHealth.startHealth, (int)countOfAllPhrases);
        playerMovement.canMove = false;

        _dataController.IncreaseLevel();

        int points = Convert.ToInt32(_viewController.pointsText.text);

        Debug.Log(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
        UpdateUserProgress(new UserProgress(points, _dataController.GetUserLevel(), DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")), "users/update/progress");
    }
    private void UpdateUserProgress(UserProgress bodyObject, string rootAddition)
    {

        _restManager.currentRequest = new RequestHelper
        {
            Uri = _restManager._root + rootAddition,
            Headers = new Dictionary<string, string> {
            { "Authorization", "Bearer "+ _dataController.GetJwtToken() }  },
            Body = bodyObject,
            EnableDebug = true
        };

        RestClient.Post(_restManager.currentRequest)
        .Then(res =>
        {
            _isDataUpdating = false;
        })
        .Catch(err =>
        {
            _isDataUpdating = false;

            var error = err as RequestException;
            Debug.Log(error.Message);

        });
    }
    public void OnReturnToMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(TryBackToMenu());
    }
    IEnumerator TryBackToMenu()
    {
        _viewController.ShowEndLoadingView();

        while (_isDataUpdating)
        {
            yield return null;
        }

        _viewController.HideEndLoadingView();
        SceneManager.LoadScene("MenuScreen");
    }

}
