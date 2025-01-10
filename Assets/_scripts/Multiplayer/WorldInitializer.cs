using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = System.Random;

public class WorldInitializer : NetworkBehaviour
{
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private List<SpritesSettings> _spriteSettings;
    [SerializeField] private List<LevelTrapSettings> _trapSettings;
    [SerializeField] private List<ArticleTask> _levelTasks;
    private readonly List<ArticleTask> _copyLevelTasks = new List<ArticleTask>();

    [SerializeField] private GameObject phrasePrefab;
    [SerializeField] private GameObject articlePrefab;

    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private GameObject laserPrefab;

    [SerializeField] private GameObject _phraseForPreloadGrid;

    private Room _room;
    private Grid Grid => _room.GridComponent;

    private float _trapRotationAngle;

    private readonly string[] _articlesNamesArray = {"a", "the", "an", "none"};
    private readonly List<Article> _allArticles = new List<Article>();
    private readonly SyncList<Vector2> _coordBetweenPhrasesParts = new SyncList<Vector2>();
    private readonly SyncList<Vector2> _allPhrasesCoordinates = new SyncList<Vector2>();

    private readonly SyncDictionary<Vector2, string> _articlesValueDictionary = new SyncDictionary<Vector2, string>();

    private int _minGridX = 15;
    private int _minGridY = 5;
    private readonly int _letterInBlock = 5;

    private ExitGameManager _exitGameManager;

    [SerializeField] private int _phraseMinCount = 2;
    [SerializeField] private int _phraseMaxCount = 6;

    [SyncVar] public int DeadCount;
    [SyncVar] public int numPlayers;

    [SyncVar] public int _correctArticles;
    [SyncVar] public float _spentTime;
    [SyncVar] public bool GameIsEnded;

    private void Awake()
    {
        _spentTime = 0;
        GameIsEnded = false;

        _correctArticles = 0;
        //GameEvents.current.OnPlayerDied += CheckAllPlayerDeath;
        DeadCount = 0;
        _exitGameManager = GetComponent<ExitGameManager>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        UpdatePlayerCount();
    }

    [ServerCallback]
    private void UpdatePlayerCount()
    {
        numPlayers = NetworkServer.connections.Count; // Количество подключённых игроков
    }

    [Server]
    private void ReturnToLobby()
    {
        Debug.Log("All dead");

        _exitGameManager.ExitGameScene();

        /*var players = FindObjectsOfType<PlayerHealth>(true);

        foreach (var playerHealth in players)
        {
            playerHealth.ResetHealth();
        }

        if (isServer)
        {
            ReGenerateRoom();
        }
        else
        {
            Invoke(nameof(ReGenerateRoom), 3f);
        }*/
    }

    private void Start()
    {
        GenerateRoom();
    }

    private void ReGenerateRoom()
    {
        if (isServer)
        {
            ServerGenerateRoom();
        }
        else
        {
            _room = FindObjectOfType<Room>();
            _room.GridComponent.SetupSprites(_spriteSettings);
            ClSetupArticlesAndPhrasesValues();

            SetUpPhrasesCells();
            ClearSpaceBetweenPhraseParts();
        }


        var playersMovement2 = FindObjectsOfType<PlayerMovement>(true);

        foreach (var playerMovement in playersMovement2)
        {
            playerMovement.Grid = Grid;
        }
    }

    private void GenerateRoom()
    {
        if (isServer)
        {
            ServerGenerateRoom();
        }
        else
        {
            _room = FindObjectOfType<Room>();
            _room.GridComponent.SetupSprites(_spriteSettings);
            ClSetupArticlesAndPhrasesValues();

            SetUpPhrasesCells();
            ClearSpaceBetweenPhraseParts();

            // SetupClientPointsText();
        }

        SetupClientPointsText();

        foreach (var cannon in Grid.cannonsList)
        {
            cannon.StartShooting();
        }
    }

    private void Update()
    {
        if (!GameIsEnded)
        {
            _spentTime += Time.deltaTime;
        }

        UpdatePlayerCount();
        if (DeadCount == numPlayers)
        {
            ReturnToLobby();
            DeadCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (var cannon in Grid.cannonsList)
            {
                cannon.StartShooting();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (var cannon in Grid.cannonsList)
            {
                cannon.StopShooting();
            }
        }
    }

    private void ClSetupArticlesAndPhrasesValues()
    {
        _allArticles.Clear();
        _allArticles.AddRange(FindObjectsOfType<Article>());

        foreach (var article in _allArticles)
        {
            var position = article.transform.position;
            var coordinates = new Vector2(position.x, position.y);
            article.SetArticleText(_articlesValueDictionary[coordinates]);
        }

        var _phrases = FindObjectsOfType<Phrase>();
        foreach (var phrase in _phrases)
        {
            phrase.SetupByOwn();
        }
    }

    [Server]
    private void ServerGenerateRoom()
    {
        _allArticles.Clear();
        _copyLevelTasks.Clear();
        _copyLevelTasks.AddRange(_levelTasks);

        ShafleList();

        if (_room != null)
        {
            _room.GridComponent.ClearGrid();
            Destroy(_room.gameObject);
        }

        var roomGO = Instantiate(_roomPrefab, transform);
        _room = roomGO.GetComponent<Room>();

        SetupArticles();

        SetupGrid();

        SetUpPhrasesCells();
        MoveArticles();
        ClearSpaceBetweenPhraseParts();

        NetworkServer.Spawn(_room.gameObject);
    }

    private void ShafleList()
    {
        Random random = new Random();

        for (int i = _copyLevelTasks.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            var temp = _copyLevelTasks[i];
            _copyLevelTasks[i] = _copyLevelTasks[j];
            _copyLevelTasks[j] = temp;
        }
    }

    [Server]
    private void SetupGrid()
    {
        _room.GridComponent.GenerateGrid(false, false, false, false, _spriteSettings);
        SetupTraps();
    }

    [Server]
    private void SetupArticles()
    {
        _articlesValueDictionary.Clear();
        _allPhrasesCoordinates.Clear();

        int maxCountRoomTasks = 2;
        var countOfAllPhrases = 0;
        var articlesCount = 0;

        _minGridX = 15;
        _minGridY = 5;

        if (_phraseMaxCount > _levelTasks.Count)
        {
            _phraseMaxCount = _levelTasks.Count;
        }

        var randCountTasks = UnityEngine.Random.Range(_phraseMinCount, _phraseMaxCount);

        for (int i = 0; i < randCountTasks; i++)
        {
            int lenthOfWordPart = 0;
            int phrasesPrefabOffset = 0;
            int articlePrefabOffset = (i + 1) * 2;
            _minGridY += 3;

            for (int j = 0; j < _copyLevelTasks[0].articlesCount; j++)
            {
                countOfAllPhrases++;
                articlesCount++;

                if (j == 0 && _copyLevelTasks[0].firstPhrase != "")
                {
                    lenthOfWordPart = 1 + _copyLevelTasks[0].firstPhrase.Length / _letterInBlock;
                    phrasesPrefabOffset += lenthOfWordPart;
                }

                int phraseY = _minGridY / randCountTasks / 2 + _minGridY / randCountTasks * i + 1;
                int phraseX = _minGridX / 4 + phrasesPrefabOffset;


                Phrase crntPhrase = GetInstanceGO(phrasePrefab, phraseX, phraseY, _room.phrasesContainer.transform)
                    .GetComponent<Phrase>();


                if (j == 0 && _copyLevelTasks[0].firstPhrase != "")
                {
                    crntPhrase.SetUpFirstPart(lenthOfWordPart, _copyLevelTasks[0].firstPhrase);
                    AddPhraseCoordinates(lenthOfWordPart, phraseY, crntPhrase, "first");
                }

                lenthOfWordPart = CountPhraseLenth(0, j, ref phrasesPrefabOffset);
                crntPhrase.SetUpSecondPart(lenthOfWordPart, _copyLevelTasks[0].phrases[j]);
                AddPhraseCoordinates(lenthOfWordPart, phraseY, crntPhrase, "second");

                crntPhrase.correctArticle = _copyLevelTasks[0].articles[j];

                Article crntArticle;

                for (int k = 0; k < 4; k++)
                {
                    crntArticle = GetInstanceGO(articlePrefab, -100, -100, _room.articlesContainer.transform)
                        .GetComponent<Article>();
                    crntArticle.SetArticleText(_articlesNamesArray[k]);
                    _allArticles.Add(crntArticle);
                }

                phrasesPrefabOffset++;
                articlePrefabOffset++;
            }

            if (phrasesPrefabOffset + 5 > _minGridX)
            {
                _minGridX = phrasesPrefabOffset + 5;
            }

            _copyLevelTasks.Remove(_copyLevelTasks[0]);
        }

        Grid.gridSideX = _minGridX;
        Grid.gridSideY = _minGridY;

        // Neighbours crntRoomNeighbours = CheckNeighbours(roomX, roomY);
        // Grid.GenerateGrid(crntRoomNeighbours.left, crntRoomNeighbours.right, crntRoomNeighbours.up,
        //     crntRoomNeighbours.down, _spriteSettings);
    }

    private int CountPhraseLenth(int i, int j, ref int prefabOffset)
    {
        int lenthOfWordPart = 1 + _copyLevelTasks[i].phrases[j].Length / _letterInBlock;
        prefabOffset += lenthOfWordPart;
        return lenthOfWordPart;
    }

    private void SetUpPhrasesCells()
    {
        Vector2 phraseBlockCoord;
        for (int i = 0; i < _allPhrasesCoordinates.Count; i++)
        {
            phraseBlockCoord = _allPhrasesCoordinates[i];
            Grid.cellsDictionary[phraseBlockCoord].currentObject = _phraseForPreloadGrid;
        }
    }

    private void ClearSpaceBetweenPhraseParts()
    {
        Vector2 spaceCoord;
        for (int i = 0; i < _coordBetweenPhrasesParts.Count; i++)
        {
            spaceCoord = _coordBetweenPhrasesParts[i];
            Grid.cellsDictionary[spaceCoord].currentObject = null;
        }
    }

    [Server]
    private void AddPhraseCoordinates(int lenthOfWordPart, int phraseY, Phrase crntPhrase, string part)
    {
        float midleOfPhrase;
        if (part == "first")
        {
            midleOfPhrase = crntPhrase.gameObject.transform.position.x -
                            Grid.cellSize * (float) (lenthOfWordPart / 2.0 + 0.5);
        }
        else
        {
            midleOfPhrase = crntPhrase.gameObject.transform.position.x +
                            Grid.cellSize * (float) (lenthOfWordPart / 2.0 + 0.5);
        }

        int startPhraseBlock;
        if (Math.Ceiling(midleOfPhrase) > midleOfPhrase)
        {
            startPhraseBlock = (int) (Math.Ceiling(midleOfPhrase) - lenthOfWordPart / 2);
        }
        else
        {
            startPhraseBlock = (int) (midleOfPhrase - (lenthOfWordPart - 1) / 2);
        }

        if (part != "first")
        {
            _allPhrasesCoordinates.Add(new Vector2(startPhraseBlock - 1, phraseY));
            _coordBetweenPhrasesParts.Add(new Vector2(startPhraseBlock - 1, phraseY));
        }

        for (int k = 0; k < lenthOfWordPart; k++)
        {
            _allPhrasesCoordinates.Add(new Vector2(startPhraseBlock + k, phraseY));
        }
    }

    [Server]
    private void MoveArticles()
    {
        Vector2 articleCoord;
        for (int i = 0; i < _allArticles.Count; i++)
        {
            articleCoord = GetFreeGridCoordinate();
            Grid.cellsDictionary[articleCoord].currentObject = _allArticles[i].gameObject;
            _allArticles[i].transform.position = articleCoord;
            _articlesValueDictionary.Add(articleCoord, _allArticles[i].selfArticle);
        }
    }

    public Grid GetCurrentGrid(out string hasGrid)
    {
        hasGrid = _room.GridComponent.cellsDictionary.Count.ToString();
        return _room.GridComponent;
    }

    [Server]
    private void SetupTraps()
    {
        int num = UnityEngine.Random.Range(0, _trapSettings.Count);

        PutTheTraps(_room,
            UnityEngine.Random.Range(_trapSettings[num].trapCount / 2, _trapSettings[num].trapCount + 1));
        PutTheLasers(_room,
            UnityEngine.Random.Range(_trapSettings[num].lasersCount / 2, _trapSettings[num].lasersCount + 1));
        PutTheCannons(_room,
            UnityEngine.Random.Range(_trapSettings[num].cannonsCount / 2, _trapSettings[num].cannonsCount + 1));
    }

    private void PutTheTraps(Room crntRoom, int trapCount)
    {
        Vector2 trapCoord;
        for (int i = 0; i < trapCount; i++)
        {
            trapCoord = GetFreeGridCoordinate("trap");
            Grid.cellsDictionary[trapCoord].currentObject =
                GetInstanceGO(trapPrefab, trapCoord.x, trapCoord.y, crntRoom.transform);
        }
    }

    private void PutTheLasers(Room crntRoom, int laserCount)
    {
        _trapRotationAngle = 0f;
        Vector2 trapCoord;
        for (int i = 0; i < laserCount; i++)
        {
            trapCoord = GetFreeGridCoordinate("laser");
            Grid.cellsDictionary[trapCoord].currentObject = GetInstanceGO(laserPrefab, trapCoord.x, trapCoord.y,
                crntRoom.transform, _trapRotationAngle);
            Grid.lasersList.Add(Grid.cellsDictionary[trapCoord].currentObject.GetComponentInChildren<LaserLine>());
        }
    }

    private void PutTheCannons(Room crntRoom, int cannonsCount)
    {
        _trapRotationAngle = 0f;
        Vector2 trapCoord;
        for (int i = 0; i < cannonsCount; i++)
        {
            trapCoord = GetFreeGridCoordinate("cannon");
            Grid.cellsDictionary[trapCoord].currentObject = GetInstanceGO(cannonPrefab, trapCoord.x, trapCoord.y,
                crntRoom.transform, _trapRotationAngle);

            Grid.cannonsList.Add(Grid.cellsDictionary[trapCoord].currentObject.GetComponent<Cannon>());
        }
    }

    private Vector2 GetFreeGridCoordinate(string trapType = "article")
    {
        int i = -100, j = -100;
        switch (trapType)
        {
            case "article":
            {
                i = UnityEngine.Random.Range(2, Grid.gridSideX - 2);
                j = UnityEngine.Random.Range(2, Grid.gridSideY - 2);
                break;
            }
            case "trap":
            {
                i = UnityEngine.Random.Range(2, Grid.gridSideX - 2);
                j = UnityEngine.Random.Range(2, Grid.gridSideY - 2);
                break;
            }
            case "cannon":
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        i = 0;
                        _trapRotationAngle = -90f;
                    }
                    else
                    {
                        i = Grid.gridSideX;
                        _trapRotationAngle = 90f;
                    }

                    j = UnityEngine.Random.Range(2, Grid.gridSideY - 2);
                }
                else
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        j = 0;
                        _trapRotationAngle = Mathf.Epsilon;
                    }
                    else
                    {
                        j = Grid.gridSideY;
                        _trapRotationAngle = 180f;
                    }

                    i = UnityEngine.Random.Range(2, Grid.gridSideX - 2);
                }

                break;
            }
            case "laser":
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    i = 0;
                    _trapRotationAngle = -90f;
                }
                else
                {
                    _trapRotationAngle = 90f;
                    i = Grid.gridSideX;
                }

                j = UnityEngine.Random.Range(2, Grid.gridSideY - 2);
                break;
            }
            default:
                break;
        }

        return Grid.cellsDictionary[new Vector2(i, j)].currentObject != null
            ? GetFreeGridCoordinate(trapType)
            : new Vector2(i, j);
    }

    [Server]
    GameObject GetInstanceGO(GameObject prefab, float xOffset, float yOffset, Transform parentTransform,
        float rotationAngle = 0f)
    {
        Quaternion quat = Quaternion.Euler(0, 0, rotationAngle);

        float zOffset = (rotationAngle != 0f) ? -2.5f : 0.51f;
        var instance = Instantiate(prefab, new Vector3(xOffset, yOffset, zOffset), quat, parentTransform);

        NetworkServer.Spawn(instance);
        return instance;
    }

    [Server]
    public void CheckAllArticles()
    {
        var phrases = FindObjectsOfType<Phrase>();
        var passedCount = 0;
        foreach (var phrase in phrases)
        {
            if (phrase.isPassed)
            {
                passedCount++;
            }
        }

        RpcSetPointsText(passedCount, phrases.Length);

        _correctArticles = passedCount;

        if (passedCount == phrases.Length)
        {
            EndGame();
        }
    }

    private void SetupClientPointsText()
    {
        var phrases = FindObjectsOfType<Phrase>();
        var passedCount = 0;
        foreach (var phrase in phrases)
        {
            if (phrase.isPassed)
            {
                passedCount++;
            }
        }

        FindObjectOfType<ViewController>().pointsText.text = $"{passedCount}/{phrases.Length}";
    }

    [ClientRpc]
    private void RpcSetPointsText(int passed, int all)
    {
        FindObjectOfType<ViewController>().pointsText.text = $"{passed}/{all}";
    }

    [ClientRpc]
    private void EndGame()
    {
        var _viewController = FindObjectOfType<ViewController>();
        GameIsEnded = true;

        string formattedTime = TimeSpan.FromSeconds(_spentTime).ToString(@"m\:ss");
        _viewController.SetUpEndPanel(formattedTime);
        Debug.Log(formattedTime);
        //заморозить передвижение

        var playersMovement2 = FindObjectsOfType<PlayerMovement>(true);

        foreach (var playerMovement in playersMovement2)
        {
            playerMovement.canMove = false;
        }
    }
}