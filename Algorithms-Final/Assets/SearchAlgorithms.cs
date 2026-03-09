using System.Collections.Generic;
using UnityEngine;

public class SearchAlgorithms<T>
{
    //    private NewDungeonGenerator DungeonGen;
    //    private GraphGenerator GraphGen;
    //    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //    //void Start()
    //    //{
    //    //    DungeonGen = GetComponent<NewDungeonGenerator>();
    //    //    GraphGen = GetComponent<GraphGenerator>();
    //    //}

    //    // Update is called once per frame
    //    void Update()
    //{

    //}


    //public void BFS()
    //{
    //    Debug.Log("BFS Started:");
    //    //Debug.Log("TODO: Print every node in the graph using breadth first order starting from startNode");
    //    Queue<T> Todo = new();
    //    HashSet<T> DiscoveredNodes = new();
    //    Todo.Enqueue();
    //    DiscoveredNodes.Add();

    //    while (Todo.Count > 0)
    //    {
    //        var currentNode = Todo.Dequeue();
    //        var Neighbors = GraphGen.RoomGraph.GetNeighbors(currentNode);
    //        Debug.Log($"{currentNode}: {string.Join(", ", Neighbors)}");


    //        foreach (var neighbor in Neighbors)
    //        {
    //            if (DiscoveredNodes.Contains(neighbor)) continue;
    //            Todo.Enqueue(neighbor);
    //            DiscoveredNodes.Add(neighbor);
    //        }

    //    }
    //}
}
