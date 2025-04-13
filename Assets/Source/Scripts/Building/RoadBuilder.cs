using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    [SerializeField] private StructureBuilder _builder;
    [SerializeField] private List<Cell> _temporaryCells = new List<Cell>();
    [SerializeField] private List<Cell> _neighbouringCells = new List<Cell>();
    [SerializeField] private GroundPointer _pointer;
    [SerializeField] private RoadConnector _connector;

    private Vector3 _startPosition;
    private bool _isPlacing;

    private void Start()
    {
        _pointer.GroundClicked += OnGroundClicked;
        InputInitializer.Instance.HoldCancelledInput += OnHoldCancelledInput;
    }

    private void OnDestroy()
    {
        _pointer.GroundClicked -= OnGroundClicked;
        InputInitializer.Instance.HoldCancelledInput -= OnHoldCancelledInput;
    }

    private void OnGroundClicked(Vector3 position) => PlaceRoad(position);
    private void OnHoldCancelledInput(Vector2 position) => FinishPlacingRoad();

    public void PlaceRoad(Vector3 position)
    {
        if (!_builder.IsPositionInGrid(position))
            return;
        if (!_builder.IsPositionFree(position))
            return;

        Cell cellAtPosition = _builder.GetCellByPosition(position);

        if (!_isPlacing)
        {
            _isPlacing = true;
            _startPosition = position;

            _temporaryCells.Clear();
            _neighbouringCells.Clear();
            _temporaryCells.Add(cellAtPosition);
            _builder.PlaceTemporaryStructure(cellAtPosition, _connector.DeadEnd, CellType.Road);
        }
        else
        {
            _temporaryCells.Clear();
            _builder.RemoveAllTemporaryStructures();

            foreach (Cell neighbour in _neighbouringCells)
            {
                _connector.ConnectRoadAtCell(neighbour);
            }

            _neighbouringCells.Clear();
            Cell startCell = _builder.GetCellByPosition(_startPosition);
            Cell endCell = _builder.GetCellByPosition(position);

            List<Cell> tempCells;
            if (AStar.FindPath(startCell, endCell, out tempCells))
            {
                foreach (Cell cell in tempCells)
                {
                    _temporaryCells.Add(cell);
                    if (!_builder.IsPositionFree(cell.transform.position))
                        continue;
                    _builder.PlaceTemporaryStructure(cell, _connector.DeadEnd, CellType.Road);
                }
            }
        }
        ConnectRoads();
    }

    public void FinishPlacingRoad()
    {
        _isPlacing = false;
        _builder.ConfirmBuilding();
        _startPosition = Vector3.zero;
    }

    private void ConnectRoads()
    {
        foreach (Cell cell in _temporaryCells)
        {
            _connector.ConnectRoadAtCell(cell);
            _neighbouringCells.AddRange(_builder.GetCellByPosition(cell.transform.position).ConnectedCells);

            foreach (Cell neighbour in _neighbouringCells)
                if(!_temporaryCells.Contains(neighbour))
                    _connector.ConnectRoadAtCell(neighbour);
        }
    }
}
