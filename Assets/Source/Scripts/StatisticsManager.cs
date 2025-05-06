using System;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour, ISaveLoadDependant
{
    public AgentManager agentManager;
    public int StepsTakenTotal;
    public Dictionary<Vector3, int> StepsPerAgent;

    public void LoadData(StateData stateData)
    {
        StepsPerAgent = new Dictionary<Vector3, int>();
        foreach(Agent agent in agentManager.Agents.Keys)
        {
            StepsPerAgent.Add(agent.transform.position, 0);
        }
        agentManager.StepTaken += OnStepTaken;
    }

    private void OnStepTaken(Vector3 position)
    {
        StepsPerAgent[position]++;
        StepsTakenTotal++;
    }

    public void SaveData(ref StateData stateData)
    {
        stateData.StepsPerAgent = StepsPerAgent;
        stateData.StepsTotal = StepsTakenTotal;
    }
}
