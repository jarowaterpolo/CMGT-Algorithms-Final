using System.Collections.Generic;
using UnityEngine;

public class DeleteDoors : Generator
{
    private NewDungeonGenerator dungeonGen;
    private SearchDungeon searchDungeon;

    private RectInt savedDoor;

    List<RectInt> randomDoors;
    int currentDeleteIndex = 0;

    private void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        searchDungeon = GetComponent<SearchDungeon>();
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

        DispatchOnEndGenerationEvent();
    }

    public void AddDoor()
    {
        dungeonGen.doors.Add(savedDoor);
        DispatchOnEndGenerationEvent();
    }
}
