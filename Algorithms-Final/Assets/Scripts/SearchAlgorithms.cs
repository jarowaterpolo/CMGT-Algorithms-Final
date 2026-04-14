using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorithms<T>
{
    public (bool allReachable, Dictionary<T,List<T>> Adjacents) BFS(Graph<T> graph, T StartNode, Action<T> visitAction)
    {
        Debug.Log("BFS Started:");

        Queue<(T Current, T Parent)> ToDo = new();
        HashSet<T> DiscoveredNodes = new();
        Dictionary<T, List<T>> adjacents = new();

        ToDo.Enqueue((StartNode, default));
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var (currentNode, parentNode) = ToDo.Dequeue();

            visitAction?.Invoke(currentNode);

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (T neighbor in neighbors)
            {
                if (!DiscoveredNodes.Contains(neighbor))
                {
                    if (!adjacents.ContainsKey(currentNode))
                    {
                        adjacents[currentNode] = new();
                    }

                    adjacents[currentNode].Add(neighbor);

                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue((neighbor, currentNode));
                }
            }

        }

        bool allNodesReachable = graph.GetKeyList().Count == DiscoveredNodes.Count;
        return (allNodesReachable, adjacents);
    }
}
