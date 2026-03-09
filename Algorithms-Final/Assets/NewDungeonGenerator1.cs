using NaughtyAttributes;
using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class NewDungeonGenerator : Generator
{
    public List<RectInt> StartRoom;

    private List<RectInt> ToDoRooms = new();
    private List<RectInt> DoneRooms = new();
    private List<RectInt> Overlaps = new();
    [SerializeField]
    private List<RectInt> Doors = new();

    private RectInt CurrentRoom;

    public RectInt minRoomSize = new RectInt(0, 0, 8, 8);

    public bool splitHorizontally;
    public int roomIndex;

    public int N;
    public float SplitDelay = 0.2f;

    public enum SplitType
    {
        Instant, Overtime, Space
    }

    public SplitType splitType;

    public Color[] colors = { Color.green, Color.red, Color.cyan, Color.black, new Color(255,175,0,1)};

    public bool ShowDoneRooms = true;

    public Vector2 DoorRandomRange = new Vector2(1,7);
    public int DoorSize = 2;

    public float DungeonDrawHeight;

    private Graph<Vector3> RoomGraph;
    private Graph<Vector3> DoorGraph;

    private void Start()
    {
        RoomGraph = new Graph<Vector3>();
        DoorGraph = new Graph<Vector3>();
    }

    void Update()
    {
        DrawAll();
    }
    public void SplitRandom()
    {
        int random;
        RectInt newRoom = ToDoRooms[roomIndex];
        ToDoRooms.Remove(ToDoRooms[roomIndex]);
        CurrentRoom = newRoom;

        if (newRoom.width <= minRoomSize.width * 2 && newRoom.height <= minRoomSize.height * 2)
        {
            DoneRooms.Add(newRoom);
        }
        else
        {
            if (newRoom.height < minRoomSize.height * 2)
            {
                splitHorizontally = true;
            }

            if (splitHorizontally == false)
            {
                random = Random.Range(minRoomSize.height, newRoom.height - minRoomSize.height);
                newRoom.height = random;

                ToDoRooms.Add(newRoom);

                newRoom.height = CurrentRoom.height - random + 1;
                newRoom.y = newRoom.y + random - 1;

                ToDoRooms.Add(newRoom);
            }
            else
            {
                random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);
                newRoom.width = random;

                ToDoRooms.Add(newRoom);

                newRoom.width = CurrentRoom.width - random + 1;
                newRoom.x = newRoom.x + random - 1;

                ToDoRooms.Add(newRoom);
                splitHorizontally = false;
            }
        }
        CurrentRoom = new RectInt();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator GenerateDungeon()
    {
        ToDoRooms.Clear();
        DoneRooms.Clear();
        Overlaps.Clear();
        Doors.Clear();
        RoomGraph.ClearGraph();
        DoorGraph.ClearGraph();

        //Main --- Generator script testing
        StartGenerator();

        ToDoRooms.Add(StartRoom[0]);

        while (ToDoRooms.Count > 0)
        {
            if (splitType != SplitType.Instant) yield return CustomWait();
            SplitRandom();
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            for (int j = i + 1; j < DoneRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).width < 1 && AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).height < 1) continue;

                if (splitType != SplitType.Instant) yield return CustomWait();
                GetOverlaps(i, j);
            }
        }

        for (int i = 0; i < Overlaps.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait();
            PlaceDoors(i);
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait();
            MakeGraphKeysRoom(i);
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait();
            MakeGraphKeysDoor(i);
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait();
            GetGraphEdgesRoom(i);
        }

        StopGenerating();
    }

    IEnumerator CustomWait()
    {
        switch (splitType)
        {
            case SplitType.Overtime:
                yield return new WaitForSeconds(SplitDelay);
                break;
            case SplitType.Space:
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                yield return null;
                break;
        }
    }

    public void GetOverlaps(int i, int j)
    {
        var OverlapedSpace = AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]);

        if (OverlapedSpace.width >= 5 * DoorSize || OverlapedSpace.height >= 5 * DoorSize)
        {
            Overlaps.Add(OverlapedSpace);
        }
    }

    public void PlaceDoors(int i)
    {
        var overlap = Overlaps[i];
        float Rx = Random.Range(overlap.x + 2 * DoorSize, overlap.x + overlap.width - 2 * DoorSize);
        float Ry = Random.Range(overlap.y + 2 * DoorSize, overlap.y + overlap.height - 2 * DoorSize);

        if (overlap.height == 1)
        {
            overlap.x = (int)Rx;
            overlap.width = 1 * DoorSize;
        }
        else if (overlap.width == 1)
        {
            overlap.y = (int)Ry;
            overlap.height = 1 * DoorSize;
        }

        Doors.Add(overlap);
    }

    public void MakeGraphKeysRoom(int i)
    {
        Vector3 roomMiddle = new(DoneRooms[i].x + DoneRooms[i].width / 2, 0, DoneRooms[i].y + DoneRooms[i].height / 2);
        RoomGraph.AddNode(roomMiddle);
    }

    public void GetGraphEdgesRoom(int i)
    {
        foreach(var door in Doors)
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
        Vector3 doorMiddle = new(Doors[i].x + Doors[i].width/2f, 0, Doors[i].y + Doors[i].height/2f);
        DoorGraph.AddNode(doorMiddle);
    }

    void DrawAll()
    {
        if (ShowDoneRooms)
        {
            //Drawing Done Rooms
            for (int i = 0; i < DoneRooms.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(DoneRooms[i], colors[0], 0, false, DungeonDrawHeight);
            }
        }

        //Drawing Not Done Rooms
        for (int i = 0; i < ToDoRooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(ToDoRooms[i], colors[1]);
        }

        //Drawing Current Room
        AlgorithmsUtils.DebugRectInt(CurrentRoom, colors[2]);


        if (Overlaps.Count > 0)
        {
            //Drawing Overlaps
            for (int i = 0; i < Overlaps.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(Overlaps[i], colors[3], 0, false, DungeonDrawHeight);
            }
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(Doors[i], colors[4], 0, false, DungeonDrawHeight);
        }

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

    //public void BFS()
    //{
    //    Debug.Log("BFS Started:");
    //    //Debug.Log("TODO: Print every node in the graph using breadth first order starting from startNode");
    //    Queue<Vector3> Todo = new();
    //    HashSet<Vector3> DiscoveredNodes = new();
    //    Todo.Enqueue(RoomList);
    //    DiscoveredNodes.Add(startNode);

    //    while (Todo.Count > 0)
    //    {
    //        var currentNode = Todo.Dequeue();
    //        var Neighbors = GetNeighbors(currentNode);
    //        Debug.Log($"{currentNode}: {string.Join(", ", Neighbors)}");


    //        foreach (var neighbor in Neighbors)
    //        {
    //            if (DiscoveredNodes.Contains(neighbor)) continue;
    //            Todo.Enqueue(neighbor);
    //            DiscoveredNodes.Add(neighbor);
    //        }

    //    }
    //}

}
