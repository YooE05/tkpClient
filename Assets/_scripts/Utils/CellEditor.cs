using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//действия скрипта работают без запуска игры
public class CellEditor : MonoBehaviour
{
    Cell cell;
    [SerializeField] int cellSize=1;
    private void Start()
    {
        cell = GetComponent<Cell>();
    }

    void Update()
    {
        SnapToGrid();
    }

    private void SnapToGrid()
    {
        int gridSize = cellSize;
        if (cell!=null)
        {transform.position = new Vector3(cell.GetGridPos().x / cellSize * gridSize, cell.GetGridPos().y / cellSize * gridSize,0); }
        
    }

}

