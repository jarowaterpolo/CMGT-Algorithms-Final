using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeleteDoors : Generator
{
    private NewDungeonGenerator dungeonGen;
    private GraphGenerator graphGen;
    private SearchDungeon searchDungeon;
    private DeleteRooms deleteRooms;

    private RectInt savedDoor;

    List<RectInt> randomDoors;
    int currentDeleteIndex = 0;

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        graphGen = GetComponent<GraphGenerator>();
        searchDungeon = GetComponent<SearchDungeon>();
        deleteRooms = GetComponent<DeleteRooms>();

        deleteRooms.OnEndGeneration += deleteRoomsOnEndGeneration;
    }

    private void deleteRoomsOnEndGeneration()
    {
        Debug.Log("Start deleting doors");
        StartCoroutine(StartDeleting());
    }

    private IEnumerator StartDeleting()
    {
        DispatchOnStartGenerationEvent();

        yield return NewDoorDeletion();

        DispatchOnEndGenerationEvent();
    }

    IEnumerator NewDoorDeletion()
    {
        foreach (var door in dungeonGen.doors.ToList())
        {
            Vector3 doorMiddle = new(door.x + door.width / 2f, 0, door.y + door.height / 2f);

            if (!searchDungeon.Adjacents.ContainsKey(doorMiddle))
            {
                //Debug.Log($"removed door {door}");
                dungeonGen.doors.Remove(door);
            }

            //if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);

        }

        yield return graphGen.ReBuildGraph();
        yield return searchDungeon.Search();
        if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
    }
}
