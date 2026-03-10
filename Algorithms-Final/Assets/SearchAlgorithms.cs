using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorithms<T>
{
    public void BFS(Graph<T> graph, T StartNode, Action<T> visitAction)
    {
        Debug.Log("BFS Started:");

        Queue<T> ToDo = new();
        HashSet<T> DiscoveredNodes = new();

        ToDo.Enqueue(StartNode);
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            T currentNode = ToDo.Dequeue();
            visitAction(currentNode);

            foreach (T neighbor in graph.GetNeighbors(currentNode))
            {
                if (DiscoveredNodes.Contains(neighbor)) continue;
                ToDo.Enqueue(neighbor);
                DiscoveredNodes.Add(neighbor);
            }

        }
    }
}
