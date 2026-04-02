using NaughtyAttributes;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SearchDungeon : Generator
{
    [HideInInspector]
    public Graph<Vector3> RoomGraph;

    private NewDungeonGenerator DungeonGen;
    private GraphGenerator GraphGen;

    public SearchAlgorithms<Vector3> searchAlgorithm;

    public int RemoveDoorRepeatCount = 10;

    public AudioSource CompleteSound;
    private bool Complete;

    private int j;


    void Start()
    {
        GraphGen = GetComponent<GraphGenerator>();
        DungeonGen = GetComponent<NewDungeonGenerator>();

        RoomGraph = GraphGen.RoomGraph;

        searchAlgorithm = new();

        DungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        GraphGen.OnEndGeneration += GraphGen_OnEndGeneration;
    }
    private void GraphGen_OnEndGeneration()
    {
        if (Complete) return;
        if (j < 1)
        {
            audioSource.Play();
        }
        StartCoroutine(SearchDungeonGraph());
    }
    private void DungeonGen_OnStartGeneration()
    {
        Complete = false;
        j = 0;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SearchDungeonGraph()
    {
        UnityEngine.Random.InitState(DungeonGen.seed);
        //Debug.Log("Current Seed in use = " + DungeonGen.seed);

        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();
        j++;

        if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);

        RoomGraph = GraphGen.RoomGraph;
        var FirstRoom = RoomGraph.GetFirstKey();

        Action<Vector3> DrawCircleNode = node => DebugExtension.DebugCircle(node + new Vector3(0, 0, 0), colors[5], 1, 3);

        var (allRoomsReachable, hasLoop) = searchAlgorithm.BFS(RoomGraph, FirstRoom, DrawCircleNode);

        if (DungeonGen.AmountOfRoomsToDelete > 0)
        {
            if (allRoomsReachable)
            {
                DispatchOnEndGenerationEvent();
            }
            else
            {
                DungeonGen.AddRoom();
            }
        }
        else
        {
            Debug.Log("all rooms that needed to be deleted are gone");

            if (hasLoop == true && allRoomsReachable)
            {
                Debug.Log("dungeon still has a loop");
                DispatchOnEndGenerationEvent();
            }
            else if (hasLoop != true && allRoomsReachable != true)
            {
                Debug.Log("dungeon door added");
                DungeonGen.AddDoor();
            }
            else if (allRoomsReachable == true && hasLoop != true)
            {
                Debug.Log("dungeon gen done");
            }
            else
            {
                Debug.Log("dungeon door added");
                DungeonGen.AddDoor();
            }

            Debug.Log(allRoomsReachable + "= room reachable & " + hasLoop + " = has loop ");
        }
    }
}
