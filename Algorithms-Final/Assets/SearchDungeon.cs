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

    private bool Complete;


    void Start()
    {
        GraphGen = GetComponent<GraphGenerator>();
        DungeonGen = GetComponent<NewDungeonGenerator>();

        RoomGraph = GraphGen.RoomGraph;

        searchAlgorithm = new();

        GraphGen.OnEndGeneration += GraphGen_OnEndGeneration;
    }

    private void GraphGen_OnEndGeneration()
    {
        if (Complete) return;
        StartCoroutine(SearchDungeonGraph());
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SearchDungeonGraph()
    {
        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();

        yield return null;
        RoomGraph = GraphGen.RoomGraph;
        var FirstRoom = RoomGraph.GetFirstKey();

        Action<Vector3> printNode = node => Debug.Log($"Visited node: {node}");
        printNode += node => Debug.DrawRay(node, Vector3.up * 10, colors[1], 100f);

        Action<Vector3> DrawCircleNode = node => DebugExtension.DebugCircle(node + new Vector3(0, 0, 0), colors[5], 1, 100);

        //searchAlgorithm.BFS(RoomGraph, FirstRoom, printNode);

        var allRoomsReachable = searchAlgorithm.BFS(RoomGraph, FirstRoom, DrawCircleNode);

        if (allRoomsReachable)
        {
            DispatchOnEndGenerationEvent();
        }
        else
        {
            Debug.Log("not all rooms are reachable");
            DungeonGen.AddDoor();
            Complete = true;
        }
    }
}
