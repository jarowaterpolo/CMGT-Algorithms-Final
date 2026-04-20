using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Algorithms
{
    BFS,
    DFS,
    Dijkstra,
    AStar
}

public class PathFinder : Generator
{ 
    private TileMapGraph tileMapGraph;

    private Vector3 startNode;
    private Vector3 endNode;

    public List<Vector3> path = new List<Vector3>();
    HashSet<Vector3> discovered = new HashSet<Vector3>();

    private Graph<Vector3> graph;

    public Algorithms algorithm = Algorithms.BFS;

    void Start()
    {
        tileMapGraph = GetComponent<TileMapGraph>();
        graph = tileMapGraph.graphNodes;
    }

    private Vector3 GetClosestNodeToPosition(Vector3 position)
    {
        Vector3 closestNode = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        foreach (var node in graph.GetKeys())
        {
            float dist = (node - position).sqrMagnitude;

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestNode = node;
            }
        }

        //Find the closest node to the position

        return closestNode;
    }

    public List<Vector3> CalculatePath(Vector3 from, Vector3 to)
    {
        Vector3 playerPosition = from;

        startNode = GetClosestNodeToPosition(playerPosition);
        endNode = GetClosestNodeToPosition(to);

        List<Vector3> shortestPath = new List<Vector3>();

        switch (algorithm)
        {
            case Algorithms.BFS:
                shortestPath = BFS(startNode, endNode);
                break;
            case Algorithms.DFS:
                shortestPath = DFS(startNode, endNode);
                break;
            case Algorithms.Dijkstra:
                shortestPath = Dijkstra(startNode, endNode);
                break;
            case Algorithms.AStar:
                shortestPath = AStar(startNode, endNode);
                break;
        }

        path = shortestPath; //Used for drawing the path

        return shortestPath;
    }

    List<Vector3> BFS(Vector3 start, Vector3 end)
    {
        //Use this "discovered" list to see the nodes in the visual debugging used on OnDrawGizmos()
        discovered.Clear();

        Queue<Vector3> ToDo = new();
        Dictionary<Vector3, Vector3> parentMap = new();
        Vector3 currentNode = start;

        ToDo.Enqueue(currentNode);
        discovered.Add(currentNode);

        while (ToDo.Count > 0)
        {
            currentNode = ToDo.Dequeue();

            if (currentNode == end)
            {
                return ReconstructPath(parentMap, start, end);
            }

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (Vector3 neighbor in neighbors)
            {
                if (!discovered.Contains(neighbor))
                {
                    discovered.Add(neighbor);
                    ToDo.Enqueue(neighbor);
                    parentMap[neighbor] = currentNode;
                }
            }

        }

        return new List<Vector3>(); // No path found
    }

    List<Vector3> DFS(Vector3 start, Vector3 end)
    {
        //Use this "discovered" list to see the nodes in the visual debugging used on OnDrawGizmos()
        discovered.Clear();

        Stack<Vector3> ToDo = new();
        Dictionary<Vector3, Vector3> parentMap = new();
        Vector3 currentNode = start;

        ToDo.Push(currentNode);
        discovered.Add(currentNode);

        while (ToDo.Count > 0)
        {
            currentNode = ToDo.Pop();

            if (currentNode == end)
            {
                return ReconstructPath(parentMap, start, end);
            }

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (Vector3 neighbor in neighbors)
            {
                if (!discovered.Contains(neighbor))
                {
                    discovered.Add(neighbor);
                    ToDo.Push(neighbor);
                    parentMap[neighbor] = currentNode;
                }
            }

        }

        return new List<Vector3>(); // No path found
    }


    public List<Vector3> Dijkstra(Vector3 start, Vector3 end)
    {
        //Use this "discovered" list to see the nodes in the visual debugging used on OnDrawGizmos()
        discovered.Clear();

        Dictionary<Vector3, float> costMap = new();
        Dictionary<Vector3, Vector3> parentMap = new();
        List<(Vector3 node, float cost)> ToDo = new();

        costMap[start] = 0;
        ToDo.Add((start, 0));
        discovered.Add(start);

        while (ToDo.Count > 0)
        {
            ToDo = ToDo.OrderByDescending(node => node.cost).ToList();

            var currentNode = ToDo[ToDo.Count - 1].node;
            ToDo.RemoveAt(ToDo.Count - 1);

            if (currentNode == end)
            {
                return ReconstructPath(parentMap, start, end);
            }

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (Vector3 neighbor in neighbors)
            {
                var newCost = costMap[currentNode] + Cost(currentNode, neighbor);

                if (!costMap.ContainsKey(neighbor) || newCost < costMap[neighbor])
                {
                    discovered.Add(neighbor);

                    costMap[neighbor] = newCost;
                    parentMap[neighbor] = currentNode;
                    ToDo.Add((neighbor, newCost));
                }
            }

        }

        /* */
        return new List<Vector3>(); // No path found
    }

    List<Vector3> AStar(Vector3 start, Vector3 end)
    {
        //Use this "discovered" list to see the nodes in the visual debugging used on OnDrawGizmos()
        discovered.Clear();

        Dictionary<Vector3, float> costMap = new();
        Dictionary<Vector3, Vector3> parentMap = new();
        List<(Vector3 node, float cost)> ToDo = new();

        costMap[start] = 0;
        ToDo.Add((start, 0));
        discovered.Add(start);

        while (ToDo.Count > 0)
        {
            ToDo = ToDo.OrderByDescending(node => node.cost).ToList();

            var currentNode = ToDo[ToDo.Count - 1].node;
            ToDo.RemoveAt(ToDo.Count - 1);

            if (currentNode == end)
            {
                return ReconstructPath(parentMap, start, end);
            }

            var neighbors = graph.GetNeighbors(currentNode);

            foreach (Vector3 neighbor in neighbors)
            {
                var newCost = costMap[currentNode] + Cost(currentNode, neighbor);

                if (!costMap.ContainsKey(neighbor) || newCost < costMap[neighbor])
                {
                    discovered.Add(neighbor);

                    costMap[neighbor] = newCost;
                    parentMap[neighbor] = currentNode;
                    ToDo.Add((neighbor, newCost + Heuristic(neighbor, end)));
                }
            }

        }

        /* */
        return new List<Vector3>(); // No path found
    }

    public float Cost(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }

    public float Heuristic(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }

    List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> parentMap, Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = parentMap[currentNode];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startNode, .3f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endNode, .3f);

        if (discovered != null)
        {
            foreach (var node in discovered)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(node, .3f);
            }
        }

        if (path != null)
        {
            foreach (var node in path)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(node, .3f);
            }
        }


    }
}
