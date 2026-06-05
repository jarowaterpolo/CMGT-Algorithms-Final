using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRooms : Generator
{
    private NewDungeonGenerator dungeonGen;
    private GraphGenerator graphGen;
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
        graphGen = GetComponent<GraphGenerator>();
        searchDungeon = GetComponent<SearchDungeon>();

        searchDungeon.OnEndGeneration += searchDungeonOnEndGeneration;
    }

    private void searchDungeonOnEndGeneration()
    {
        Debug.Log("Start deleting rooms");
        StartCoroutine(StartDeleting());
    }

    private IEnumerator StartDeleting()
    {
        checkedRooms.Clear();

        DispatchOnStartGenerationEvent();


        amountOfRoomsToDelete = (int)(dungeonGen.doneRooms.Count * deletePercent / 100);
        Debug.Log("need to delete " + amountOfRoomsToDelete + " Rooms");

        for (int i = 0; i < amountOfRoomsToDelete; i++)
        {
            yield return null;
            if (searchDungeon.allRoomsReachable)
            {
                DeleteRoom();
                Debug.Log("room deleted");
            }
            else
            {
                AddRoom();
                Debug.Log("room added");
            }

            yield return graphGen.ReBuildGraph();
            yield return searchDungeon.Search();
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
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

                if (currentRoomSize < smallestRoomSize && !checkedRooms.Contains(room))
                {
                    smallestRoom = room;
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
    }

    public void AddRoom()
    {
        dungeonGen.doneRooms.Add(savedRoom);

        foreach (var door in savedDoors)
        {
            dungeonGen.doors.Add(door);
        }

        //amountOfRoomsToDelete++;

        savedDoors.Clear();
    }
}
