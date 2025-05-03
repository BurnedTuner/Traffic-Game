using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour, ISaveLoadDependant
{
    [Header("Task To Add")]
    public Task TaskToAdd;

    public List<Task> AllTasks;
    public List<Task> AvailibleTasks;
    public List<Agent> AllAgents;
    public List<Agent> AvailibleAgents;


    [ContextMenu("Add Task")]
    private void AddTask()
    {
        AllTasks.Add(TaskToAdd);
        TaskToAdd = new Task();
    }

    private void Update()
    {
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

        if(AvailibleTasks.Count > 0)
        {
            for(int i = 0; i < AvailibleTasks.Count; i++)
                for (int j = 0; j < AvailibleAgents.Count; j++)
                {
                    List<Cell> path;
                    if(AStar.FindTwoPointPath(AvailibleAgents[j].CurrentCell, AvailibleTasks[i].PickUpNode, AvailibleTasks[i].DeliveryNode, out path, AvailibleAgents[j]))
                    {
                        AvailibleAgents[j].AssignedTask = AvailibleTasks[i];
                        AvailibleAgents[j].Path = path;
                        AvailibleTasks[i].AssignedAgent = AvailibleAgents[j];
                        AvailibleTasks.Remove(AvailibleTasks[i]);
                        AvailibleAgents.Remove(AvailibleAgents[j]);
                        i--;
                        break;
                    } 
                }

            DrawPath();
        }

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
        Debug.Log("s");
        AllAgents = new List<Agent>();
        AllAgents.AddRange(FindObjectsByType<Agent>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID));
    }

    public void SaveData(ref StateData stateData)
    {
        
    }
}
