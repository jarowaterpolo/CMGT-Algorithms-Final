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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonGen = GetComponent<NewDungeonGenerator>();
        DoneRooms = DungeonGen.DoneRooms;
        Doors = DungeonGen.Doors;

        RoomGraph = DungeonGen.RoomGraph;
        DoorGraph = DungeonGen.DoorGraph;
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

        RoomGraph = DungeonGen.RoomGraph;
        DoorGraph = DungeonGen.DoorGraph;

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (DungeonGen.splitType != NewDungeonGenerator.SplitType.Instant) yield return DungeonGen.CustomWait();
            MakeGraphKeysRoom(i);
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (DungeonGen.splitType != NewDungeonGenerator.SplitType.Instant) yield return DungeonGen.CustomWait();
            MakeGraphKeysDoor(i);
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (DungeonGen.splitType != NewDungeonGenerator.SplitType.Instant) yield return DungeonGen.CustomWait();
            GetGraphEdgesRoom(i);
        }

        DispatchOnEndGenerationEvent();
    }
}
