using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConnector : MonoBehaviour
{
    public GameObject DeadEnd;
    public GameObject Straight;
    public GameObject Corner;
    public GameObject ThreeWay;
    public GameObject FourWay; //Scriptable object roadType???

    [SerializeField] private StructureBuilder _builder;
    [SerializeField] private CellGrid _grid;

    public void ConnectRoadAtCell(Cell cell)
    {
        if (cell.Type != CellType.Road)
            return;

        CellType[] neighbourTypes = cell.NeighbouringTypes();
        int roadCount = 0;
        roadCount = neighbourTypes.Where(x => x == CellType.Road).Count();
        
        switch (roadCount)
        {
            case 0:
            case 1:
                CreateDeadEnd(neighbourTypes, cell);
                break;
            case 2:
                if (CreateStraightRoad(neighbourTypes, cell))
                    return;
                else
                    CreateCorner(neighbourTypes, cell);
                break;

            case 3:
                Create3Way(neighbourTypes, cell);
                break;

            case 4:
                Create4Way(cell);
                break;
        }
    }

    private void Create4Way(Cell temporaryCell)
    {
        _builder.ModifyStructureModel(temporaryCell, FourWay, Quaternion.identity);
    }

    private void Create3Way(CellType[] neighbourTypes, Cell temporaryCell)
    {
        float rotation = 0;
        for (int i = 0; i <= 2; i++)
        {
            if (neighbourTypes[i] != CellType.Road)
                break;
            rotation += 90;
        }

        _builder.ModifyStructureModel(temporaryCell, ThreeWay, Quaternion.Euler(0, rotation, 0));
    }

    private void CreateCorner(CellType[] neighbourTypes, Cell temporaryCell)
    {
        float rotation = 180;
        for (int i = 1; i <= 3; i++)
        {
            if (neighbourTypes[i] != CellType.Road && neighbourTypes[i-1] != CellType.Road)
                break;
            rotation += 90;
        }

        _builder.ModifyStructureModel(temporaryCell, Corner, Quaternion.Euler(0, rotation, 0));
    }

    private bool CreateStraightRoad(CellType[] neighbourTypes, Cell temporaryCell)
    {
        if (neighbourTypes[0] == CellType.Road && neighbourTypes[2] == CellType.Road)
        {
            _builder.ModifyStructureModel(temporaryCell, Straight, Quaternion.identity);
            return true;
        }

        if (neighbourTypes[1] == CellType.Road && neighbourTypes[3] == CellType.Road)
        {
            _builder.ModifyStructureModel(temporaryCell, Straight, Quaternion.Euler(0, 90, 0));
            return true;
        }

        return false;
    }

    private void CreateDeadEnd(CellType[] neighbourTypes, Cell temporaryCell)
    {
        float rotation = 180;
        for (int i = 0; i <= 2; i++)
        {
            if (neighbourTypes[i] == CellType.Road)
                break;
            rotation += 90;
        }

        _builder.ModifyStructureModel(temporaryCell, DeadEnd, Quaternion.Euler(0, rotation, 0));
    }
}
