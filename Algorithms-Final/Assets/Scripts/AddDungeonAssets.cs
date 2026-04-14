using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDungeonAssets : Generator
{
    private NewDungeonGenerator dungeonGen;
    private GraphGenerator graphGen;
    private DeleteDoors deleteDoors;

    [SerializeField]
    private GameObject Wall_Prefab;

    [SerializeField]
    private GameObject Floor_Prefab;

    [SerializeField]
    private Transform parent;

    private RectInt SavedRoom;

    private Vector3 Offset = new(.5f, 0, .5f);
    private Vector3 DoorOffset = new();

    HashSet<Vector3> Wall_Positions = new();
    HashSet<Vector3> Floor_Positions = new();

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

        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        SavedRoom = RectInt.zero;
        Wall_Positions.Clear();
        Floor_Positions.Clear();
    }

    private void deleteDoors_OnEndGeneration()
    {
        StartCoroutine(SpawnDungeonAssets());
    }

    //[Button]
    public IEnumerator SpawnDungeonAssets()
    {
        DispatchOnStartGenerationEvent();

        foreach (Transform child in parent.transform)
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
        SavedRoom = RectInt.zero;

        foreach (var door in dungeonGen.doors)
        {
            if (dungeonGen.doorSize > 1)
            {
                var doorSize = dungeonGen.doorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        DoorOffset = new(i, 0, 0);
                    }
                    else
                    {
                        DoorOffset = new(0, 0, i);
                    }
                    Wall_Positions.Add(new Vector3(door.x + .5f, 0, door.y + .5f) + DoorOffset);
                }
            }
            else
            {
                Wall_Positions.Add(new(door.x + .5f, 0, door.y + .5f));
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
            Vector3 SpawnPos = new Vector3(i, 0, room.yMin) + Offset;

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
            Vector3 SpawnPos = new Vector3(room.xMin, 0, i) + Offset;

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
        if (Wall_Positions.Contains(SpawnPos)) return null;

        GameObject Wall = Instantiate(Wall_Prefab, SpawnPos, Quaternion.identity, parent);
        Wall_Positions.Add(Wall.transform.position);

        return Wall;
    }
    private IEnumerator SpawnFloorsForRooms()
    {
        SavedRoom = RectInt.zero;

        for (int i = 0; i <  dungeonGen.doneRooms.Count; i++)
        {
            var room = dungeonGen.doneRooms[i];
            //SpawnFloorForRooms(room);
            yield return SpawnFloorForRooms(room);
        }

        foreach (var door in dungeonGen.doors)
        {
            Vector3 SpawnPos = new();
            if (dungeonGen.doorSize > 1)
            {
                var doorSize = dungeonGen.doorSize;

                for (int i = 0; i < doorSize; i++)
                {
                    Debug.Log("doorsize = " + i);
                    if (door.width > door.height)
                    {
                        DoorOffset = new(i, 0, 0);
                    }
                    else
                    {
                        DoorOffset = new(0, 0, i);
                    }
                    SpawnPos = new Vector3(door.x + .5f, 0, door.y + .5f) + DoorOffset;
                    GameObject FloorPiece = AddFloor(SpawnPos);
                    if (FloorPiece != null)
                    {
                        FloorPiece.name = "Floor_Piece_Door";
                    }
                    if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                }
            }
            else
            {
                SpawnPos = new(door.x + .5f, 0, door.y + .5f);
                GameObject FloorPiece = AddFloor(SpawnPos);
                if (FloorPiece != null)
                {
                    FloorPiece.name = "Floor_Piece_Door";
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
                Vector3 SpawnPos = new Vector3(i, 0, j) + Offset;

                GameObject FloorPiece = AddFloor(SpawnPos);
                if (FloorPiece != null)
                {
                    FloorPiece.name = "Floor_Piece_" + i;
                }
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
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
