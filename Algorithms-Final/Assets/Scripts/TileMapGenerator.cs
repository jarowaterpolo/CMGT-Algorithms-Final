using UnityEngine;
using System.Text;
using NaughtyAttributes;

public class TileMapGenerator : Generator
{
    private NewDungeonGenerator dungeonGen;

    private int[,] tileMap;
    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
    }

    public void GenerateTileMap()
    {
        int[,] tilemap = new int[dungeonGen.startRoom.height, dungeonGen.startRoom.width];
        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);

        for (int i = 0; i < rows; i++) 
        {
            for (int j = 0; j < cols; j++) 
            {
                tilemap[i, j] = 0;
            }
        }

        foreach (var room in dungeonGen.doneRooms)
        {
            AlgorithmsUtils.FillRectangleOutline(tilemap, room, 1);
        }

        foreach (var door in dungeonGen.doors)
        {
            AlgorithmsUtils.FillRectangle(tilemap, door, 0);
        }

        tileMap = tilemap;
    }
}
