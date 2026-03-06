using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
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
                    PlaceDoors(i);
                    break;
            }
        }
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

        if (OverlapedSpace.width >= 5 || OverlapedSpace.height >= 5)
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
        float Rx = Random.Range(overlap.x + 2, overlap.x + overlap.width - 2);
        float Ry = Random.Range(overlap.y + 2, overlap.y + overlap.height - 2);

        if (overlap.height == 1)
        {
            overlap.x = (int)Rx;
            overlap.width = 1;
        }
        else if (overlap.width == 1)
        {
            overlap.y = (int)Ry;
            overlap.height = 1;
        }

        Doors.Add(overlap);
    }

}
