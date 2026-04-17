using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SearchAlgorithms<T>
{
    private HashSet<T> DiscoveredNodes = new();

    public void BFS(Graph<T> graph, T StartNode, Action<T> visitAction)
    {
        DiscoveredNodes.Clear();
        Debug.Log("BFS Started:");

        Queue<T> ToDo = new();
        Dictionary<T, List<T>> adjacents = new();

        ToDo.Enqueue(StartNode);
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var currentNode = ToDo.Dequeue();

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
                    ToDo.Enqueue(neighbor);
                }
            }

        }
    }
    public (bool allReachable, Dictionary<T,List<T>> Adjacents) BFS_DungeonGeneration(Graph<T> graph, T StartNode, Action<T> visitAction)
    {
        DiscoveredNodes.Clear();
        Debug.Log("BFS_DungeonGeneration Started:");

        Queue<T> ToDo = new();
        Dictionary<T, List<T>> adjacents = new();

        ToDo.Enqueue(StartNode);
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var currentNode = ToDo.Dequeue();

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
                    ToDo.Enqueue(neighbor);
                }
            }

        }

        bool allNodesReachable = graph.GetKeyList().Count == DiscoveredNodes.Count;
        return (allNodesReachable, adjacents);
    }

    public List<T> BFS_ShortestPathFinder(Graph<T> graph, T[] NodeRoute, Action<T> visitAction)  
    {
        DiscoveredNodes.Clear();
        Debug.Log("BFS_ShortestPathFinder Started:");

        var StartNode = NodeRoute[0];
        var EndNode = NodeRoute[1];

        Queue<T> ToDo = new();
        Dictionary<T, T> parentMap = new();
        var currentNode = StartNode;

        ToDo.Enqueue(currentNode);
        DiscoveredNodes.Add(currentNode);

        while (ToDo.Count > 0)
        {
            currentNode = ToDo.Dequeue();

            if (currentNode.Equals(NodeRoute[0]))
            {
                return ReconstructPath(parentMap, StartNode, EndNode);
            }

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (T neighbor in neighbors)
            {
                if (!DiscoveredNodes.Contains(neighbor))
                {
                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue(neighbor);
                    parentMap[neighbor] = currentNode;
                }
            }

        }

        return new List<T>(); // No path found
    }

    List<T> ReconstructPath(Dictionary<T, T> parentMap, T start, T end)
    {
        List<T> path = new List<T>();
        T currentNode = end;

        while (!currentNode.Equals(start))
        {
            path.Add(currentNode);
            currentNode = parentMap[currentNode];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }
}
