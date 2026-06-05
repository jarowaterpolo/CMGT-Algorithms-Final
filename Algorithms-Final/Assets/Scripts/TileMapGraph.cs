using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileMapGraph : Generator
{
    private NewDungeonGenerator dungeonGen;
    private TileMapGenerator tileMapGen;
    private AddFloors addFloors;

    private List<Vector3> nodePositions = new();
    [HideInInspector]
    public Graph<Vector3> graphNodes = new();

    private Vector3Int[] directions3D =
{
        new(-1,0,-1),
        new(0,0,-1),
        new(1,0,-1),
        new(-1,0,0),
        new(1,0,0),
        new(-1,0,1),
        new(0,0,1),
        new(1,0,1)
    };

    private Vector2Int[] directions2D =
{
        new(-1,-1),
        new(0,-1),
        new(1,-1),
        new(-1,0),
        new(1,0),
        new(-1,1),
        new(0,1),
        new(1,1)
    };

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        tileMapGen = GetComponent<TileMapGenerator>();
        addFloors = GetComponent<AddFloors>();

        addFloors.OnEndGeneration += AddFloors_OnEndGeneration;
    }

    private void Update()
    {
        //Draw();
    }

    private void AddFloors_OnEndGeneration()
    {
        StartCoroutine(CreateTileMapGraph());
    }

    private IEnumerator CreateTileMapGraph()
    {
        yield return FloodFill();
        yield return AddGraphNodes();
        yield return AddGraphEdges();
    }

    private IEnumerator FloodFill()
    {
        var tileMap = tileMapGen.GetTileMap();
        var StartNode = dungeonGen.doors[0].position;

        Queue<Vector2Int> ToDo = new();
        HashSet<Vector2Int> DiscoveredNodes = new();

        ToDo.Enqueue(StartNode);
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var currentNode = ToDo.Dequeue();

            nodePositions.Add(new Vector3(currentNode.x + .5f, 0, currentNode.y + .5f));
            //Debug.Log("tile map graphnode added");

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!DiscoveredNodes.Contains(neighbor) && tileMap[neighbor.y, neighbor.x] != 1)
                {
                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue(neighbor);
                }
            }

        }
            yield return null;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();

        foreach (var dir in directions2D)
        {
            neighbors.Add(pos + dir);
        }
        return neighbors;
    }

    private IEnumerator AddGraphNodes()
    {
        foreach(var node in nodePositions)
        {
            graphNodes.AddNode(node);
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
        }
    }

    private IEnumerator AddGraphEdges()
    {
        var nodeSet = new HashSet<Vector3>(graphNodes.GetKeys());

        foreach (var node in nodeSet)
        {
            var neighbors = graphNodes.GetNeighbors(node);

            foreach (var dir in directions3D)
            {
                var neighbor = node + dir;

                if (!nodeSet.Contains(neighbor) || neighbors.Contains(neighbor)) continue;

                graphNodes.AddEdge(node, node + dir);
            }
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
        }
    }

    private void Draw()
    {
        foreach (var node in graphNodes.GetKeys())
        {
            DebugExtension.DebugPoint(node, Color.red, 1);

            var edges = graphNodes.GetNeighbors(node);
            if (edges != null)
            {
                foreach (var edge in edges)
                {
                    Debug.DrawLine(node, edge, Color.cyan, 1);
                }
            }
        }
    }
}
