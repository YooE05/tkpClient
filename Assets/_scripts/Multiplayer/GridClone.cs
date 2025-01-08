using Mirror;
using UnityEngine;

public class GridClone: NetworkBehaviour
{
    public SyncDictionary<Vector2, Cell> CellsDictionary = new SyncDictionary<Vector2, Cell>();
    public int gridSideX;
    public int gridSideY;
    public int cellSize;

    public void Construct(Grid grid)
    {
        foreach (var var in grid.cellsDictionary)
        {
            CellsDictionary.Add(var);
        }

        gridSideX = grid.gridSideX;
        gridSideY = grid.gridSideY;
        cellSize = grid.cellSize;
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