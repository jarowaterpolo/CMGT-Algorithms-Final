using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchDungeon : Generator
{
    private NewDungeonGenerator dungeonGen;
    private GraphGenerator graphGen;

    private SearchAlgorithms<Vector3> searchAlgorithm;

    private bool Complete;
    private int j;

    [HideInInspector]
    public Graph<Vector3> roomGraph;

    [HideInInspector]
    public bool allRoomsReachable;
    public Dictionary<Vector3, List<Vector3>> Adjacents;



    void Start()
    {
        graphGen = GetComponent<GraphGenerator>();
        dungeonGen = GetComponent<NewDungeonGenerator>();

        roomGraph = graphGen.roomGraph;

        searchAlgorithm = new();

        dungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        graphGen.OnEndGeneration += GraphGen_OnEndGeneration;
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
        UnityEngine.Random.InitState(dungeonGen.Seed);

        DispatchOnStartGenerationEvent();
        j++;

        yield return Search();

        yield return null;

        DispatchOnEndGenerationEvent();
    }

    public IEnumerator Search()
    {
        roomGraph = graphGen.roomGraph;
        var FirstRoom = roomGraph.GetFirstKey();

        Action<Vector3> DrawCircleNode = node => DebugExtension.DebugCircle(node /*+ new Vector3(0, 0, 0)*/, colors[5], 1, 3);

        (allRoomsReachable, Adjacents) = searchAlgorithm.BFS_DungeonGeneration(roomGraph, FirstRoom, DrawCircleNode);

        if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
    }
}
