using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureBuilder : MonoBehaviour
{
    [SerializeField] private CellGrid _grid;


    private Dictionary<Cell, StructureModel> _tempStructures = new Dictionary<Cell, StructureModel>();
    private Dictionary<Cell, StructureModel> _structureDictionary = new Dictionary<Cell, StructureModel>();

    public bool IsPositionInGrid(Vector3 position) => _grid.IsPositionInGrid(position);
    public Cell GetCellByPosition(Vector3 position) => _grid.GetCellByPosition(position);


    public bool IsPositionFree(Vector3 position)
    {
        if (!_grid.IsPositionInGrid(position))
            throw new IndexOutOfRangeException("Cell " + position + " is not on grid!");

        return _grid.IsPositionOfType(position, CellType.Empty);
    }


    internal void RemoveAllTemporaryStructures()
    {
        foreach (var tempStruct in _tempStructures.Values)
        {
            var position = Vector3Int.RoundToInt(tempStruct.transform.position);
            _grid.GetCellByPosition(position).Type = CellType.Empty;
            Destroy(tempStruct.gameObject);
        }

        _tempStructures.Clear();
        //Debug.Log("Removed Temp Sructures");
    }

    internal void ConfirmBuilding()
    {
        foreach (var tempStruct in _tempStructures)
        {
            _structureDictionary.Add(tempStruct.Key, tempStruct.Value);
        }

        _tempStructures.Clear();
        //Debug.Log("Confirming Building");
    }

    public void PlaceTemporaryStructure(Vector3 position, GameObject structurePrefab, CellType cellType)
    {
        if (!_grid.IsPositionInGrid(position))
            throw new IndexOutOfRangeException("Cell " + position + " is not on grid!");

        Cell tempStructCell = _grid.GetCellByPosition(position);
        tempStructCell.Type = cellType;
        StructureModel structureModel = CreateANewStructureModel(position, structurePrefab, cellType);

        _tempStructures.Add(tempStructCell, structureModel);
    }

    private StructureModel CreateANewStructureModel(Vector3 position, GameObject structurePrefab, CellType cellType)
    {
        GameObject structure = new GameObject(cellType.ToString());
        structure.transform.SetParent(transform); //???
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }


    public void ModifyStructureModel(Cell modifiedCell, GameObject newModel, Quaternion rotation)
    {
        if (_tempStructures.ContainsKey(modifiedCell))
            _tempStructures[modifiedCell].SwapModel(newModel, rotation);
        else if (_structureDictionary.ContainsKey(modifiedCell))
            _structureDictionary[modifiedCell].SwapModel(newModel, rotation);
    }
}
