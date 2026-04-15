using System;
using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(FloodFill());
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

            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
        }

        DispatchOnEndGenerationEvent();
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
}
