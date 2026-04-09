using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AddDungeonAssets : Generator
{
    private NewDungeonGenerator DungeonGen;
    private GraphGenerator GraphGen;
    private SearchDungeon SearchDungeon;

    [SerializeField]
    private GameObject Wall_Prefab;

    [SerializeField]
    private GameObject Floor_Prefab;

    [SerializeField]
    private Transform parent;

    [SerializeField]
    NavMeshSurface navMesh;

    private RectInt SavedRoom;

    private Vector3 Offset = new(.5f, 0, .5f);

    HashSet<Vector3> Wall_Positions = new();
    HashSet<Vector3> Floor_Positions = new();

    private void Start()
    {
        DungeonGen = GetComponent<NewDungeonGenerator>();
        GraphGen = GetComponent<GraphGenerator>();
        SearchDungeon = GetComponent<SearchDungeon>();

        DungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        SearchDungeon.OnEndGeneration += searchDungeon_OnEndSearch;
    }

    private void DungeonGen_OnStartGeneration()
    {
        
    }

    private void searchDungeon_OnEndSearch()
    {
        SpawnDungeonAssets();
    }

    //[Button]
    public void SpawnDungeonAssets()
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        SpawnWallsForRooms();
        SpawnFloorsForRooms();
    }

    private void SpawnWallsForRooms()
    {
        SavedRoom = RectInt.zero;

        foreach (var door in DungeonGen.Doors)
        {
            if (DungeonGen.DoorSize > 1)
            {
                var doorSize = DungeonGen.DoorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    Vector3 DoorOfsset = new();
                    Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        DoorOfsset = new(i, 0, 0);
                    }
                    else
                    {
                        DoorOfsset = new(0, 0, i);
                    }
                    Wall_Positions.Add(new Vector3(door.x + .5f, 0, door.y + .5f) + DoorOfsset);
                }
            }
            else
            {
                Wall_Positions.Add(new(door.x + .5f, 0, door.y + .5f));
            }
        }

        foreach (var room in DungeonGen.DoneRooms)
        {
            SpawnWallsForRoom(room);
        }
    }

    private void SpawnWallsForRoom(RectInt room)
    {
        for (int i = room.xMin; i < room.xMax; i++)
        {
            Vector3 SpawnPos = new Vector3(i, 0, room.yMin) + Offset;

            GameObject H_Wall_Front = AddWall(SpawnPos);
            if (H_Wall_Front != null)
            {
                H_Wall_Front.name = "H_Wall_Front_" + i;
            }

            SpawnPos.z = room.yMax - .5f;

            GameObject H_Wall_Back = AddWall(SpawnPos);
            if (H_Wall_Back != null)
            {
                H_Wall_Back.name = "H_Wall_Back_" + i;
            }
        }

        for (int i = room.yMin; i < room.yMax; i++)
        {
            Vector3 SpawnPos = new Vector3(room.xMin, 0, i) + Offset;

            GameObject V_Wall_Left = AddWall(SpawnPos);
            if (V_Wall_Left != null)
            {
                V_Wall_Left.name = "V_Wall_Left_" + i;
            }

            SpawnPos.x = room.xMax - .5f;

            GameObject V_Wall_Right = AddWall(SpawnPos);
            if (V_Wall_Right != null)
            {
                V_Wall_Right.name = "V_Wall_Right_" + i;
            }
        }
    }

    private GameObject AddWall(Vector3 SpawnPos)
    {
        if (Wall_Positions.Contains(SpawnPos)) return null;

        GameObject Wall = Instantiate(Wall_Prefab, SpawnPos, Quaternion.identity, parent);
        Wall_Positions.Add(Wall.transform.position);

        return Wall;
    }
    private void SpawnFloorsForRooms()
    {
        SavedRoom = RectInt.zero;

        foreach (var room in DungeonGen.DoneRooms)
        {
            SpawnFloorForRooms(room);
        }

        foreach (var door in DungeonGen.Doors)
        {
            Vector3 SpawnPos = new();
            if (DungeonGen.DoorSize > 1)
            {
                var doorSize = DungeonGen.DoorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    Vector3 DoorOfsset = new();
                    Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        DoorOfsset = new(i, 0, 0);
                    }
                    else
                    {
                        DoorOfsset = new(0, 0, i);
                    }
                    SpawnPos = new Vector3(door.x + .5f, 0, door.y + .5f) + DoorOfsset;
                }
            }
            else
            {
                SpawnPos = new(door.x + .5f, 0, door.y + .5f);
            }



            GameObject FloorPiece = AddFloor(SpawnPos);
            if (FloorPiece != null)
            {
                FloorPiece.name = "Floor_Piece_Door";
            }
        }
    }

    private void SpawnFloorForRooms(RectInt room)
    {
        for (int i = room.xMin + 1; i < room.xMax - 1; i++)
        {
            for (int j = room.yMin + 1; j < room.yMax - 1; j++)
            {
                Vector3 SpawnPos = new Vector3(i, 0, j) + Offset;

                GameObject FloorPiece = AddFloor(SpawnPos);
                if (FloorPiece != null)
                {
                    FloorPiece.name = "Floor_Piece_" + i;
                }
            }
        }
    }

    private GameObject AddFloor(Vector3 SpawnPos)
    {
        if (Floor_Positions.Contains(SpawnPos)) return null;

        GameObject FloorPiece = Instantiate(Floor_Prefab, SpawnPos, Quaternion.Euler(90, 0, 0), parent);
        Floor_Positions.Add(FloorPiece.transform.position);

        return FloorPiece;
    }
}
