using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateData
{
    [SerializeField] private string _stateName;
    public Vector3 GridOrigin;
    public Vector2Int GridSize;
    public Vector3 CellSize;

    public StateData()
    {
        _stateName = "NewState";
        GridOrigin = Vector3.zero;
        GridSize = new Vector2Int(5, 5);
        CellSize = new Vector3(1,1,1);
    }

    public StateData(string stateName)
    {
        _stateName = stateName;
        GridOrigin = Vector3.zero;
        GridSize = new Vector2Int(5, 5);
        CellSize = new Vector3(1, 1, 1);
    }
}
