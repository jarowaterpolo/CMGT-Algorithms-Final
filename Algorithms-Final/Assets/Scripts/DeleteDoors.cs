using System.Collections;
using System.Collections.Generic;
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

        while (searchDungeon.hasLoop || !searchDungeon.allRoomsReachable)
        {
            if (searchDungeon.hasLoop == true && searchDungeon.allRoomsReachable)
            {
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                MakeRandomizedDoorList();
                DeleteDoor();
                yield return graphGen.ReBuildGraph();
                yield return searchDungeon.Search();
                StartDeleting();
            }
            else if (searchDungeon.hasLoop != true && searchDungeon.allRoomsReachable != true)
            {
                AddDoor();
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                yield return graphGen.ReBuildGraph();
                yield return searchDungeon.Search();
                StartDeleting();
            }
            else if (searchDungeon.allRoomsReachable == true && searchDungeon.hasLoop != true)
            {
                DispatchOnEndGenerationEvent();
            }
            else
            {
                AddDoor();
                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                yield return graphGen.ReBuildGraph();
                yield return searchDungeon.Search();
                StartDeleting();
            }
        }

        DispatchOnEndGenerationEvent();
    }

    void MakeRandomizedDoorList()
    {
        randomDoors = new(dungeonGen.doors);

        for (int i = 0; i < randomDoors.Count; i++)
        {
            RectInt temp = randomDoors[i];
            int RI = Random.Range(i, randomDoors.Count);
            randomDoors[i] = randomDoors[RI];
            randomDoors[RI] = temp;
        }

        currentDeleteIndex = 0;
    }
    void DeleteDoor()
    {
        //int randomIndex = Random.Range(0, doors.Count);
        //savedDoor = doors[randomIndex];

        savedDoor = randomDoors[currentDeleteIndex];
        dungeonGen.doors.Remove(savedDoor);

        currentDeleteIndex++;
    }

    public void AddDoor()
    {
        dungeonGen.doors.Add(savedDoor);
    }
}
