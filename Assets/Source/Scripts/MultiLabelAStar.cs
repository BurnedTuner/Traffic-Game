using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiLabelAStar
{
    private static float _maxT = float.PositiveInfinity;

    public static bool FindPath(Cell start, Cell pickup, Cell delivery, Agent agent, out List<Cell> path)
    {
        path = new List<Cell>();
        List<(Cell, NodeInfo)> queue = new List<(Cell, NodeInfo)>();
        Dictionary<Cell, NodeInfo> proccessedLabel1 = new Dictionary<Cell, NodeInfo>();
        Dictionary<Cell, NodeInfo> proccessedLabel2 = new Dictionary<Cell, NodeInfo>();
        queue.Add((start, new NodeInfo(1,0, start, pickup, delivery)));

        while (queue.Count > 0)
        {
            int lowestFIndex = FindLowestFIndex(queue);

            if (proccessedLabel1.ContainsKey(queue[lowestFIndex].Item1) && queue[lowestFIndex].Item2.Label == 1)
            {
                queue.RemoveAt(lowestFIndex);
                continue;
            }
            if (proccessedLabel2.ContainsKey(queue[lowestFIndex].Item1) && queue[lowestFIndex].Item2.Label == 2)
            {
                queue.RemoveAt(lowestFIndex);
                continue;
            }

            if (queue[lowestFIndex].Item2.Label == 1)
                proccessedLabel1.Add(queue[lowestFIndex].Item1, queue[lowestFIndex].Item2);

            if (queue[lowestFIndex].Item2.Label == 2)
                proccessedLabel2.Add(queue[lowestFIndex].Item1, queue[lowestFIndex].Item2);


            if (queue[lowestFIndex].Item2.Label == 1 && queue[lowestFIndex].Item2.Time > _maxT)
            {
                queue.RemoveAt(lowestFIndex);
                continue;
            }

            if (queue[lowestFIndex].Item2.Label == 1 && queue[lowestFIndex].Item1 == pickup)
            {
                queue[lowestFIndex].Item2.Label = 2;
                path.AddRange(ReconstructPath(start, pickup, proccessedLabel1));
                continue;
            }

            if (queue[lowestFIndex].Item2.Label == 2 && queue[lowestFIndex].Item1 == delivery)
            {
                path.Remove(pickup);
                path.AddRange(ReconstructPath(pickup, delivery, proccessedLabel2));
                return true;
            }

            foreach (Cell cell in queue[lowestFIndex].Item1.ConnectedCells)
            {
                if (cell.AttachedAgent != agent && cell.AttachedAgent != null)
                    continue;
                if (queue[lowestFIndex].Item2.Label == 2 && proccessedLabel2.ContainsKey(cell))
                    continue;
                if (queue[lowestFIndex].Item2.Label == 1 && proccessedLabel1.ContainsKey(cell))
                    continue;

                queue.Add((cell, new NodeInfo(queue[lowestFIndex].Item2.Label, queue[lowestFIndex].Item2.Time + 1, cell, pickup, delivery)));
                queue[queue.Count - 1].Item2.CameFrom = queue[lowestFIndex].Item1;
            }  
        }

        return false;
    }

    private static List<Cell> ReconstructPath(Cell start, Cell end, Dictionary<Cell, NodeInfo> proccessed)
    {
        List<Cell> result = new List<Cell>();
        Cell current = end;
        result.Add(end);
        while (current != start)
        {
            result.Add(proccessed[current].CameFrom);
            current = proccessed[current].CameFrom;
        }

        result.Reverse();

        return result;
    }

    private static int FindLowestFIndex(List<(Cell, NodeInfo)> queue)
    {
        float minPriority = float.PositiveInfinity;
        int result = 0;
        for(int i = 0; i < queue.Count; i++)
        {
            if (minPriority > queue[i].Item2.Priority)
            {
                minPriority = queue[i].Item2.Priority;
                result = i;
            }
        }

        return result;
    }

    private class NodeInfo
    {
        public int Label = 0; //label (l)
        public int Time = 0; //TimeElapsed (g)
        public float Heuristics = 0; //Heuristics (h)
        public float Priority = 0; //Priority (f)
        public Cell CameFrom;

        public NodeInfo(int l, int g, Cell owner, Cell pickup, Cell delivery)
        {
            Label = l;
            Time = g;

            if (Label == 1)
                Heuristics = Vector3.Distance(owner.transform.position, pickup.transform.position) +
                             Vector3.Distance(delivery.transform.position, pickup.transform.position);
            else if (Label == 2)
                Heuristics = Vector3.Distance(owner.transform.position, delivery.transform.position);
            Heuristics += owner.NodeCost;

            Priority = Time + Heuristics;
        }
    }
}
