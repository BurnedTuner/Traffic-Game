using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateData
{
    [SerializeField] private string _stateName;
    public Vector3 GridOrigin;
    public Vector2Int GridSize;
    public Vector3 CellSize;
    public List<Vector3> AgentPositions;
    public List<Vector3> RoadPositions;
    public List<Vector3> StoragePositions;
    public int StepsTotal;
    public Dictionary<Vector3, int> StepsPerAgent;

    public StateData()
    {
        _stateName = "NewState";
        GridOrigin = Vector3.zero;
        GridSize = new Vector2Int(5, 5);
        CellSize = new Vector3(1,1,1);
        AgentPositions = new List<Vector3>();
        RoadPositions = new List<Vector3>();
        StoragePositions = new List<Vector3>();
        StepsTotal = 0;
        StepsPerAgent = new Dictionary<Vector3, int>();
    }

    public StateData(string stateName)
    {
        _stateName = stateName;
        GridOrigin = Vector3.zero;
        GridSize = new Vector2Int(5, 5);
        CellSize = new Vector3(1, 1, 1);
        AgentPositions = new List<Vector3>();
        RoadPositions = new List<Vector3>();
        StoragePositions = new List<Vector3>();
        StepsTotal = 0;
        StepsPerAgent = new Dictionary<Vector3, int>();
    }
}
