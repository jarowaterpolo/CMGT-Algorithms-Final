using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class AddFloors : Generator
{
    private NewDungeonGenerator dungeonGen;
    private TileMapGenerator tileMapGen;
    private AddWalls addWalls;

    [SerializeField]
    private GameObject[] floorPrefabs;

    [SerializeField]
    private Transform dungeonParent;

    [SerializeField] private bool UseRecursive;

    private Vector2Int[] Directions =
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

    private int[,] tileMap;

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        tileMapGen = GetComponent<TileMapGenerator>();
        addWalls = GetComponent<AddWalls>();

        dungeonGen.OnStartGeneration += dungeonGen_OnStartGeneration;
        addWalls.OnEndGeneration += addWalls_OnEndGeneration;
    }

    private void dungeonGen_OnStartGeneration()
    {
        foreach (Transform child in dungeonParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void addWalls_OnEndGeneration()
    {
        tileMap = tileMapGen.GetTileMap();

        if (UseRecursive)
        {
            RecursiveFloodFill();
        }
        else
        {
            StartCoroutine(FloodFill());
        }
    }

    private IEnumerator FloodFill()
    {
        DispatchOnStartGenerationEvent();

        var StartNode = dungeonGen.doors[0].position;

        Queue<Vector2Int> ToDo = new();
        HashSet<Vector2Int> DiscoveredNodes = new();

        ToDo.Enqueue(StartNode);
        DiscoveredNodes.Add(StartNode);

        while (ToDo.Count > 0)
        {
            var currentNode = ToDo.Dequeue();

            Instantiate(floorPrefabs[0], new Vector3(currentNode.x + .5f, 0, currentNode.y + .5f), Quaternion.Euler(90,0,0), dungeonParent);

            foreach (var neighbor in GetNeighbors(currentNode)) 
            { 
                if (!DiscoveredNodes.Contains(neighbor) && tileMap[neighbor.y, neighbor.x] != 1)
                {
                    DiscoveredNodes.Add(neighbor);
                    ToDo.Enqueue(neighbor);
                }
            }

            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
        }

        DispatchOnEndGenerationEvent();
    }

    private void RecursiveFloodFill()
    {
        DispatchOnStartGenerationEvent();

        var StartNode = dungeonGen.doors[0].position;

        HashSet<Vector2Int> DiscoveredNodes = new();
        DiscoveredNodes.Add(StartNode);
        FloodFillRecursion(DiscoveredNodes, StartNode);
        Debug.Log("Discovered nodes count = " + DiscoveredNodes.Count);
        DispatchOnEndGenerationEvent();
    }

    private void FloodFillRecursion(HashSet<Vector2Int> discovered, Vector2Int node)
    {
        Instantiate(floorPrefabs[0], new Vector3(node.x + .5f, 0, node.y + .5f), Quaternion.Euler(90, 0, 0), dungeonParent);
        foreach(var neighbor in GetNeighbors(node))
        {
            if (!discovered.Contains(neighbor) && tileMap[neighbor.y, neighbor.x] != 1)
            {
                discovered.Add(neighbor);
                FloodFillRecursion(discovered, neighbor);
            }
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new();

        foreach (var dir in Directions)
        {
            neighbors.Add(pos + dir);
        }
        return neighbors;
    }
}
