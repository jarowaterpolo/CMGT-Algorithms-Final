using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewDungeonGenerator : MonoBehaviour
{
    //public RectInt StartRoom;
    public List<RectInt> rooms;

    [SerializeField]
    private List<RectInt> ToDoRooms = new();
    [SerializeField]
    private List<RectInt> DoneRooms = new();
    [SerializeField]
    private List<RectInt> OverlappingRooms = new();
    [SerializeField]
    private List<RectInt> Overlaps = new();
    [SerializeField]
    private List<RectInt> Doors = new();

    private RectInt CurrentRoom;

    public RectInt minRoomSize = new RectInt(0, 0, 8, 8);

    public bool splitHorizontally;
    public int roomIndex;

    public int N;
    public float SplitDelay = 0.2f;

    public int Overlap = 2;

    public enum SplitType
    {
        Instant, Overtime, Space
    }

    public SplitType splitType;

    public Color[] colors = { Color.green, Color.red, Color.cyan, Color.black, new Color(255,175,0,1)};

    public bool ShowDoneRooms = true;

    public Vector2 DoorRandomRange = new Vector2(1,7);

    void Update()
    {
        if (ShowDoneRooms)
        {
            //Drawing Done Rooms
            for (int i = 0; i < DoneRooms.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(DoneRooms[i], colors[0]);
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
                AlgorithmsUtils.DebugRectInt(Overlaps[i], colors[3]);
            }
        }

        for (int i = 0; i < Doors.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(Doors[i], colors[4]);
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

                newRoom.height = CurrentRoom.height - random;
                newRoom.y = newRoom.y + random;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);
            }
            else
            {
                random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);

                newRoom.width = random;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);

                newRoom.width = CurrentRoom.width - random;
                newRoom.x = newRoom.x + random;

                //Debug.Log("OUTPUT Room = " + newRoom);
                ToDoRooms.Add(newRoom);

                splitHorizontally = false;
            }
        }
        CurrentRoom = new RectInt();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SplitRooms()
    {
        ToDoRooms.Clear();
        DoneRooms.Clear();

        ToDoRooms.Add(rooms[0]);

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
                    SplitRandom();
                    break;
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void GetOverlaps()
    {
        OverlappingRooms.Clear();

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            var room = DoneRooms[i];
            room.width += Overlap;
            room.height += Overlap;
            room.x -= Overlap / 2;
            room.y -= Overlap / 2;
            OverlappingRooms.Add(room);
        }

        Overlaps.Clear();

        //Getting Overlaps
        for (int i = 0; i < OverlappingRooms.Count; i++)
        {
            for (int j = 0; j < OverlappingRooms.Count; j++)
            {
                if (i == j) continue;
                if (i < j) continue;
                var OverlapedSpace = AlgorithmsUtils.Intersect(OverlappingRooms[i], OverlappingRooms[j]);
                if (OverlapedSpace != RectInt.zero && OverlapedSpace.width >= Overlap && OverlapedSpace.height >= Overlap)
                {
                    Overlaps.Add(OverlapedSpace);
                }
            }

        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void PlaceDoors()
    {
        for (int i = 0; i < Overlaps.Count; i++)
        {
            var overlap = Overlaps[i];
            float Ro = Random.Range(DoorRandomRange[0], DoorRandomRange[1]);

            if (overlap.width >= Overlap && overlap.height >= Overlap) continue;

            if (overlap.height == Overlap)
            {
                //overlap.x += (int)Ro;
                overlap.x += overlap.width / 2;
                overlap.width = minRoomSize.width / 4;
            }
            else if (overlap.width == Overlap)
            {
                //overlap.y += (int)Ro;
                overlap.y += overlap.height / 2;
                overlap.height /= minRoomSize.height / 4;
            }

            if (overlap.width >= Overlap && overlap.height >= Overlap)
            {
                Doors.Add(overlap);
            }
        }
    }

}
