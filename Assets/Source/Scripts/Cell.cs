using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public CellGrid ParentGrid;
    public Agent AttachedAgent;
    public int NodeCost;
    public CellType Type;
    public Dictionary<Vector3, Cell> ConnectedCells;

    public void InitializeNode()
    {
        ConnectedCells = new Dictionary<Vector3, Cell>();
        if (ParentGrid)
        {
            if (ParentGrid.GetIndexByCell(this) != Vector2Int.RoundToInt(Vector2.positiveInfinity))
            {
                int i = ParentGrid.GetIndexByCell(this).x;
                int j = ParentGrid.GetIndexByCell(this).y;
                
                //Left, Top, Right, Down
                if (ParentGrid.GetCellByIndex(i - 1, j) != null)
                    ConnectedCells.Add(Vector3.left, ParentGrid.GetCellByIndex(i - 1, j));

                if (ParentGrid.GetCellByIndex(i, j + 1) != null)
                    ConnectedCells.Add(Vector3.forward, ParentGrid.GetCellByIndex(i, j + 1));

                if (ParentGrid.GetCellByIndex(i + 1, j) != null)
                    ConnectedCells.Add(Vector3.left, ParentGrid.GetCellByIndex(i + 1, j));

                if (ParentGrid.GetCellByIndex(i, j - 1) != null)
                    ConnectedCells.Add(Vector3.back, ParentGrid.GetCellByIndex(i, j - 1));
            }
        }
    }
}

public enum CellType
{
    Empty,
    Road,
    None
}