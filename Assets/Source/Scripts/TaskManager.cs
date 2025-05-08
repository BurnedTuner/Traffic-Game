using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour, ISaveLoadDependant
{
    public CellGrid _grid;
    public StatisticsManager _stats;

    [Header("Task To Add")]
    public Task TaskToAdd;

    public List<Task> AllTasks;
    public List<Task> AvailibleTasks;
    public List<Agent> AllAgents;
    public List<Agent> AvailibleAgents;

    [Header("Generating Tasks")]
    public bool GenerateTasks;
    public List<Cell> PossibleCells;
    public int AmountOfTasks;

    [ContextMenu("Add Task")]
    private void AddTask()
    {
        AllTasks.Add(TaskToAdd);
        TaskToAdd = new Task();
    }

    private void Update()
    {
        _stats.CollectStats = AllTasks.Count > 0;
        AvailibleAgents = new List<Agent>();
        AvailibleTasks = new List<Task>();
        for (int i = 0; i < AllTasks.Count; i++)
        {
            if(AllTasks[i].Complete)
            {
                AllTasks.RemoveAt(i);
                i--;
                continue;
            }

            if (AllTasks[i].AssignedAgent == null)
                AvailibleTasks.Add(AllTasks[i]);
        }

        foreach (Agent agent in AllAgents)
            if (agent.AssignedTask.PickUpNode == null)
                AvailibleAgents.Add(agent);

        for (int i = 0; i < AvailibleTasks.Count; i++)
            for (int j = 0; j < AvailibleAgents.Count; j++)
            {
                List<Cell> path;
                if (AStar.FindTwoPointPath(AvailibleAgents[j].CurrentCell, AvailibleTasks[i].PickUpNode, AvailibleTasks[i].DeliveryNode, out path, AvailibleAgents[j]))
                {
                    AllTasks.Find(x => x == AvailibleTasks[i]).AssignedAgent = AvailibleAgents[j];
                    AvailibleTasks[i].AssignedAgent = AvailibleAgents[j];
                    AllAgents.Find(x => x == AvailibleAgents[j]).AssignedTask = AvailibleTasks[i];
                    AvailibleAgents[j].Path = path;
                    AvailibleTasks.Remove(AvailibleTasks[i]);
                    AvailibleAgents.Remove(AvailibleAgents[j]);
                    Debug.Log(i);
                    i--;
                    j--;
                    break;
                }
            }
            //DrawPath();

        if(AvailibleAgents.Count > 0)
        {
            foreach(Task task in AllTasks)
            {
                foreach (Agent agent in AvailibleAgents)
                    if (task.DeliveryNode == agent.CurrentCell || task.PickUpNode == agent.CurrentCell)
                        agent.TryFreeCurrentCell();
            }
        }
    }

    private void DrawPath()
    {
        foreach(Task task in AvailibleTasks)
        {
            List<Cell> path;
            if (AStar.FindPath(task.PickUpNode, task.DeliveryNode, out path))
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(path[i].transform.position + Vector3.up, path[i + 1].transform.position + Vector3.up, Color.green, 10);
            else
                Debug.Log("No path availible");
        }
    }

    public void LoadData(StateData stateData)
    {
        AllAgents = new List<Agent>();
        AllAgents.AddRange(FindObjectsByType<Agent>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID));

        if (GenerateTasks)
        {
            AllTasks = new List<Task>();
            PossibleCells = new List<Cell>();
            foreach (Vector3 position in stateData.StoragePositions)
            {
                PossibleCells.Add(_grid.GetCellByPosition(position));
            }

            for (int i = 0; i < AmountOfTasks; i++)
            {
                TaskToAdd = new Task();
                int pickUp = Random.Range(0, PossibleCells.Count - 1);
                int delivery = Random.Range(0, PossibleCells.Count - 1);
                TaskToAdd.PickUpNode = PossibleCells[pickUp];
                TaskToAdd.DeliveryNode = PossibleCells[delivery];
                AllTasks.Add(TaskToAdd);
            }
        }
    }

    public void SaveData(ref StateData stateData)
    {
        
    }
}
