using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
public class DungeonGenerator : MonoBehaviour
{
    //public RectInt StartRoom;
    public List<RectInt> rooms;
    //public List<RectInt> outputRooms;

    public bool splitHorizontally;
    public int roomIndex;

    public int N;

    private Color StartRoom = Color.green;
    private Color OutputRoom = Color.red;

    public Color[] colors = { Color.green, Color.red };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(rooms[i], colors[i % 2]);
        }


        //foreach (var room in rooms)
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
    public void SplitRoomNTimes()
    {
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
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void SplitRoomUntilWidthLowerThan10()
    {
        while (rooms[roomIndex].width > 10)
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.width = rooms[roomIndex].width / 2 + 1;
            rooms.Add(newRoom);

            newRoom.width = rooms[roomIndex].width - rooms[roomIndex].width / 2;
            newRoom.x = rooms[roomIndex].x + rooms[roomIndex].width / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void SplitRoomUntilHeightLowerThan10()
    {
        while (rooms[roomIndex].height > 10)
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
    public void SplitRoomUntilHeightLowerThan10AndWidthLowerThan10()
    {
        while (rooms[roomIndex].height > 10)
        {
            RectInt newRoom = rooms[roomIndex];
            newRoom.height = rooms[roomIndex].height / 2 + 1;
            rooms.Add(newRoom);

            newRoom.height = rooms[roomIndex].height - rooms[roomIndex].height / 2;
            newRoom.y = rooms[roomIndex].y + rooms[roomIndex].height / 2;

            rooms.Add(newRoom);
            rooms.Remove(rooms[roomIndex]);
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
        }
    }
}
