using NaughtyAttributes;
using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class NewDungeonGenerator : Generator
{
    public List<RectInt> StartRoom;

    private List<RectInt> ToDoRooms = new();
    [HideInInspector]
    public List<RectInt> DoneRooms = new();
    private List<RectInt> Overlaps = new();
    //[HideInInspector]
    public List<RectInt> Doors = new();

    private RectInt CurrentRoom;

    public RectInt minRoomSize = new RectInt(0, 0, 8, 8);

    public bool splitHorizontally;
    public int roomIndex;

    public int N;

    public bool ShowDoneRooms = true;

    public Vector2 DoorRandomRange = new Vector2(1,7);
    public int DoorSize = 2;

    public float DungeonDrawHeight;

    private SearchDungeon searchDungeon;
    private RectInt SavedDoor;

    public void Start()
    {
        searchDungeon = GetComponent<SearchDungeon>();
        searchDungeon.OnEndGeneration += searchDungeon_OnEndSearch;
    }

    private void searchDungeon_OnEndSearch()
    {
        DeleteDoor();
    }

    void Update()
    {
        DrawAll();
    }
    public void SplitRandom()
    {
        int random;
        RectInt newRoom = ToDoRooms[roomIndex];
        ToDoRooms.Remove(ToDoRooms[roomIndex]);
        CurrentRoom = newRoom;

        if (newRoom.width <= minRoomSize.width * 2 && newRoom.height <= minRoomSize.height * 2)
        {
            DoneRooms.Add(newRoom);
        }
        else
        {
            if (newRoom.height < minRoomSize.height * 2)
            {
                splitHorizontally = true;
            }

            if (splitHorizontally == false)
            {
                random = Random.Range(minRoomSize.height, newRoom.height - minRoomSize.height);
                newRoom.height = random;

                ToDoRooms.Add(newRoom);

                newRoom.height = CurrentRoom.height - random + 1;
                newRoom.y = newRoom.y + random - 1;

                ToDoRooms.Add(newRoom);
            }
            else
            {
                random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);
                newRoom.width = random;

                ToDoRooms.Add(newRoom);

                newRoom.width = CurrentRoom.width - random + 1;
                newRoom.x = newRoom.x + random - 1;

                ToDoRooms.Add(newRoom);
                splitHorizontally = false;
            }
        }
        //CurrentRoom = new RectInt();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator GenerateDungeon()
    {
        ToDoRooms.Clear();
        DoneRooms.Clear();
        Overlaps.Clear();
        Doors.Clear();

        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();

        ToDoRooms.Add(StartRoom[0]);

        while (ToDoRooms.Count > 0)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            SplitRandom();
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            for (int j = i + 1; j < DoneRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).width < 1 && AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).height < 1) continue;

                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                GetOverlaps(i, j);
            }
        }

        for (int i = 0; i < Overlaps.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            PlaceDoors(i);
        }

        CurrentRoom = new();

        DispatchOnEndGenerationEvent();
    }
    public void GetOverlaps(int i, int j)
    {
        var OverlapedSpace = AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]);
        CurrentRoom = OverlapedSpace;

        if (OverlapedSpace.width >= 5 * DoorSize || OverlapedSpace.height >= 5 * DoorSize)
        {
            Overlaps.Add(OverlapedSpace);
        }
    }

    public void PlaceDoors(int i)
    {
        var overlap = Overlaps[i];
        float Rx = Random.Range(overlap.x + 2 * DoorSize, overlap.x + overlap.width - 2 * DoorSize);
        float Ry = Random.Range(overlap.y + 2 * DoorSize, overlap.y + overlap.height - 2 * DoorSize);

        if (overlap.height == 1)
        {
            overlap.x = (int)Rx;
            overlap.width = 1 * DoorSize;
        }
        else if (overlap.width == 1)
        {
            overlap.y = (int)Ry;
            overlap.height = 1 * DoorSize;
        }

        CurrentRoom = overlap;
        Doors.Add(overlap);
    }

    void DeleteDoor()
    {
        SavedDoor = Doors[Random.Range(0, Doors.Count)];
        Doors.Remove(SavedDoor);

        DispatchOnEndGenerationEvent();
    }

    public void AddDoor()
    {
        Doors.Add(SavedDoor);

        DispatchOnEndGenerationEvent();
    }

    void DrawAll()
    {
        if (ShowDoneRooms)
        {
            //Drawing Done Rooms
            for (int i = 0; i < DoneRooms.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(DoneRooms[i], colors[0], 0, false, DungeonDrawHeight);
            }
        }

        //Drawing Not Done Rooms
        for (int i = 0; i < ToDoRooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(ToDoRooms[i], colors[1]);
        }

        if (Overlaps.Count > 0)
        {
            //Drawing Overlaps
            for (int i = 0; i < Overlaps.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(Overlaps[i], colors[3], 0, false, DungeonDrawHeight);
            }
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(Doors[i], colors[4], 0, false, DungeonDrawHeight);
        }

        //Drawing Current Room
        AlgorithmsUtils.DebugRectInt(CurrentRoom, colors[2], 0, false, DungeonDrawHeight);
    }
}
