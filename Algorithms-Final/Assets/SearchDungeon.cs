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


    void Start()
    {
        DungeonGen = GetComponent<NewDungeonGenerator>();
        GraphGen = GetComponent<GraphGenerator>();

        RoomGraph = DungeonGen.RoomGraph;

        searchAlgorithm = new();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SearchDungeonGraph()
    {
        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();

        yield return null;
        RoomGraph = DungeonGen.RoomGraph;
        var FirstRoom = RoomGraph.GetFirstKey();
        Debug.Log(FirstRoom);

        Action<Vector3> printNode = node => Console.WriteLine($"Visited node: {node}");
        //printNode += node => Debug.DrawRay(node, Vector3.up, Color.black);

        searchAlgorithm.BFS(RoomGraph, FirstRoom, printNode);

        DispatchOnEndGenerationEvent();
    }
}
