using NaughtyAttributes;
using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
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

    public bool ShowDoneRooms = true;

    public Vector2 DoorRandomRange = new Vector2(1,7);
    public int DoorSize = 2;

    public float DungeonDrawHeight;

    public float DeletePercent;
    public bool DeleteSmallestRoom;

    public int seed;
    public bool useRandomSeed;

    List<RectInt> SavedDoors = new();

    private SearchDungeon searchDungeon;
    private RectInt SavedDoor;
    private RectInt SavedRoom;
    
    [HideInInspector] public int AmountOfRoomsToDelete;

    private Cam cameraScript;

    public void Start()
    {
        cameraScript = GetComponent<Cam>();

        searchDungeon = GetComponent<SearchDungeon>();
        searchDungeon.OnEndGeneration += searchDungeon_OnEndSearch;
    }

    private void searchDungeon_OnEndSearch()
    {
        Debug.Log("need to delete " + AmountOfRoomsToDelete + " Rooms");

        if (AmountOfRoomsToDelete > 0)
        { 
            DeleteRoom();
        }
        else
        {
            DeleteDoor();
        }
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
        cameraScript.UpdateCam();

        if (useRandomSeed)
        {
            seed = System.DateTime.Now.GetHashCode();
            Debug.Log("Random seed used = " + seed);
        }
        else
        {
            Debug.Log("Seed from inspector used = " + seed);
        }

        Random.InitState(seed);
        //Debug.Log("Current Seed in use = " + seed);

        ToDoRooms.Clear();
        DoneRooms.Clear();
        Overlaps.Clear();
        Doors.Clear();

        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();
        audioSource.Play();

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

        AmountOfRoomsToDelete = (int) (DoneRooms.Count * DeletePercent / 100);
        Debug.Log("need to delete " + AmountOfRoomsToDelete + " Rooms");

        ///testing purposes
        MakeRandomizedDoorList();

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

    List<RectInt> RandomDoors;
    int currentDeleteIndex = 0;
    void MakeRandomizedDoorList()
    {
        RandomDoors = new(Doors);

        for (int i = 0; i < RandomDoors.Count; i++)
        {
            RectInt temp = RandomDoors[i];
            int RI = Random.Range(i, RandomDoors.Count);
            RandomDoors[i] = RandomDoors[RI];
            RandomDoors[RI] = temp;
        }

        currentDeleteIndex = 0;
    }
    void DeleteDoor()
    {
        //int randomIndex = Random.Range(0, Doors.Count);
        //SavedDoor = Doors[randomIndex];

        SavedDoor = RandomDoors[currentDeleteIndex];
        Doors.Remove(SavedDoor);

        currentDeleteIndex++;

        DispatchOnEndGenerationEvent();
    }

    public void AddDoor()
    {
        Doors.Add(SavedDoor);
        DispatchOnEndGenerationEvent();
    }

    void DeleteRoom()
    {
        if (!DeleteSmallestRoom)
        {
            SavedRoom = DoneRooms[Random.Range(0, DoneRooms.Count)];
        }
        else
        {
            SavedDoors.Clear();
            RectInt smallestRoom = DoneRooms[0];

            foreach (var room in DoneRooms)
            {
                int smallestRoomSize = smallestRoom.width * smallestRoom.height;
                int currentRoomSize = room.width * room.height;

                //Debug.Log($"CurrentRoom: {room} Size: {currentRoomSize}");
                //Debug.Log($"SmallestRoom: {smallestRoom} Size: {smallestRoomSize}");

                if (currentRoomSize < smallestRoomSize)
                {
                    smallestRoom = room;
                    //Debug.Log($"Room: {room} Size: {currentRoomSize}");
                }
            }

            SavedRoom = smallestRoom;
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (SavedRoom.Overlaps(Doors[i]))
            {
                Debug.Log("door " + i + " was added " + Doors[i].ToString());
                SavedDoors.Add(Doors[i]);
            }
        }

        DoneRooms.Remove(SavedRoom);

        foreach (var door in SavedDoors)
        {
            Doors.Remove(door);
        }

        AmountOfRoomsToDelete--;

        DispatchOnEndGenerationEvent();
    }

    public void AddRoom()
    {
        DoneRooms.Add(SavedRoom);
        
        foreach(var door in SavedDoors)
        {
            Doors.Add(door);
        }

        SavedDoors.Clear();

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
