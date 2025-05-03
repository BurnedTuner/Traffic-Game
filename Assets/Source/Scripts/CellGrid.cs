using System;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour, ISaveLoadDependant
{
    [SerializeField] private Vector3 _gridOrigin;
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private GameObject _cellPrefab;
    public List<List<Cell>> Cells;
    public List<List<GameObject>> CellGameObjects;

    private void Awake()
    {
        CreateGrid();
    }


    [ContextMenu("Create Grid")]
    private void CreateGrid()
    {
        Cells = new List<List<Cell>>();
        CellGameObjects = new List<List<GameObject>>();
        for (int i = 0; i < _gridSize.x; i++)
        {
            Cells.Add(new List<Cell>());
            CellGameObjects.Add(new List<GameObject>());
            for (int j = 0; j < _gridSize.y; j++)
            {
                CellGameObjects[i].Add(Instantiate(_cellPrefab));
                CellGameObjects[i][j].name += i.ToString() + j.ToString();
                CellGameObjects[i][j].transform.position = _gridOrigin + Vector3.right * _cellSize.x * i + Vector3.forward * _cellSize.z * j;
                CellGameObjects[i][j].transform.parent = transform;

                Cells[i].Add(CellGameObjects[i][j].GetComponent<Cell>());
                Cells[i][j].ParentGrid = this;
            }
        }

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
                Cells[i][j].InitializeNode();
        }
    }

    public Vector2Int GetIndexByCell(Cell cell)
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
                if (Cells[i][j] == cell)
                    return new Vector2Int(i, j);
        }

        return Vector2Int.RoundToInt(Vector2.positiveInfinity);
    }

    public Cell GetCellByIndex(int ind1, int ind2)
    {
        if (ind1 >= 0 && ind2 >= 0)
            if (Cells.Count > ind1)
                if (Cells[ind1].Count > ind2)
                    return Cells[ind1][ind2];
        return null;
    }

    public Cell GetCellByPosition(Vector3 position)
    {
        // Debug.Log(position);
        if (!IsPositionInGrid(position))
            return null;
        else
        {
            position -= _gridOrigin;
            int i = Mathf.RoundToInt(position.x / _cellSize.x);
            int j = Mathf.RoundToInt(position.z / _cellSize.z);
            return Cells[i][j];
        }
    }

    public bool IsPositionInGrid(Vector3 position)
    {
        position -= _gridOrigin;
        int i = Mathf.RoundToInt(position.x / _cellSize.x);
        int j = Mathf.RoundToInt(position.z / _cellSize.z);
        if (i >= 0 && j >= 0)
            if (Cells.Count > i)
                if (Cells[i].Count > j)
                    return true;
        return false;
    }

    public bool IsPositionFree(Vector3Int position)
    {
        if (!IsPositionInGrid(position))
            throw new IndexOutOfRangeException("Cell " + position + " is not on grid!");

        return IsPositionOfType(position, CellType.Empty);
    }

    public bool IsPositionOfType(Vector3 position, CellType cellType)
    {
        if (!IsPositionInGrid(position))
            throw new IndexOutOfRangeException("Cell " + position + " is not on grid!");

        return GetCellByPosition(position).Type == cellType;
    }

    public void LoadData(StateData stateData)
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                Destroy(CellGameObjects[i][j]);
            }
        }
        _gridOrigin = stateData.GridOrigin;
        _gridSize = stateData.GridSize;
        _cellSize = stateData.CellSize;
        CreateGrid();
    }

    public void SaveData(ref StateData stateData)
    {
        stateData.GridOrigin = _gridOrigin;
        stateData.GridSize = _gridSize;
        stateData.CellSize = _cellSize;
    }
}
