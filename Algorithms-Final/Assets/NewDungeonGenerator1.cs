using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class NewDungeonGenerator : MonoBehaviour
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

    private Dictionary<Vector3, List<Vector3>> RoomList;
    private Dictionary<Vector3, List<Vector3>> DoorList;

    void Update()
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

        if (RoomList != null && RoomList.Count != 0)
        {
            foreach (var node in RoomList)
            {
                if (node.Key is Vector3 position)
                {
                    Debug.DrawRay(position, Vector3.up * 10, Color.blue);
                }
            }
        }

        if (RoomList != null && RoomList.Count != 0)
        {
            foreach (var node in RoomList)
            {
                if (node.Key is Vector3 position)
                {
                    foreach (var value in node.Value)
                    {
                        Debug.DrawLine(position, value, Color.blue);
                    }
                }
            }
        }

        if (DoorList != null && DoorList.Count != 0)
        {
            foreach (var node in DoorList)
            {
                if (node.Key is Vector3 position)
                {
                    Debug.DrawRay(position, Vector3.up * 10, Color.blue);
                }
            }
        }

        if (DoorList != null && DoorList.Count != 0)
        {
            foreach (var node in DoorList)
            {
                if (node.Key is Vector3 position)
                {
                    foreach (var value in node.Value)
                    {
                        Debug.DrawLine(position, value, Color.blue);
                    }
                }
            }
        }
    }
    public void SplitRandom()
    {
        int random;
        //yield return null;

        RectInt newRoom = ToDoRooms[roomIndex];

        ToDoRooms.Remove(ToDoRooms[roomIndex]);

        CurrentRoom = newRoom;

        //Debug.Log("INPUT Room = " + CurrentRoom);

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

                //Debug.Log(minRoomSize.height + "  " + (newRoom.height - minRoomSize.height) + "  " + random);

                newRoom.height = random;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);

                //testing for Overlap!!
                newRoom.height = CurrentRoom.height - random + 1;
                newRoom.y = newRoom.y + random - 1;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);
            }
            else
            {
                random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);

                newRoom.width = random;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);

                //testing for Overlap!!
                newRoom.width = CurrentRoom.width - random + 1;
                newRoom.x = newRoom.x + random - 1;

                //Debug.Log("OUTPUT Room = " + newRoom);
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
        RoomList = new();
        DoorList = new();


        ToDoRooms.Add(StartRoom[0]);

        while (ToDoRooms.Count > 0)
        {
            switch (splitType)
            {
                case SplitType.Instant:
                    SplitRandom();
                    break;
                case SplitType.Overtime:
                    yield return new WaitForSeconds(SplitDelay);
                    SplitRandom();
                    break;
                case SplitType.Space:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    yield return null;
                    SplitRandom();
                    break;
            }
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            for (int j = i + 1; j < DoneRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).width < 1 && AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]).height < 1) continue;

                switch (splitType)
                {
                    case SplitType.Instant:
                        GetOverlaps(i, j);
                        break;
                    case SplitType.Overtime:
                        yield return new WaitForSeconds(SplitDelay);
                        GetOverlaps(i, j);
                        break;
                    case SplitType.Space:
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                        yield return null;
                        GetOverlaps(i, j);
                        break;
                }
            }
        }

        for (int i = 0; i < Overlaps.Count; i++)
        {
            switch (splitType)
            {
                case SplitType.Instant:
                    PlaceDoors(i);
                    break;
                case SplitType.Overtime:
                    yield return new WaitForSeconds(SplitDelay);
                    PlaceDoors(i);
                    break;
                case SplitType.Space:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    yield return null;
                    PlaceDoors(i);
                    break;
            }
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            switch (splitType)
            {
                case SplitType.Instant:
                    MakeGraphKeysRoom(i);
                    break;
                case SplitType.Overtime:
                    yield return new WaitForSeconds(SplitDelay);
                    MakeGraphKeysRoom(i);
                    break;
                case SplitType.Space:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    yield return null;
                    MakeGraphKeysRoom(i);
                    break;
            }
        }

        //for (int i = 0; i < Doors.Count; i++)
        //{
        //    switch (splitType)
        //    {
        //        case SplitType.Instant:
        //            MakeGraphKeysDoor(i);
        //            break;
        //        case SplitType.Overtime:
        //            yield return new WaitForSeconds(SplitDelay);
        //            MakeGraphKeysDoor(i);
        //            break;
        //        case SplitType.Space:
        //            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        //            yield return null;
        //            MakeGraphKeysDoor(i);
        //            break;
        //    }
        //}

        for (int i = 0; i < RoomList.Count; i++)
        {
            switch (splitType)
            {
                case SplitType.Instant:
                    GetGraphEdgesRoom(i);
                    break;
                case SplitType.Overtime:
                    yield return new WaitForSeconds(SplitDelay);
                    GetGraphEdgesRoom(i);
                    break;
                case SplitType.Space:
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    yield return null;
                    GetGraphEdgesRoom(i);
                    break;
            }
        }

        //for (int i = 0; i < DoorList.Count; i++)
        //{
        //    switch (splitType)
        //    {
        //        case SplitType.Instant:
        //            GetGraphEdgesDoor(i);
        //            break;
        //        case SplitType.Overtime:
        //            yield return new WaitForSeconds(SplitDelay);
        //            GetGraphEdgesDoor(i);
        //            break;
        //        case SplitType.Space:
        //            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        //            yield return null;
        //            GetGraphEdgesDoor(i);
        //            break;
        //    }
        //}

        //foreach (var node in AdjacencyList)
        //{
        //    //Debug.Log(node.ToString());
        //    var EdgeList = node.Value;

        //    foreach (var edge in EdgeList)
        //    {
        //        Debug.Log("Node " + node.Key + " connects to " + edge.ToString());
        //    }
        //}
    }

    //[Space(10)]

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void SplitRandomRoomManually()
    {
        ToDoRooms.Clear();
        DoneRooms.Clear();
        Overlaps.Clear();
        Doors.Clear();

        ToDoRooms.Add(StartRoom[0]);

        while (ToDoRooms.Count > 0)
        {
            int random;
            //yield return null;

            RectInt newRoom = ToDoRooms[roomIndex];

            ToDoRooms.Remove(ToDoRooms[roomIndex]);

            CurrentRoom = newRoom;

            //Debug.Log("INPUT Room = " + CurrentRoom);

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

                    //Debug.Log(minRoomSize.height + "  " + (newRoom.height - minRoomSize.height) + "  " + random);

                    newRoom.height = random;

                    //Debug.Log("OUTPUT Room = " + newRoom);
                    ToDoRooms.Add(newRoom);

                    //testing for Overlap!!
                    newRoom.height = CurrentRoom.height - random + 1;
                    newRoom.y = newRoom.y + random - 1;

                    //Debug.Log("OUTPUT Room = " + newRoom);
                    ToDoRooms.Add(newRoom);
                }
                else
                {
                    random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);

                    newRoom.width = random;

                    //Debug.Log("OUTPUT Room = " + newRoom);
                    ToDoRooms.Add(newRoom);

                    //testing for Overlap!!
                    newRoom.width = CurrentRoom.width - random + 1;
                    newRoom.x = newRoom.x + random - 1;

                    //Debug.Log("OUTPUT Room = " + newRoom);
                    ToDoRooms.Add(newRoom);

                    splitHorizontally = false;
                }
            }
            CurrentRoom = new RectInt();
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void GetOverlapsManually()
    {
        Overlaps.Clear();
        Doors.Clear();

        //Getting Overlaps
        for (int i = 0; i < DoneRooms.Count; i++)
        {
            for (int j = i + 1; j < DoneRooms.Count; j++)
            {
                var OverlapedSpace = AlgorithmsUtils.Intersect(DoneRooms[i], DoneRooms[j]);

                if (OverlapedSpace != RectInt.zero && OverlapedSpace.width >= 1 && OverlapedSpace.height >= 1)
                {
                    if (OverlapedSpace.width >= 5 || OverlapedSpace.height >= 5)
                    {
                        Overlaps.Add(OverlapedSpace);
                    }
                }
            }

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

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void PlaceDoorsManually()
    {
        Doors.Clear();

        for (int i = 0; i < Overlaps.Count; i++)
        {
            var overlap = Overlaps[i];
            float Rx = Random.Range(overlap.x + 2, overlap.x + overlap.width - 2);
            float Ry = Random.Range(overlap.y + 2, overlap.y + overlap.height - 2);

            if (overlap.height == 1)
                {
                    overlap.x = (int)Rx;
                //overlap.x += overlap.width / 2 - 1;
                overlap.width = 1;
                }
                else if (overlap.width == 1)
                {
                    overlap.y = (int)Ry;
                //overlap.y += overlap.height / 2 - 1;
                overlap.height = 1;
                }

                Doors.Add(overlap);
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
            AddNodeRoom(roomMiddle);
            //Debug.Log(roomMiddle);
    }

    public void GetGraphEdgesRoom(int i)
    {
        //Check if the rooms overlap with the doors then add them
        foreach(var door  in Doors)
        {
            if (AlgorithmsUtils.Intersects(DoneRooms[i], door))
            {
                var middleRoom = new Vector3(DoneRooms[i].position.x + DoneRooms[i].width / 2, 0, DoneRooms[i].position.y + DoneRooms[i].height / 2);
                Vector3 doorMiddle = new(door.x + door.width / 2, 0, door.y + door.height / 2);

                if (door.width == 1)
                {
                    doorMiddle.x += .5f;
                }

                if (door.height == 1)
                {
                    doorMiddle.z += .5f;
                }

                if (RoomList.ContainsKey(middleRoom))
                {
                    AddEdgeRoom(middleRoom, doorMiddle);
                }
            }
        }


        //foreach (var fromNode in RoomList.Keys)
        //{
        //    foreach (var toNode in DoorList.Keys)
        //    {
        //        if (fromNode == toNode) continue;
        //        Vector3 VectorBetweenNodes = toNode - fromNode;
        //        //Debug.Log("Vector Between nodes = " + VectorBetweenNodes);
        //        //Debug.Log("magnitude = " + VectorBetweenNodes.magnitude);
        //        if (VectorBetweenNodes.magnitude > 15) continue;
        //        AddEdgeRoom(fromNode, toNode);
        //    }
        //}
    }

    public void MakeGraphKeysDoor(int i)
    {
        Vector3 doorMiddle = new(Doors[i].x + Doors[i].width/2, 0, Doors[i].y + Doors[i].height/2);

        if (Doors[i].width == 1)
        {
            doorMiddle.x += .5f;
        }

        if (Doors[i].height == 1)
        {
            doorMiddle.z += .5f;
        }

        AddNodeDoor(doorMiddle);
    }

    //public void GetGraphEdgesDoor(int i)
    //{
    //    foreach (var fromNode in DoorList.Keys)
    //    {
    //        foreach (var toNode in RoomList.Keys)
    //        {
    //            if (fromNode == toNode) continue;
    //            Vector3 VectorBetweenNodes = toNode - fromNode;
    //            //Debug.Log("Vector Between nodes = " + VectorBetweenNodes);
    //            //Debug.Log("magnitude = " + VectorBetweenNodes.magnitude);
    //            if (VectorBetweenNodes.magnitude > 15) continue;
    //            AddEdgeDoor(fromNode, toNode);
    //        }
    //    }
    //}

    public void AddNodeRoom(Vector3 node)
    {
        //Debug.Log("TODO: Implement AddNode logic (add node if it does not already exist)");
        if (RoomList.ContainsKey(node)) return;
        RoomList.Add(node, new());
    }

    public void AddEdgeRoom(Vector3 fromNode, Vector3 toNode)
    {
        //Debug.Log("TODO: Implement AddEdge logic (validate nodes and connect them)");
        RoomList[fromNode].Add(toNode);
    }

    public void AddNodeDoor(Vector3 node)
    {
        //Debug.Log("TODO: Implement AddNode logic (add node if it does not already exist)");
        if (DoorList.ContainsKey(node)) return;
        DoorList.Add(node, new());
    }

    public void AddEdgeDoor(Vector3 fromNode, Vector3 toNode)
    {
        //Debug.Log("TODO: Implement AddEdge logic (validate nodes and connect them)");
        DoorList[fromNode].Add(toNode);
    }

}
