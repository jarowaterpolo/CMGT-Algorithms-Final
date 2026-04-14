using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDungeonAssets : Generator
{
    private NewDungeonGenerator dungeonGen;
    private GraphGenerator graphGen;
    private DeleteDoors deleteDoors;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject floorPrefab;

    [SerializeField]
    private Transform dungeonParent;

    private RectInt savedRoom;

    private Vector3 overallOffset = new(.5f, 0, .5f);
    private Vector3 doorOffset = new();

    HashSet<Vector3> wallPositions = new();
    HashSet<Vector3> floorPositions = new();

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        graphGen = GetComponent<GraphGenerator>();
        deleteDoors = GetComponent<DeleteDoors>();

        dungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        deleteDoors.OnEndGeneration += deleteDoors_OnEndGeneration;
    }

    private void DungeonGen_OnStartGeneration()
    {
        StopAllCoroutines();

        foreach (Transform child in dungeonParent.transform)
        {
            Destroy(child.gameObject);
        }

        savedRoom = RectInt.zero;
        wallPositions.Clear();
        floorPositions.Clear();
    }

    private void deleteDoors_OnEndGeneration()
    {
        StartCoroutine(SpawnDungeonAssets());
    }

    //[Button]
    public IEnumerator SpawnDungeonAssets()
    {
        DispatchOnStartGenerationEvent();

        foreach (Transform child in dungeonParent.transform)
        {
            Destroy(child.gameObject);
        }

        yield return SpawnWallsForRooms();
        Debug.Log("Generating Rooms is done");
        //SpawnFloorsForRooms();
        yield return (SpawnFloorsForRooms());
        Debug.Log("Generating Floors is done");

        yield return null;

        DispatchOnEndGenerationEvent();
    }

    private IEnumerator SpawnWallsForRooms()
    {
        savedRoom = RectInt.zero;

        foreach (var door in dungeonGen.doors)
        {
            if (dungeonGen.doorSize > 1)
            {
                var doorSize = dungeonGen.doorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    //Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        doorOffset = new(i, 0, 0);
                    }
                    else
                    {
                        doorOffset = new(0, 0, i);
                    }
                    wallPositions.Add(new Vector3(door.x + .5f, 0, door.y + .5f) + doorOffset);
                }
            }
            else
            {
                wallPositions.Add(new(door.x + .5f, 0, door.y + .5f));
            }
        }

        foreach (var room in dungeonGen.doneRooms)
        {
            //SpawnWallsForRoom(room);
            yield return SpawnWallsForRoom(room);
        }
    }

    private IEnumerator SpawnWallsForRoom(RectInt room)
    {
        for (int i = room.xMin; i < room.xMax; i++)
        {
            Vector3 SpawnPos = new Vector3(i, 0, room.yMin) + overallOffset;

            GameObject H_Wall_Front = AddWall(SpawnPos);
            if (H_Wall_Front != null)
            {
                H_Wall_Front.name = "H_Wall_Front_" + i;
            }
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);

            SpawnPos.z = room.yMax - .5f;

            GameObject H_Wall_Back = AddWall(SpawnPos);
            if (H_Wall_Back != null)
            {
                H_Wall_Back.name = "H_Wall_Back_" + i;
            }
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
        }

        for (int i = room.yMin; i < room.yMax; i++)
        {
            Vector3 SpawnPos = new Vector3(room.xMin, 0, i) + overallOffset;

            GameObject V_Wall_Left = AddWall(SpawnPos);
            if (V_Wall_Left != null)
            {
                V_Wall_Left.name = "V_Wall_Left_" + i;
            }
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);

            SpawnPos.x = room.xMax - .5f;

            GameObject V_Wall_Right = AddWall(SpawnPos);
            if (V_Wall_Right != null)
            {
                V_Wall_Right.name = "V_Wall_Right_" + i;
            }
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
        }
    }

    private GameObject AddWall(Vector3 SpawnPos)
    {
        if (wallPositions.Contains(SpawnPos)) return null;

        GameObject Wall = Instantiate(wallPrefab, SpawnPos, Quaternion.identity, dungeonParent);
        wallPositions.Add(Wall.transform.position);

        return Wall;
    }
    private IEnumerator SpawnFloorsForRooms()
    {
        savedRoom = RectInt.zero;

        for (int i = 0; i <  dungeonGen.doneRooms.Count; i++)
        {
            var room = dungeonGen.doneRooms[i];
            //SpawnFloorForRooms(room);
            yield return SpawnFloorForRooms(room);
        }

        foreach (var door in dungeonGen.doors)
        {
            Vector3 spawnPos = new();
            if (dungeonGen.doorSize > 1)
            {
                var doorSize = dungeonGen.doorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    //Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        doorOffset = new(i, 0, 0);
                    }
                    else
                    {
                        doorOffset = new(0, 0, i);
                    }
                    spawnPos = new Vector3(door.x + .5f, 0, door.y + .5f) + doorOffset;
                    GameObject floorPiece = AddFloor(spawnPos);
                    if (floorPiece != null)
                    {
                        floorPiece.name = "Floor_Piece_Door";
                    }
                    if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                }
            }
            else
            {
                spawnPos = new(door.x + .5f, 0, door.y + .5f);
                GameObject floorPiece = AddFloor(spawnPos);
                if (floorPiece != null)
                {
                    floorPiece.name = "Floor_Piece_Door";
                }
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            }
        }
    }

    private IEnumerator SpawnFloorForRooms(RectInt room)
    {
        for (int i = room.xMin + 1; i < room.xMax - 1; i++)
        {
            for (int j = room.yMin + 1; j < room.yMax - 1; j++)
            {
                Vector3 spawnPos = new Vector3(i, 0, j) + overallOffset;

                GameObject floorPiece = AddFloor(spawnPos);
                if (floorPiece != null)
                {
                    floorPiece.name = "Floor_Piece_" + i;
                }
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            }
        }
    }

    private GameObject AddFloor(Vector3 spawnPos)
    {
        if (floorPositions.Contains(spawnPos)) return null;

        GameObject floorPiece = Instantiate(floorPrefab, spawnPos, Quaternion.Euler(90, 0, 0), dungeonParent);
        floorPositions.Add(floorPiece.transform.position);

        return floorPiece;
    }

}
