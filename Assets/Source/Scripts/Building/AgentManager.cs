using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour, ISaveLoadDependant
{
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private List<Agent> _agents;

    public void LoadData(StateData stateData)
    {
        foreach (Agent agent in _agents)
            Destroy(agent.gameObject);

        _agents = new List<Agent>();
        List<Vector3> loadedPositions = stateData.AgentPositions;
        foreach(Vector3 position in loadedPositions)
        {
            GameObject agent =  Instantiate(_agentPrefab);
            agent.transform.position = position;
            _agents.Add(agent.GetComponent<Agent>());
            agent.GetComponent<Agent>().Setup();
        }
    }

    public void SaveData(ref StateData stateData)
    {
        List<Vector3> positionsToSave = new List<Vector3>();
        foreach(Agent agent in _agents)
        {
            positionsToSave.Add(agent.transform.position);
        }
        stateData.AgentPositions = positionsToSave;
    }
}
