using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchDungeon : Generator
{
    private NewDungeonGenerator DungeonGen;
    private GraphGenerator GraphGen;

    private SearchAlgorithms<Vector3> searchAlgorithm;

    private bool Complete;
    private int j;

    [HideInInspector]
    public Graph<Vector3> RoomGraph;

    [HideInInspector]
    public bool allRoomsReachable;
    public Dictionary<Vector3, List<Vector3>> Adjacents;



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

        DispatchOnStartGenerationEvent();
        j++;

        yield return Search();

        yield return null;

        DispatchOnEndGenerationEvent();
    }

    public IEnumerator Search()
    {
        RoomGraph = GraphGen.RoomGraph;
        var FirstRoom = RoomGraph.GetFirstKey();

        Action<Vector3> DrawCircleNode = node => DebugExtension.DebugCircle(node /*+ new Vector3(0, 0, 0)*/, colors[5], 1, 3);

        (allRoomsReachable, Adjacents) = searchAlgorithm.BFS(RoomGraph, FirstRoom, DrawCircleNode);

        if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
    }
}
