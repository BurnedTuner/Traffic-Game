using System.Collections.Generic;
using UnityEngine;

public class StructureDataManager : MonoBehaviour, ISaveLoadDependant
{
    [SerializeField] private CellGrid _grid;
    [SerializeField] private RoadBuilder _roadBuilder;
    [SerializeField] private StorageBuilder _storageBuilder;

    public void LoadData(StateData stateData)
    {
        foreach(Vector3 position in stateData.RoadPositions)
        {
            _roadBuilder.PlaceRoad(position);
            _roadBuilder.FinishPlacingRoad();
        }

        foreach(Vector3 position in stateData.StoragePositions)
        {
            _storageBuilder.PlaceStorage(position);
        }
    }

    public void SaveData(ref StateData stateData)
    {
        List<Vector3> roadPositions = new List<Vector3>(); 
        List<Vector3> storagePositions = new List<Vector3>(); 
        foreach(List<Cell> row in _grid.Cells)
        {
            foreach(Cell cell in row)
            {
                if (cell.Type == CellType.Road)
                    roadPositions.Add(cell.transform.position);
                if (cell.Type == CellType.Storage)
                    storagePositions.Add(cell.transform.position);
            }
        }

        stateData.RoadPositions = roadPositions;
        stateData.StoragePositions = storagePositions;
    }
}
