using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Grid : NetworkBehaviour
{
    SpritesSettings spritesSettings;

    public readonly SyncDictionary<Vector2, Cell> cellsDictionary = new SyncDictionary<Vector2, Cell>();
    public SyncList<GameObject> wallList = new SyncList<GameObject>();

    [SyncVar] private int _spriteListId;

    [SyncVar] public int gridSideX;
    [SyncVar] public int gridSideY;
    [SyncVar] public int cellSize;

    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject upDoorPrefab;

    [SyncVar] int countOfNeighbours;
    bool needWall = true;
    List<Cell> exitCellsForTraps = new List<Cell>();
    public List<Cannon> cannonsList = new List<Cannon>();
    public List<LaserLine> lasersList = new List<LaserLine>();

    public void GenerateGrid(bool leftNei, bool rightNei, bool upNei, bool downNei, List<SpritesSettings> spriteList)
    {
        _spriteListId = GetRandomNumber(spriteList.Count);

        spritesSettings = spriteList[_spriteListId];

        countOfNeighbours = 0;
        exitCellsForTraps.Clear();
        for (int i = 0; i < gridSideX + 1; i++)
        {
            for (int j = 0; j < gridSideY + 1; j++)
            {
                Vector2 cellCoordinates = new Vector2(i, j);

                GameObject newCell = Instantiate(cellPrefab, new Vector3(cellCoordinates.x, cellCoordinates.y, 1),
                    Quaternion.identity, gameObject.transform);
                SetRandomSprite(newCell, spritesSettings.floorCells);


                if (!cellsDictionary.ContainsKey(cellCoordinates))
                {
                    cellsDictionary.Add(cellCoordinates, newCell.GetComponent<Cell>());
                }
                else if (cellsDictionary[cellCoordinates] == null)
                {
                    cellsDictionary[cellCoordinates] = newCell.GetComponent<Cell>();
                }

                NetworkServer.Spawn(newCell);

                if (i == 0 || i == gridSideX || j == 0 || j == gridSideY)
                {
                    CheckExitNessesity(leftNei, rightNei, upNei, downNei, i, j, newCell);

                    if (needWall)
                    {
                        GameObject wall = Instantiate(wallPrefab, new Vector3(cellCoordinates.x, cellCoordinates.y, -1),
                            Quaternion.identity, gameObject.transform);
                        wallList.Add(wall);
                        ChangeWallSprite(i, j, wall);
                        NetworkServer.Spawn(wall);
                    }
                    else
                    {
                        countOfNeighbours++;
                    }

                    needWall = true;
                }
            }
        }
    }

    public void SetupSprites(List<SpritesSettings> spriteList)
    {
        spritesSettings = spriteList[_spriteListId];

        for (int i = 0; i < gridSideX + 1; i++)
        {
            for (int j = 0; j < gridSideY + 1; j++)
            {
                Vector2 cellCoordinates = new Vector2(i, j);

                //GameObject newCell = Instantiate(cellPrefab, new Vector3(cellCoordinates.x, cellCoordinates.y, 1),
                //     Quaternion.identity, gameObject.transform);
                var cell = cellsDictionary[cellCoordinates];

                SetRandomSprite(cell.gameObject, spritesSettings.floorCells);


                /*if (!cellsDictionary.ContainsKey(cellCoordinates))
                {
                    cellsDictionary.Add(cellCoordinates, newCell.GetComponent<Cell>());
                }
                else if (cellsDictionary[cellCoordinates] == null)
                {
                    cellsDictionary[cellCoordinates] = newCell.GetComponent<Cell>();
                }

                NetworkServer.Spawn(newCell);*/

                if (i == 0 || i == gridSideX || j == 0 || j == gridSideY)
                {
                    CheckExitNessesity(false, false, false, false, i, j, cell.gameObject);

                    /*if (needWall)
                    {
                      //  GameObject wall = Instantiate(wallPrefab, new Vector3(cellCoordinates.x, cellCoordinates.y, -1),
                      //     Quaternion.identity, gameObject.transform);
                      //  wallList.Add(wall);
                        ChangeWallSprite(i, j, wallList.Find(go=>go.transform.position==new Vector3(cellCoordinates.x, cellCoordinates.y, -1)));
                      //  NetworkServer.Spawn(wall);
                    }
                    else
                    {
                        countOfNeighbours++;
                    }*/

                    //needWall = true;
                }

                foreach (var wall in wallList)
                {
                    ChangeWallSprite((int) wall.transform.position.x, (int) wall.transform.position.y, wall.gameObject);
                }
            }
        }
    }

    private void ChangeWallSprite(int i, int j, GameObject wall)
    {
        if (i == 0)
        {
            SetRandomSprite(wall, spritesSettings.verticalLeftWalls);
            if (j == 0)
            {
                SetRandomSprite(wall, spritesSettings.leftDownCorner);
            }
            else if (j == gridSideY)
            {
                SetRandomSprite(wall, spritesSettings.leftUpCorner);
            }
        }
        else if (i == gridSideX)
        {
            SetRandomSprite(wall, spritesSettings.verticalRightWalls);
            if (j == 0)
            {
                SetRandomSprite(wall, spritesSettings.rightDownCorner);
            }
            else if (j == gridSideY)
            {
                SetRandomSprite(wall, spritesSettings.rightUpCorner);
            }
        }
        else if (j == 0)
        {
            SetRandomSprite(wall, spritesSettings.horizontalDownWalls);
        }
        else
        {
            SetRandomSprite(wall, spritesSettings.horizontalUpWalls);
        }
    }

    int GetRandomNumber(int upperBorder, int lowerBorder = 0)
    {
        return UnityEngine.Random.Range(lowerBorder, upperBorder);
    }

    private void SetRandomSprite(GameObject newCell, List<Sprite> spriteList)
    {
        int rand = GetRandomNumber(spriteList.Count);
        var randSprite = spriteList[rand];
        newCell.GetComponent<SpriteRenderer>().sprite = randSprite;
        // RpcSetRandomSprite(newCell, randSprite);
    }

    [ClientRpc]
    private void RpcSetRandomSprite(GameObject newCell, Sprite sprite)
    {
        newCell.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void CheckExitNessesity(bool leftNei, bool rightNei, bool upNei, bool downNei, int i, int j,
        GameObject newCell)
    {
        if (j == gridSideY / 2)
        {
            newCell.GetComponent<Cell>().currentObject = newCell;
            exitCellsForTraps.Add(newCell.GetComponent<Cell>());
            if (i == 0 && leftNei)
            {
                SpawnExitCell(i, j, "left");
            }

            if (i == gridSideX && rightNei)
            {
                SpawnExitCell(i, j, "right");
            }
        }
        else if (i == gridSideX / 2)
        {
            newCell.GetComponent<Cell>().currentObject = newCell;
            exitCellsForTraps.Add(newCell.GetComponent<Cell>());
            if (j == 0 && downNei)
            {
                SpawnExitCell(i, j, "down");
            }

            if (j == gridSideY && upNei)
            {
                SpawnExitCell(i, j, "up");
            }
        }
    }

    private void SpawnExitCell(int i, int j, string exitDirection)
    {
        Vector3 exitCellCoordinates;

        switch (exitDirection)
        {
            case "up":
            {
                exitCellCoordinates = new Vector3(i, j + 1, 1);
                Instantiate(upDoorPrefab, exitCellCoordinates + Vector3.down + new Vector3(0, 0.234f, -0.5f),
                    Quaternion.identity, gameObject.transform);
            }
                break;
            case "down":
            {
                exitCellCoordinates = new Vector3(i, j - 1, 1);
            }
                break;
            case "left":
            {
                exitCellCoordinates = new Vector3(i - 1, j, 1);
            }
                break;
            case "right":
            {
                exitCellCoordinates = new Vector3(i + 1, j, 1);
            }
                break;
            default:
                exitCellCoordinates = new Vector3(i, j, 1);
                break;
        }

        GameObject exitCell = Instantiate(cellPrefab, exitCellCoordinates, Quaternion.identity, gameObject.transform);
        if (!cellsDictionary.ContainsKey(exitCellCoordinates))
        {
            cellsDictionary.Add(exitCellCoordinates, exitCell.GetComponent<Cell>());
        }
        else if (cellsDictionary[exitCellCoordinates] == null)
        {
            cellsDictionary[exitCellCoordinates] = exitCell.GetComponent<Cell>();
        }


        exitCell.GetComponent<Cell>().exitDirection = exitDirection;
        NetworkServer.Spawn(exitCell);
        needWall = false;
    }

    public void ClearExitCells()
    {
        foreach (var exit in exitCellsForTraps)
        {
            exit.currentObject = null;
        }
    }

    public void ClearGrid()
    {
        if (cellsDictionary.Count > 1)
        {
            foreach (Cell cell in FindObjectsOfType<Cell>())
            {
                Destroy(cell.gameObject);
            }

            //вариант для квадратного поля
            /*  for (int i = 0; i < (gridSide) * 4; i++)
              {
                  Destroy(wallList[i]);
              }*/
            for (int i = 0; i < 2 * (gridSideX + gridSideY) - countOfNeighbours; i++)
            {
                Destroy(wallList[i]);
            }

            cellsDictionary.Clear();
            wallList.Clear();
        }
    }

    public void StopCannonsShooting()
    {
        foreach (Cannon cannon in cannonsList)
        {
            cannon.StopShooting();
        }

        foreach (LaserLine laser in lasersList)
        {
            laser.StopBlasting();
        }
    }

    public void StartCannonsShooting()
    {
        foreach (Cannon cannon in cannonsList)
        {
            //if (cannon.isShoot)
            cannon.StartShooting();
            if (cannon.GetComponentInChildren<LaserInteraction>())
            {
                cannon.GetComponentInChildren<LaserInteraction>().ResetLaser();
            }
        }

        foreach (LaserLine laser in lasersList)
        {
            //  if(cannon.isShoot)

            laser.StartBlasting();
        }
    }

    public bool CellIsBorder(Cell checkingCell)
    {
        Vector2 cellCoordinates = checkingCell.GetGridPos();
        if (cellCoordinates.x == gridSideX - 1 || cellCoordinates.x == 1 || cellCoordinates.y == 1 ||
            cellCoordinates.y == gridSideY - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}