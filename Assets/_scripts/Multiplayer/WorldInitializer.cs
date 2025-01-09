using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WorldInitializer : NetworkBehaviour
{
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private List<SpritesSettings> _spriteSettings;
    [SerializeField] private List<LevelTrapSettings> _trapSettings;

    [SerializeField] private GameObject phrasePrefab;
    [SerializeField] private GameObject articlePrefab;

    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private GameObject laserPrefab;

    private Room _room;
    private Grid Grid => _room.GridComponent;

    private float _trapRotationAngle;

    private void Start()
    {
        if (isServer)
        {
            ServerGenerateRoom();
        }
        else
        {
            _room = FindObjectOfType<Room>();
            _room.GridComponent.SetupSprites(_spriteSettings);
        }
    }

    [Server]
    private void ServerGenerateRoom()
    {
        var roomGO = Instantiate(_roomPrefab, transform);
        _room = roomGO.GetComponent<Room>();
        SetupGrid();
        NetworkServer.Spawn(_room.gameObject);
    }

    [Server]
    private void SetupGrid()
    {
        _room.GridComponent.GenerateGrid(false, false, false, false, _spriteSettings);
        SetupArticles();
        SetupTraps();
    }

    private void SetupArticles()
    {
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

        PutTheTraps(_room, UnityEngine.Random.Range(1, _trapSettings[num].trapCount + 1));
        PutTheLasers(_room, UnityEngine.Random.Range(1, _trapSettings[num].lasersCount + 1));
        PutTheCannons(_room, UnityEngine.Random.Range(1, _trapSettings[num].cannonsCount + 1));
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

    GameObject GetInstanceGO(GameObject prefab, float xOffset, float yOffset, Transform parentTransform,
        float rotationAngle = 0f)
    {
        Quaternion quat = Quaternion.Euler(0, 0, rotationAngle);

        float zOffset = (rotationAngle != 0f) ? -2.5f : 0.51f;
        var instance = Instantiate(prefab, new Vector3(xOffset, yOffset, zOffset), quat, parentTransform);

        NetworkServer.Spawn(instance);
        return instance;
    }
}