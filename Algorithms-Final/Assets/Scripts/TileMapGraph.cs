using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGraph : Generator
{
    private NewDungeonGenerator dungeonGen;
    private TileMapGenerator tileMapGen;
    private AddFloors addFloors;

    private List<Vector3> nodePositions = new();
    private Graph<Vector3> graphNodes = new();

    private Vector2Int[] directions =
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
        Draw();
    }

    private void AddFloors_OnEndGeneration()
    {
        StartCoroutine(FloodFill());
        AddGraphNodes();
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
            Debug.Log("tile map graphnode added");

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!DiscoveredNodes.Contains(neighbor) && tileMap[neighbor.y, neighbor.x] != 1)
                {
                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue(neighbor);
                }
            }

            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();

        foreach (var dir in directions)
        {
            neighbors.Add(pos + dir);
        }
        return neighbors;
    }

    private void AddGraphNodes()
    {
        foreach(var node in nodePositions)
        {
            graphNodes.AddNode(node);
        }
    }

    private void Draw()
    {
        foreach (var node in nodePositions)
        {
            DebugExtension.DebugPoint(node, Color.red, 1);
        }
    }
}
