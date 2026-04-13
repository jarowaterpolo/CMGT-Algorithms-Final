using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DungeonGenerator : MonoBehaviour
{
    //public RectInt startRoom;
    public List<RectInt> rooms;

    [SerializeField] 
    private List<RectInt> ToDoRooms = new List<RectInt>();
    [SerializeField]
    private List<RectInt> DoneRooms = new List<RectInt>();

    private RectInt CurrentRoom;

    public RectInt minRoomSize;

    public bool splitHorizontally;
    public int roomIndex;

    public int N;
    public float SplitDelay = 0.2f;

    public enum SplitType
    {
        Instant, Overtime, Space
    }

    public SplitType splitType;

    public Color[] colors = { Color.green, Color.red, Color.cyan };

    private Color StartRoom = Color.green;
    private Color OutputRoom = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            //AlgorithmsUtils.DebugRectInt(startRoom[i], colors[i % 2]);
        }

        for (int i = 0; i < DoneRooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(DoneRooms[i], colors[0]);
        }
        for (int i = 0; i < ToDoRooms.Count; i++) 
        {
            AlgorithmsUtils.DebugRectInt(ToDoRooms[i], colors[1]);
        }

        AlgorithmsUtils.DebugRectInt(CurrentRoom, colors[2]);
        //foreach (var room in startRoom)
        //{
        //    AlgorithmsUtils.DebugRectInt(room, colors[N % 2]);
        //}
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void SplitRoom()
    {
        if (splitHorizontally == true)
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.width = rooms[roomIndex].width / 2 + 1;
            rooms.Add(newRoom);

            newRoom.width = rooms[roomIndex].width - rooms[roomIndex].width / 2;
            newRoom.x = rooms[roomIndex].x + rooms[roomIndex].width / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
        }
        else
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.height = rooms[roomIndex].height / 2 + 1;
            rooms.Add(newRoom);

            newRoom.height = rooms[roomIndex].height - rooms[roomIndex].height / 2;
            newRoom.y = rooms[roomIndex].y + rooms[roomIndex].height / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator CouroutineSplitRoomNTimes()
    {
        yield return null;
        ToDoRooms.Clear();
        DoneRooms.Clear();

        ToDoRooms.Add(rooms[0]);

        for (int i = 0; i < N; i++)
        {
            if (splitHorizontally == true)
            {
                RectInt newRoom = rooms[roomIndex];
                newRoom.width = rooms[roomIndex].width / 2 + 1;
                rooms.Add(newRoom);

                newRoom.width = rooms[roomIndex].width - rooms[roomIndex].width / 2;
                newRoom.x = rooms[roomIndex].x + rooms[roomIndex].width / 2;

                rooms.Add(newRoom);
                rooms.Remove(rooms[roomIndex]);
                yield return new WaitForSeconds(SplitDelay);
            }
            else
            {
                RectInt newRoom = rooms[roomIndex];
                newRoom.height = rooms[roomIndex].height / 2 + 1;
                rooms.Add(newRoom);

                newRoom.height = rooms[roomIndex].height - rooms[roomIndex].height / 2;
                newRoom.y = rooms[roomIndex].y + rooms[roomIndex].height / 2;

                rooms.Add(newRoom);
                rooms.Remove(rooms[roomIndex]);
                yield return new WaitForSeconds(SplitDelay);
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SplitUntilHAndWLowerThan10()
    {
        yield return null;
        ToDoRooms.Clear();
        DoneRooms.Clear();

        ToDoRooms.Add(rooms[0]);

        while (rooms[roomIndex].height > 10)
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.height = rooms[roomIndex].height / 2 + 1;
            rooms.Add(newRoom);

            newRoom.height = rooms[roomIndex].height - rooms[roomIndex].height / 2;
            newRoom.y = rooms[roomIndex].y + rooms[roomIndex].height / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
            yield return new WaitForSeconds(SplitDelay);
        }

        while (rooms[roomIndex].width > 10)
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.width = rooms[roomIndex].width / 2 + 1;
            rooms.Add(newRoom);

            newRoom.width = rooms[roomIndex].width - rooms[roomIndex].width / 2;
            newRoom.x = rooms[roomIndex].x + rooms[roomIndex].width / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
            yield return new WaitForSeconds(SplitDelay);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator CourountineSplitUntilMinRoomSize()
    {
        yield return null;
        ToDoRooms.Clear();
        DoneRooms.Clear();

        ToDoRooms.Add(rooms[0]);

        int i = 0;
        while (ToDoRooms.Count > 0)
        {
            RectInt newRoom = ToDoRooms[roomIndex];
            CurrentRoom = newRoom;

            if (newRoom.height < minRoomSize.height)
            {
                i++;
            }

            if (i == 0) 
            {
                newRoom.height = ToDoRooms[roomIndex].height / 2 + 1;

                ToDoRooms.Add(newRoom);


                newRoom.height = ToDoRooms[roomIndex].height - ToDoRooms[roomIndex].height / 2;
                newRoom.y = ToDoRooms[roomIndex].y + ToDoRooms[roomIndex].height / 2;

                ToDoRooms.Add(newRoom);
            }
            else
            {
                newRoom.width = ToDoRooms[roomIndex].width / 2 + 1;

                bool Requirement = (newRoom.width < minRoomSize.width);

                if (Requirement)
                {
                    DoneRooms.Add(newRoom);
                }
                else
                {
                    ToDoRooms.Add(newRoom);
                }

                newRoom.width = ToDoRooms[roomIndex].width - ToDoRooms[roomIndex].width / 2;
                newRoom.x = ToDoRooms[roomIndex].x + ToDoRooms[roomIndex].width / 2;

                if (Requirement)
                {
                    DoneRooms.Add(newRoom);
                }
                else
                {
                    ToDoRooms.Add(newRoom);
                }

                i--;
            }

            ToDoRooms.Remove(ToDoRooms[roomIndex]);
            yield return new WaitForSeconds(SplitDelay);
        }

        CurrentRoom = new RectInt();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator SplitRandomUntilMinRoomSize()
    {
        yield return null; 
        ToDoRooms.Clear();
        DoneRooms.Clear();

        ToDoRooms.Add(rooms[0]);

        int random;
        while (ToDoRooms.Count > 0)
        {
            //yield return null;

            RectInt newRoom = ToDoRooms[roomIndex];

            ToDoRooms.Remove(ToDoRooms[roomIndex]);

            CurrentRoom = newRoom;

            //Debug.Log("INPUT Room = " + currentRoom);

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

                    Debug.Log(minRoomSize.height + "  " + (newRoom.height - minRoomSize.height) + "  " + random);

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
            //yield return new WaitForSeconds(SplitDelay);
            //yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space)));
            //yield return null;

            CurrentRoom = new RectInt();
        }       
    }

    public void SplitRandom()
    {
        int random;
            //yield return null;

            RectInt newRoom = ToDoRooms[roomIndex];

            ToDoRooms.Remove(ToDoRooms[roomIndex]);

            CurrentRoom = newRoom;

            //Debug.Log("INPUT Room = " + currentRoom);

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

                    Debug.Log(minRoomSize.height + "  " + (newRoom.height - minRoomSize.height) + "  " + random);

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
            //yield return new WaitForSeconds(SplitDelay);
            //yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space)));
            //yield return null;

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
}
