using System;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    [SerializeField] private Vector3 _gridOrigin;
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private GameObject _cellPrefab;
    public List<List<Cell>> _grid;
    public List<List<GameObject>> _gridObjects;

    private void Awake()
    {
        CreateGrid();
    }


    [ContextMenu("Create Grid")]
    private void CreateGrid()
    {
        _grid = new List<List<Cell>>();
        _gridObjects = new List<List<GameObject>>();
        for (int i = 0; i < _gridSize.x; i++)
        {
            _grid.Add(new List<Cell>());
            _gridObjects.Add(new List<GameObject>());
            for (int j = 0; j < _gridSize.y; j++)
            {
                _gridObjects[i].Add(Instantiate(_cellPrefab));
                _gridObjects[i][j].name += i.ToString() + j.ToString();
                _gridObjects[i][j].transform.position = _gridOrigin + Vector3.right * _cellSize.x * i + Vector3.forward * _cellSize.z * j;
                _gridObjects[i][j].transform.parent = transform;

                _grid[i].Add(_gridObjects[i][j].GetComponent<Cell>());
                _grid[i][j].ParentGrid = this;
            }
        }

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
                _grid[i][j].InitializeNode();
        }
    }

    public Vector2Int GetIndexByCell(Cell cell)
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
                if (_grid[i][j] == cell)
                    return new Vector2Int(i, j);
        }

        return Vector2Int.RoundToInt(Vector2.positiveInfinity);
    }

    public Cell GetCellByIndex(int ind1, int ind2)
    {
        if (ind1 >= 0 && ind2 >= 0)
            if (_grid.Count > ind1)
                if (_grid[ind1].Count > ind2)
                    return _grid[ind1][ind2];
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
            return _grid[i][j];
        }
    }

    public bool IsPositionInGrid(Vector3 position)
    {
        position -= _gridOrigin;
        int i = Mathf.RoundToInt(position.x / _cellSize.x);
        int j = Mathf.RoundToInt(position.z / _cellSize.z);
        if (i >= 0 && j >= 0)
            if (_grid.Count > i)
                if (_grid[i].Count > j)
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
}
