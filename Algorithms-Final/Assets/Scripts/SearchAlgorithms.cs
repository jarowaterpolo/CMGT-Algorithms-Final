using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorithms<T>
{
    public (bool allReachable, bool hasCycle) BFS(Graph<T> graph, T StartNode, Action<T> visitAction)
    {
        Debug.Log("BFS Started:");

        Queue<(T Current, T Parent)> ToDo = new();
        HashSet<T> DiscoveredNodes = new();
        bool foundCycle = false;

        ToDo.Enqueue((StartNode, default));
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var (currentNode, parentNode) = ToDo.Dequeue();

            visitAction?.Invoke(currentNode);

            foreach (T neighbor in graph.GetNeighbors(currentNode))
            {
                if (!DiscoveredNodes.Contains(neighbor))
                {
                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue((neighbor, currentNode));
                }
                else if (parentNode != null && !neighbor.Equals(parentNode)) 
                { 
                    foundCycle = true;
                }
            }

        }

        bool allNodesReachable = graph.GetKeyList().Count == DiscoveredNodes.Count;
        return (allNodesReachable, foundCycle);
    }
}
