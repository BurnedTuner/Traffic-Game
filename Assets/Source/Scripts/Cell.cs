using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public CellGrid ParentGrid;
    public Agent AttachedAgent;
    public int NodeCost;
    public CellType Type;
    public List<Cell> ConnectedCells;
    public Dictionary<Vector3, Cell> RelativeCells;

    public void InitializeNode()
    {
        ChangeCellType(CellType.Empty);
        RelativeCells = new Dictionary<Vector3, Cell>();
        ConnectedCells = new List<Cell>();
        if (ParentGrid)
        {
            if (ParentGrid.GetIndexByCell(this) != Vector2Int.RoundToInt(Vector2.positiveInfinity))
            {
                int i = ParentGrid.GetIndexByCell(this).x;
                int j = ParentGrid.GetIndexByCell(this).y;

                //Left, Top, Right, Down
                if (ParentGrid.GetCellByIndex(i - 1, j) != null)
                {
                    RelativeCells.Add(Vector3.left, ParentGrid.GetCellByIndex(i - 1, j));
                    ConnectedCells.Add(ParentGrid.GetCellByIndex(i - 1, j));
                }
                else
                    RelativeCells.Add(Vector3.left, null);

                if (ParentGrid.GetCellByIndex(i, j + 1) != null)
                {
                    RelativeCells.Add(Vector3.forward, ParentGrid.GetCellByIndex(i, j + 1));
                    ConnectedCells.Add(ParentGrid.GetCellByIndex(i, j + 1));
                }
                else
                    RelativeCells.Add(Vector3.forward, null);

                if (ParentGrid.GetCellByIndex(i + 1, j) != null)
                {
                    RelativeCells.Add(Vector3.right, ParentGrid.GetCellByIndex(i + 1, j));
                    ConnectedCells.Add(ParentGrid.GetCellByIndex(i + 1, j));
                }
                else
                    RelativeCells.Add(Vector3.right, null);

                if (ParentGrid.GetCellByIndex(i, j - 1) != null)
                {
                    RelativeCells.Add(Vector3.back, ParentGrid.GetCellByIndex(i, j - 1));
                    ConnectedCells.Add(ParentGrid.GetCellByIndex(i, j - 1));
                }
                else
                    RelativeCells.Add(Vector3.back, null);
            }
        }
    }

    public CellType[] NeighbouringTypes()
    {
        CellType[] result = new CellType[4];
        if (RelativeCells[Vector3.left] != null)
            result[0] = RelativeCells[Vector3.left].Type;
        else
            result[0] = CellType.None;
        
        if (RelativeCells[Vector3.forward] != null)
            result[1] = RelativeCells[Vector3.forward].Type;
        else
            result[1] = CellType.None;
        
        if (RelativeCells[Vector3.right] != null)
            result[2] = RelativeCells[Vector3.right].Type;
        else
            result[2] = CellType.None;

        if (RelativeCells[Vector3.back] != null)
            result[3] = RelativeCells[Vector3.back].Type;
        else
            result[3] = CellType.None;

        return result;
    }

    public void ChangeCellType(CellType newType)
    {
        switch(newType)
        {
            case CellType.Empty:
                NodeCost = 99;
                break;

            case CellType.Road:
                NodeCost = 1;
                break;

            case CellType.Storage:
                NodeCost = 1;
                break;

            default:
                Debug.Log("Uknown Cell Type provided");
                break;
        }
        Type = newType;
    }

    public bool IsWalkable()
    {
        return Type == CellType.Road || Type == CellType.Storage;
    }
}

public enum CellType
{
    Empty,
    Road,
    Storage,
    None
}