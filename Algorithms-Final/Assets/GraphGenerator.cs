using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphGenerator : Generator
{
    private List<RectInt> DoneRooms = new();
    private List<RectInt> Doors = new();

    [HideInInspector]
    public Graph<Vector3> RoomGraph;
    [HideInInspector]
    public Graph<Vector3> DoorGraph;

    private NewDungeonGenerator DungeonGen;

    void Start()
    {
        DungeonGen = GetComponent<NewDungeonGenerator>();
        DoneRooms = DungeonGen.DoneRooms;
        Doors = DungeonGen.Doors;

        RoomGraph = new Graph<Vector3>();
        DoorGraph = new Graph<Vector3>();

        DungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        DungeonGen.OnEndGeneration += DungeonGen_OnEndGeneration;
    }
    void Update()
    {
        DrawAll();
    }

    private void DungeonGen_OnStartGeneration()
    {
        RoomGraph.ClearGraph();
        DoorGraph.ClearGraph();
    }

    private void DungeonGen_OnEndGeneration()
    {
        Debug.Log(" berichtje ontvangen!");
        StartCoroutine(GenerateGraph());
    }

    public void MakeGraphKeysRoom(int i)
    {
        Vector3 roomMiddle = new(DoneRooms[i].x + DoneRooms[i].width / 2, 0, DoneRooms[i].y + DoneRooms[i].height / 2);
        RoomGraph.AddNode(roomMiddle);
    }

    public void GetGraphEdgesRoom(int i)
    {
        foreach (var door in Doors)
        {
            if (AlgorithmsUtils.Intersects(DoneRooms[i], door))
            {
                var middleRoom = new Vector3(DoneRooms[i].position.x + DoneRooms[i].width / 2, 0, DoneRooms[i].position.y + DoneRooms[i].height / 2);
                Vector3 doorMiddle = new(door.x + door.width / 2f, 0, door.y + door.height / 2f);
                RoomGraph.AddEdge(middleRoom, doorMiddle);
            }
        }
    }

    public void MakeGraphKeysDoor(int i)
    {
        Vector3 doorMiddle = new(Doors[i].x + Doors[i].width / 2f, 0, Doors[i].y + Doors[i].height / 2f);
        DoorGraph.AddNode(doorMiddle);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator GenerateGraph()
    {
        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();

        RoomGraph = new();
        DoorGraph = new();

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            MakeGraphKeysRoom(i);
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            MakeGraphKeysDoor(i);
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            GetGraphEdgesRoom(i);
        }

        DispatchOnEndGenerationEvent();
    }

    void DrawAll()
    {
        if (RoomGraph.GetKeyList() != null && RoomGraph.GetKeyList().Count != 0)
        {
            foreach (var node in RoomGraph.GetKeyList())
            {
                DebugExtension.DebugWireSphere(node, colors[2]);

                foreach (var value in RoomGraph.GetNeighbors(node))
                {
                    Debug.DrawLine(node, value, colors[2]);
                }
            }
        }

        if (DoorGraph.GetKeyList() != null && DoorGraph.GetKeyList().Count != 0)
        {
            foreach (var node in DoorGraph.GetKeyList())
            {
                DebugExtension.DebugWireSphere(node, colors[2]);
            }
        }
    }
}
