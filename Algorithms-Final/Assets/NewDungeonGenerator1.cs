using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewDungeonGenerator : MonoBehaviour
{
    //public RectInt StartRoom;
    public List<RectInt> rooms;

    [SerializeField] 
    private List<RectInt> ToDoRooms = new List<RectInt>();
    [SerializeField]
    private List<RectInt> DoneRooms = new List<RectInt>();

    private RectInt CurrentRoom;

    public RectInt minRoomSize = new RectInt(0,0,8,8);

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

    void Update()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            //AlgorithmsUtils.DebugRectInt(rooms[i], colors[i % 2]);
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
