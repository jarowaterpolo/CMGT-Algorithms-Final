using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeleteRooms : Generator
{
    private NewDungeonGenerator dungeonGen;
    private SearchDungeon searchDungeon;

    public float deletePercent = 10;
    public bool deleteSmallestRoom = true;

    private RectInt savedDoor;
    private RectInt savedRoom;

    private List<RectInt> savedDoors = new();

    private int amountOfRoomsToDelete;

    private HashSet<RectInt> checkedRooms = new();

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        searchDungeon = GetComponent<SearchDungeon>();

        searchDungeon.OnEndGeneration += searchDungeonOnEndGeneration;
    }

    private void searchDungeonOnEndGeneration()
    {
        StartCoroutine(StartDeleting());
    }

    private IEnumerator StartDeleting()
    {
        checkedRooms.Clear();

        DispatchOnStartGenerationEvent();


        amountOfRoomsToDelete = (int)(dungeonGen.doneRooms.Count * deletePercent / 100);
        Debug.Log("need to delete " + amountOfRoomsToDelete + " Rooms");

        while (amountOfRoomsToDelete > 0)
        {
            if (searchDungeon.allRoomsReachable)
            {
                DeleteRoom();
            }
            else
            {
                AddRoom();
            }
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            //run bfs
            searchDungeon.Search();
        }

        DispatchOnEndGenerationEvent();
    }

    void DeleteRoom()
    {
        if (!deleteSmallestRoom)
        {
            savedRoom = dungeonGen.doneRooms[Random.Range(0, dungeonGen.doneRooms.Count)];
        }
        else
        {
            savedDoors.Clear();
            RectInt smallestRoom = dungeonGen.doneRooms[0];

            foreach (var room in dungeonGen.doneRooms)
            {
                int smallestRoomSize = smallestRoom.width * smallestRoom.height;
                int currentRoomSize = room.width * room.height;

                //Debug.Log($"currentRoom: {room} Size: {currentRoomSize}");
                //Debug.Log($"SmallestRoom: {smallestRoom} Size: {smallestRoomSize}");

                if (currentRoomSize < smallestRoomSize && !checkedRooms.Contains(room))
                {
                    smallestRoom = room;
                    //Debug.Log($"Room: {room} Size: {currentRoomSize}");
                }
            }

            savedRoom = smallestRoom;
            checkedRooms.Add(savedRoom);
        }

        for (int i = 0; i < dungeonGen.doors.Count; i++)
        {
            if (savedRoom.Overlaps(dungeonGen.doors[i]))
            {
                //Debug.Log("door " + i + " was added " + dungeonGen.doors[i].ToString());
                savedDoors.Add(dungeonGen.doors[i]);
            }
        }

        dungeonGen.doneRooms.Remove(savedRoom);

        foreach (var door in savedDoors)
        {
            dungeonGen.doors.Remove(door);
        }

        amountOfRoomsToDelete--;

        DispatchOnEndGenerationEvent();
    }

    public void AddRoom()
    {
        dungeonGen.doneRooms.Add(savedRoom);

        foreach (var door in savedDoors)
        {
            dungeonGen.doors.Add(door);
        }

        amountOfRoomsToDelete++;

        savedDoors.Clear();

        DispatchOnEndGenerationEvent();
    }
}
