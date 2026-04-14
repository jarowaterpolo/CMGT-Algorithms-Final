using UnityEngine;
public class TileMapGenerator : Generator
{
    private NewDungeonGenerator dungeonGen;
    private DeleteDoors deleteDoors;

    private int[,] tileMap;
    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        deleteDoors = GetComponent<DeleteDoors>();

        deleteDoors.OnEndGeneration += deleteDoors_OnEndGeneration;
    }

    private void deleteDoors_OnEndGeneration()
    {
        GenerateTileMap();
    }

    public void GenerateTileMap()
    {
        DispatchOnStartGenerationEvent();

        int[,] tilemap = new int[dungeonGen.startRoom.height, dungeonGen.startRoom.width];
        int rows = tilemap.GetLength(0);
        int cols = tilemap.GetLength(1);

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

        DispatchOnEndGenerationEvent();
    }

    public int[,] GetTileMap()
    {
        return tileMap;
    }
}
