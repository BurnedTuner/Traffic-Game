using System;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour, ISaveLoadDependant
{
    public AgentManager agentManager;
    public int StepsTakenTotal;
    public int TotalIdle;
    public int TotalBlocked;
    public Dictionary<Vector3, int> StepsPerAgent = new Dictionary<Vector3, int>();
    public Dictionary<Vector3, int> BlockedPerAgent = new Dictionary<Vector3, int>();
    public Dictionary<Vector3, int> IdlePerAgent = new Dictionary<Vector3, int>();
    public bool CollectStats;

    public void LoadData(StateData stateData)
    {
        StepsPerAgent = new Dictionary<Vector3, int>();
        BlockedPerAgent = new Dictionary<Vector3, int>();
        IdlePerAgent = new Dictionary<Vector3, int>();
        foreach(Agent agent in agentManager.Agents.Keys)
        {
            StepsPerAgent.Add(agent.transform.position, 0);
            BlockedPerAgent.Add(agent.transform.position, 0);
            IdlePerAgent.Add(agent.transform.position, 0);
        }
        agentManager.StepTaken += OnStepTaken;
        agentManager.Blocked += OnBlocked;
        agentManager.Idle += OnIdle;
    }

    private void OnIdle(Vector3 position)
    {
        if (!CollectStats)
            return;
        IdlePerAgent[position]++;
        TotalIdle++;
    }

    private void OnBlocked(Vector3 position)
    {
        if (!CollectStats)
            return;
        BlockedPerAgent[position]++;
        TotalBlocked++;
    }

    private void OnStepTaken(Vector3 position)
    {
        if (!CollectStats)
            return;
        StepsPerAgent[position]++;
        StepsTakenTotal++;
    }

    public void SaveData(ref StateData stateData)
    {
        stateData.StepsTotal = StepsTakenTotal;
        stateData.TotalIdle = TotalIdle;
        stateData.TotalBlocked = TotalBlocked;
        stateData.StepsPerAgent = new List<int>();
        stateData.StepsPerAgent.AddRange(StepsPerAgent.Values);
        stateData.IdlePerAgent = new List<int>();
        stateData.IdlePerAgent.AddRange(IdlePerAgent.Values);
        stateData.BlockedPerAgent = new List<int>();
        stateData.BlockedPerAgent.AddRange(BlockedPerAgent.Values);
    }
}
