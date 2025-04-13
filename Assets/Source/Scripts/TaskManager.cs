using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
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
                    if(MultiLabelAStar.FindPath(AvailibleAgents[j].CurrentCell, AvailibleTasks[i].PickUpNode, AvailibleTasks[i].DeliveryNode, AvailibleAgents[j], out path))
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
            if (MultiLabelAStar.FindPath(task.PickUpNode, task.PickUpNode, task.DeliveryNode, null, out path))
                for (int i = 0; i < path.Count - 1; i++)
                    Debug.DrawLine(path[i].transform.position + Vector3.up, path[i + 1].transform.position + Vector3.up, Color.green, 10);
            else
                Debug.Log("No path availible");
        }
    }
}
