using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class NewDungeonGenerator : Generator
{
    public List<RectInt> startRoom;

    private List<RectInt> toDoRooms = new();
    [HideInInspector]
    public List<RectInt> doneRooms = new();
    private List<RectInt> Overlaps = new();
    //[HideInInspector]
    public List<RectInt> doors = new();

    private RectInt currentRoom;

    public RectInt minRoomSize = new RectInt(0, 0, 8, 8);

    public bool splitHorizontally;
    public int roomIndex;

    public bool showDoneRooms = true;
    public bool showOverlaps = true;

    public Vector2 doorRandomRange = new Vector2(1,7);
    public int doorSize = 2;

    public float dungeonDrawHeight;

    public int seed;
    public bool useRandomSeed;

    private SearchDungeon searchDungeon;


    private Cam cameraScript;

    public void Start()
    {
        cameraScript = GetComponent<Cam>();

        searchDungeon = GetComponent<SearchDungeon>();
    }

    void Update()
    {
        DrawAll();
    }
    public void SplitRandom()
    {
        int random;
        RectInt newRoom = toDoRooms[roomIndex];
        toDoRooms.Remove(toDoRooms[roomIndex]);
        currentRoom = newRoom;

        if (newRoom.width <= minRoomSize.width * 2 && newRoom.height <= minRoomSize.height * 2)
        {
            doneRooms.Add(newRoom);
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

                toDoRooms.Add(newRoom);

                newRoom.height = currentRoom.height - random + 1;
                newRoom.y = newRoom.y + random - 1;

                toDoRooms.Add(newRoom);
            }
            else
            {
                random = Random.Range(minRoomSize.width, newRoom.width - minRoomSize.width);
                newRoom.width = random;

                toDoRooms.Add(newRoom);

                newRoom.width = currentRoom.width - random + 1;
                newRoom.x = newRoom.x + random - 1;

                toDoRooms.Add(newRoom);
                splitHorizontally = false;
            }
        }
        //currentRoom = new RectInt();
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public IEnumerator GenerateDungeon()
    {
        cameraScript.UpdateCam();

        if (useRandomSeed)
        {
            seed = System.DateTime.Now.GetHashCode();
            Debug.Log("Random seed used = " + seed);
        }
        else
        {
            Debug.Log("Seed from inspector used = " + seed);
        }

        Random.InitState(seed);
        //Debug.Log("Current Seed in use = " + seed);

        toDoRooms.Clear();
        doneRooms.Clear();
        Overlaps.Clear();
        doors.Clear();

        //Main --- Generator script testing
        DispatchOnStartGenerationEvent();
        audioSource.Play();

        toDoRooms.Add(startRoom[0]);

        while (toDoRooms.Count > 0)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            SplitRandom();
        }

        for (int i = 0; i < doneRooms.Count; i++)
        {
            for (int j = i + 1; j < doneRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersect(doneRooms[i], doneRooms[j]).width < 1 && AlgorithmsUtils.Intersect(doneRooms[i], doneRooms[j]).height < 1) continue;

                if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
                GetOverlaps(i, j);
            }
        }

        for (int i = 0; i < Overlaps.Count; i++)
        {
            if (splitType != SplitType.Instant) yield return CustomWait(splitType, splitDelay);
            PlaceDoors(i);
        }

        currentRoom = new();

        DispatchOnEndGenerationEvent();
    }
    public void GetOverlaps(int i, int j)
    {
        var OverlapedSpace = AlgorithmsUtils.Intersect(doneRooms[i], doneRooms[j]);
        currentRoom = OverlapedSpace;

        if (OverlapedSpace.width >= 5 * doorSize || OverlapedSpace.height >= 5 * doorSize)
        {
            Overlaps.Add(OverlapedSpace);
        }
    }

    public void PlaceDoors(int i)
    {
        var overlap = Overlaps[i];
        float Rx = Random.Range(overlap.x + 2 * doorSize, overlap.x + overlap.width - 2 * doorSize);
        float Ry = Random.Range(overlap.y + 2 * doorSize, overlap.y + overlap.height - 2 * doorSize);

        if (overlap.height == 1)
        {
            overlap.x = (int)Rx;
            overlap.width = 1 * doorSize;
        }
        else if (overlap.width == 1)
        {
            overlap.y = (int)Ry;
            overlap.height = 1 * doorSize;
        }

        currentRoom = overlap;
        doors.Add(overlap);
    }

    void DrawAll()
    {
        if (showDoneRooms)
        {
            //Drawing Done Rooms
            for (int i = 0; i < doneRooms.Count; i++)
            {
                AlgorithmsUtils.DebugRectInt(doneRooms[i], colors[0], 0, false, dungeonDrawHeight);
            }
        }

        //Drawing Not Done Rooms
        for (int i = 0; i < toDoRooms.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(toDoRooms[i], colors[1]);
        }

        if (showOverlaps)
        {
            if (Overlaps.Count > 0)
            {
                //Drawing Overlaps
                for (int i = 0; i < Overlaps.Count; i++)
                {
                    AlgorithmsUtils.DebugRectInt(Overlaps[i], colors[3], 0, false, dungeonDrawHeight);
                }
            }
        }

        for (int i = 0; i < doors.Count; i++)
        {
            AlgorithmsUtils.DebugRectInt(doors[i], colors[4], 0, false, dungeonDrawHeight);
        }

        //Drawing Current Room
        AlgorithmsUtils.DebugRectInt(currentRoom, colors[2], 0, false, dungeonDrawHeight);
    }
}
