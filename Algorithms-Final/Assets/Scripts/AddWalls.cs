using System;
using System.Collections;
using UnityEngine;

public class AddWalls : Generator
{
    private NewDungeonGenerator dungeonGen;
    private TileMapGenerator tileMapGen;

    [SerializeField]
    private GameObject[] wallPrefabs;

    [SerializeField]
    private Transform dungeonParent;

    private int bottomRight;
    private int topRight;
    private int topLeft;
    private int bottomLeft;

    private RectInt currentLocation;

    private int[,] tileMap;

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        tileMapGen = GetComponent<TileMapGenerator>();

        tileMapGen.OnEndGeneration += tileMapGen_OnEndGeneration;
        dungeonGen.OnStartGeneration += dungeonGen_OnStartGeneration;
    }

    private void Update()
    {
        AlgorithmsUtils.DebugRectInt(currentLocation, Color.blue);
    }

    private void dungeonGen_OnStartGeneration()
    {
        foreach (Transform child in dungeonParent)
        {
            Destroy(child.gameObject);
        }
    }
    private void tileMapGen_OnEndGeneration()
    {
        tileMap = tileMapGen.GetTileMap();
        StartCoroutine(MarchSquares());
    }

    private IEnumerator MarchSquares()
    {
        DispatchOnStartGenerationEvent();

        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);
        int index = 0;
        var value = 0;

        for (int i = 0; i < rows - 1; i++) 
        { 
            for (int j = 0; j < cols - 1; j++)
            {
                currentLocation = new(j, i, 2, 2);
                bottomRight = tileMap[i, j + 1];
                topRight = tileMap[i + 1, j + 1];
                topLeft = tileMap[i + 1, j];
                bottomLeft = tileMap[i, j];

                value = (1 * bottomRight + 2 * topRight + 4 * topLeft + 8 * bottomLeft);

                SpawnWall(value, new(i + 1, j + 1));

                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                index++;
            }
        }

        DispatchOnEndGenerationEvent();
    }

    private void SpawnWall(int value, Vector2Int pos)
    {
        var wall = wallPrefabs[value];
        if (wall == null) return;
        Instantiate(wall, new Vector3(pos.y, 0, pos.x), Quaternion.identity, dungeonParent);
    }
}
