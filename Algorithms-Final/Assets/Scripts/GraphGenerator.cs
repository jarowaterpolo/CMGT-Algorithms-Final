using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : Generator
{
    private List<RectInt> doneRooms = new();
    private List<RectInt> Doors = new();

    [HideInInspector]
    public Graph<Vector3> roomGraph;
    [HideInInspector]
    public Graph<Vector3> doorGraph;

    private NewDungeonGenerator dungeonGen;

    private int j;

    void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        doneRooms = dungeonGen.doneRooms;
        Doors = dungeonGen.doors;

        roomGraph = new Graph<Vector3>();
        doorGraph = new Graph<Vector3>();

        dungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        dungeonGen.OnEndGeneration += DungeonGen_OnEndGeneration;
    }
    void Update()
    {
        DrawAll();
    }

    private void DungeonGen_OnStartGeneration()
    {
        roomGraph.ClearGraph();
        doorGraph.ClearGraph();
        j = 0;
    }

    private void DungeonGen_OnEndGeneration()
    {
        StartCoroutine(GenerateGraph());
        if (j < 1)
        {
            audioSource.Play();
        }
        j++;
    }

    public void MakeGraphKeysRoom(int i)
    {
        Vector3 roomMiddle = new(doneRooms[i].x + doneRooms[i].width / 2, 0, doneRooms[i].y + doneRooms[i].height / 2);
        roomGraph.AddNode(roomMiddle);
    }

    public void GetGraphEdgesRoom(int i)
    {
        foreach (var door in Doors)
        {
            if (AlgorithmsUtils.Intersects(doneRooms[i], door))
            {
                var middleRoom = new Vector3(doneRooms[i].position.x + doneRooms[i].width / 2, 0, doneRooms[i].position.y + doneRooms[i].height / 2);
                Vector3 doorMiddle = new(door.x + door.width / 2f, 0, door.y + door.height / 2f);
                roomGraph.AddEdge(middleRoom, doorMiddle);
            }
        }
    }

    public void MakeGraphKeysDoor(int i)
    {
        Vector3 doorMiddle = new(Doors[i].x + Doors[i].width / 2f, 0, Doors[i].y + Doors[i].height / 2f);
        doorGraph.AddNode(doorMiddle);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator GenerateGraph()
    {
        Random.InitState(dungeonGen.Seed);
        //Debug.Log("Current Seed in use = " + dungeonGen.Seed);

        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();

        yield return BuildGraph();

        DispatchOnEndGenerationEvent();
        yield return null;
    }

    IEnumerator BuildGraph()
    {
        //Debug.Log("building Graph");
        roomGraph = new();
        doorGraph = new();

        for (int i = 0; i < doneRooms.Count; i++)
        {
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
            MakeGraphKeysRoom(i);
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
            MakeGraphKeysDoor(i);
        }

        for (int i = 0; i < doneRooms.Count; i++)
        {
            if (waitingType != WaitingType.Instant) yield return CustomWait(waitingType, splitDelay);
            GetGraphEdgesRoom(i);
        }

        yield return null;
    }

    public IEnumerator ReBuildGraph()
    {
        yield return BuildGraph();
    }

    void DrawAll()
    {
        if (roomGraph != null)
        {
            if (roomGraph.GetKeyList() != null && roomGraph.GetKeyList().Count != 0)
            {
                foreach (var node in roomGraph.GetKeyList())
                {
                    DebugExtension.DebugWireSphere(node, colors[2]);

                    foreach (var value in roomGraph.GetNeighbors(node))
                    {
                        Debug.DrawLine(node, value, colors[2]);
                    }
                }
            }
        }

        if (doorGraph.GetKeyList() != null && doorGraph.GetKeyList().Count != 0)
        {
            foreach (var node in doorGraph.GetKeyList())
            {
                DebugExtension.DebugWireSphere(node, colors[2]);
            }
        }
    }
}
