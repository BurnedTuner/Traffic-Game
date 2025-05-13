using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageBuilder : MonoBehaviour
{
    [SerializeField] private StructureBuilder _builder;
    [SerializeField] private List<Cell> _neighbouringCells = new List<Cell>();
    [SerializeField] private GroundPointer _pointer;
    [SerializeField] private RoadConnector _connector;
    [SerializeField] private GameObject _storagePrefab;


    private void Start()
    {
        _pointer.GroundAltClicked += OnGroundClicked;
        _pointer.OnRemoveClicked += OnRemoveClicked;
    }

    private void OnDestroy()
    {
        _pointer.GroundAltClicked -= OnGroundClicked;
        _pointer.OnRemoveClicked -= OnRemoveClicked;
    }

    private void OnGroundClicked(Vector3 position) => PlaceStorage(position);

    private void OnRemoveClicked(Vector3 position) => RemoveStruct(position);

    private void RemoveStruct(Vector3 position)
    {
        if (!_builder.IsPositionInGrid(position))
            return;
        if (_builder.IsPositionFree(position))
            return;

        Cell cellAtPosition = _builder.GetCellByPosition(position);

        _neighbouringCells.Clear();
        _builder.RemoveStructure(cellAtPosition);

        ConnectRoads(cellAtPosition);
    }

    public void PlaceStorage(Vector3 position)
    {
        if (!_builder.IsPositionInGrid(position))
            return;
        if (!_builder.IsPositionFree(position))
            return;

        Cell cellAtPosition = _builder.GetCellByPosition(position);

        _neighbouringCells.Clear();
        _builder.PlaceTemporaryStructure(cellAtPosition, _storagePrefab, CellType.Storage);

        ConnectRoads(cellAtPosition);
        _builder.ConfirmBuilding();
    }

    private void ConnectRoads(Cell cell)
    {
        _neighbouringCells.AddRange(_builder.GetCellByPosition(cell.transform.position).ConnectedCells);

        foreach (Cell neighbour in _neighbouringCells)
            _connector.ConnectRoadAtCell(neighbour);
    }
}
