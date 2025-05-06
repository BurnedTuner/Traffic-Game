using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour, ISaveLoadDependant
{
    [SerializeField] private GameObject _agentPrefab;
    public Dictionary<Agent, Vector3> Agents = new Dictionary<Agent, Vector3>();

    public event Action<Vector3> StepTaken;

    [ContextMenu("Find All Agents")]
    private void FindAllAgents()
    {
        Agents = new Dictionary<Agent, Vector3>();
        foreach(Agent agent in FindObjectsByType<Agent>(FindObjectsSortMode.InstanceID))
        {
            Agents.Add(agent, agent.transform.position);
        }
    }

    private void OnStepTaken(Agent obj)
    {
        Debug.Log("sss");
        StepTaken?.Invoke(Agents[obj]);
    }

    public void LoadData(StateData stateData)
    {
        foreach (Agent agent in Agents.Keys)
            Destroy(agent.gameObject);

        Agents = new Dictionary<Agent, Vector3>();
        List<Vector3> loadedPositions = stateData.AgentPositions;
        foreach(Vector3 position in loadedPositions)
        {
            GameObject agent =  Instantiate(_agentPrefab);
            agent.transform.position = position;
            Agents.Add(agent.GetComponent<Agent>(), position);
            agent.GetComponent<Agent>().Setup();
            agent.GetComponent<Agent>().StepTaken += OnStepTaken;
        }
    }

    public void SaveData(ref StateData stateData)
    {
        FindAllAgents();
        List<Vector3> positionsToSave = new List<Vector3>();
        foreach(Agent agent in Agents.Keys)
        {
            positionsToSave.Add(agent.transform.position);
        }
        stateData.AgentPositions = positionsToSave;
    }
}
