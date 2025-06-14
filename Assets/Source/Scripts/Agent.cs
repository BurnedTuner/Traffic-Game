using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public CellGrid AttachedGrid;
    public Task AssignedTask;
    public Cell CurrentCell;
    public List<Cell> Path;
    public float Speed;

    public event Action<Agent> StepTaken;
    public event Action<Agent> Idle;
    public event Action<Agent> Blocked;

    public void Setup()
    {
        AttachedGrid = FindAnyObjectByType<CellGrid>();
        CurrentCell = AttachedGrid.GetCellByPosition(transform.position);
        CurrentCell.AttachedAgent = this;
    }

    [ContextMenu("Draw Path")]
    private void DrawPath()
    {
        CurrentCell = AttachedGrid.GetCellByPosition(transform.position);
        if(CurrentCell && AssignedTask.DeliveryNode && AssignedTask.PickUpNode)
        {
            List<Cell> path;
            if (AStar.FindTwoPointPath(CurrentCell, AssignedTask.PickUpNode, AssignedTask.DeliveryNode, out path, 1000, this))
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(path[i].transform.position + Vector3.up, path[i + 1].transform.position + Vector3.up, Color.red, 10);
        }
    }

    private void FixedUpdate()
    {
        if (Path.Count > 1)
        {
            DrawPath();
            if (Path[1].AttachedAgent != this && Path[1].AttachedAgent != null)
            {
                if (Path[1].AttachedAgent.AssignedTask == null)
                    Path[1].AttachedAgent.TryFreeCurrentCell();
                else
                if (!TryUpdatePath())
                {
                    Blocked?.Invoke(this);
                    return;
                }
            }

            if ((Path[1].transform.position - Path[0].transform.position) != Vector3.zero)
                transform.position += (Path[1].transform.position - Path[0].transform.position).normalized * Speed;

            Vector3 distance = transform.position - Path[1].transform.position;
            distance.y = 0;

            if (distance.magnitude < 0.5f)
            {
                StepTaken?.Invoke(this);
                transform.position = new Vector3(Path[1].transform.position.x, transform.position.y, Path[1].transform.position.z);
                CurrentCell.AttachedAgent = null;
                Path.RemoveAt(0);

                CurrentCell = Path[0];
                CurrentCell.AttachedAgent = this;
                if (CurrentCell == AssignedTask.PickUpNode)
                    AssignedTask.PickedUp = true;
            }
        }
        else
            Idle?.Invoke(this);

        if (Path.Count <= 1 && AssignedTask.DeliveryNode != null)
        {
            AssignedTask.Complete = true;
            AssignedTask = new Task();
            Debug.Log("Finished Task");
        }
    }

    private bool TryUpdatePath()
    {
        Debug.Log("Updating Path");
        if (CurrentCell && AssignedTask.DeliveryNode && AssignedTask.PickUpNode)
        {
            List<Cell> path;
            
            if (!AssignedTask.PickedUp)
            {
                if (AStar.FindTwoPointPath(CurrentCell, AssignedTask.PickUpNode, AssignedTask.DeliveryNode, out path, 1000, this))
                {
                    Path = path;
                    return true;
                }
            }
            else
                if (AStar.FindPath(CurrentCell, AssignedTask.DeliveryNode, out path, 1000, this))
            {
                Path = path;
                return true;
            }
        }
        Debug.Log("No Path Availible");
        return false;
    }

    public void TryFreeCurrentCell()
    {
        Debug.Log("Try Free Current Cell");
        foreach(Cell cell in CurrentCell.ConnectedCells)
        {
            if(!cell.AttachedAgent)
            {
                List<Cell> path;
                if (AStar.FindPath(CurrentCell, cell, out path, 1000, this))
                    Path = path;
            }
        }
    }
}
