using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject player;
    public GameObject boss; // @TODO have room prefabs with bosss
    
    public static int ROOM_SIZE = 19;
    
    [SerializeField] private int seed;
    [SerializeField] private bool generateNewSeed = false;
    [SerializeField] private int desiredRoomCount = 10;
    [SerializeField] private List<Room> prefabs;

    private Dictionary<Vector2Int, Room> rooms;
    private int roomCount; // this is the number of spawned rooms + the number of connected spawn points
    private Room spawn;
    private Room end;
    private List<Room> criticalPath;

    private void Start()
    {
        this.rooms = new Dictionary<Vector2Int, Room>();
        this.criticalPath = new List<Room>();
        this.Seed();
        this.GenerateRoom();
    }

    private void Clear()
    {
        foreach (KeyValuePair<Vector2Int, Room> room in this.rooms)
        {
            Destroy(room.Value.gameObject);
        }

        this.rooms.Clear();
        this.criticalPath.Clear();
        this.Seed();
        this.roomCount = 0;
        this.spawn = null;
        this.boss = null;
    }

    private bool isValidDungeon()
    {
        if (this.rooms.Count < this.desiredRoomCount || !this.end)
        {
            return false;
        }

        return true;
    }

    private void Seed()
    {
        if (this.generateNewSeed)
        {
            this.seed = System.Environment.TickCount;
        }
        Random.InitState(this.seed);
    }

    private void GenerateRoom()
    {
        this.SpawnRoom(Vector2Int.zero);
        this.SelectBossRoom();
        this.ChooseCriticalPath();
        this.SpawnEntities();
        if (!this.isValidDungeon())
        {
            this.Clear();
            this.GenerateRoom();
        }
    }

    private void SpawnRoom(Vector2Int position, Room previousRoom = null)
    {
        Room alreadyExists;
        if (this.rooms.TryGetValue(position, out alreadyExists))
        {
            return;
        }
        
        List<Room> allowedPrefabs = this.prefabs;
        Room roomUp, roomDown, roomRight, roomLeft = null;
        Vector2Int positionUp = position + Vector2Int.up * ROOM_SIZE;
        Vector2Int positionRight = position + Vector2Int.right * ROOM_SIZE;
        Vector2Int positionDown = position + Vector2Int.down * ROOM_SIZE;
        Vector2Int positionLeft = position + Vector2Int.left * ROOM_SIZE;
        this.rooms.TryGetValue(positionRight, out roomRight);
        this.rooms.TryGetValue(positionLeft, out roomLeft);
        this.rooms.TryGetValue(positionUp, out roomUp);
        this.rooms.TryGetValue(positionDown, out roomDown);
        
        // remove rooms that connect to a dead end
        if (roomDown && !roomDown.isOpenUp)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenDown).ToList();
        }
        if (roomLeft && !roomLeft.isOpenRight)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenLeft).ToList();
        }
        if (roomUp && !roomUp.isOpenDown)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenUp).ToList();
        }
        if (roomRight && !roomRight.isOpenLeft)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenRight).ToList();
        }
        
        // select rooms that connect to neighbors with open paths to this room
        if (roomDown && roomDown.isOpenUp)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isOpenDown).ToList();
        }
        if (roomLeft && roomLeft.isOpenRight)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isOpenLeft).ToList();
        }
        if (roomUp && roomUp.isOpenDown)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isOpenUp).ToList();
        }
        if (roomRight && roomRight.isOpenLeft)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isOpenRight).ToList();
        }
        
        // minimize pathways to only connect with neighboring rooms
        if (this.rooms.Count >= this.desiredRoomCount)
        {
            if (!roomUp)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenUp).ToList();
            }
            if (!roomRight)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenRight).ToList();
            }
            if (!roomDown)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenDown).ToList();
            }
            if (!roomLeft)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenLeft).ToList();
            }
        }

        // cant spawn here, dead space
        if (allowedPrefabs.Count == 0)
        {
            return;
        }
        
        // generate the room
        Room randomRoomPrefab = allowedPrefabs[Random.Range(0, allowedPrefabs.Count)];
        GameObject roomObj = Instantiate(randomRoomPrefab.gameObject);
        roomObj.transform.position = (Vector3Int) position;
        Room roomInstance = roomObj.GetComponent<Room>();
        int distanceFromSpawn = previousRoom ? previousRoom.distanceFromSpawn + 1 : 0;
        roomInstance.distanceFromSpawn = distanceFromSpawn;
        roomInstance.previousRoom = previousRoom;
        this.rooms.Add(position, roomInstance);
        
        if (!previousRoom)
        {
            roomInstance.isSpawn = true;
            this.spawn = roomInstance;
        }

        // check to see if other rooms need to be spawned
        if (randomRoomPrefab.isOpenUp && !roomUp)
        {
            this.SpawnRoom(positionUp, roomInstance);
        }
        if (randomRoomPrefab.isOpenRight && !roomRight)
        {
            this.SpawnRoom(positionRight, roomInstance);
        }
        if (randomRoomPrefab.isOpenDown && !roomDown)
        {
            this.SpawnRoom(positionDown, roomInstance);
        }
        if (randomRoomPrefab.isOpenLeft && !roomLeft)
        {
            this.SpawnRoom(positionLeft, roomInstance);
        }
    }

    // @TODO replace the end room with a boss room instead of spawning boss in the room
    private void SelectBossRoom()
    {
        List<KeyValuePair<Vector2Int, Room>> possibleFinalRoom = this.rooms.Where(r => r.Value.numberOfPaths == 1).OrderBy(r => r.Value.distanceFromSpawn).ToList();
        if (possibleFinalRoom.Count > 0)
        {
            Room room = possibleFinalRoom.Last().Value;
            room.isBoss = true;
            this.end = room;
            this.criticalPath.Add(room);
        }
    }

    // @TODO replace rooms if they dont have a allowed on cirtical path bool set to true
    // @TODO regenerate the critical path to be as short as possible
    private void ChooseCriticalPath()
    {
        int distanceFromSpawn = this.criticalPath.Last().distanceFromSpawn;
        while (distanceFromSpawn > 0)
        {
            Room room = this.criticalPath.Last();
            room.isOnCriticalPath = true;
            this.criticalPath.Add(room.previousRoom);
            distanceFromSpawn = this.criticalPath.Last().distanceFromSpawn;
        }
    }

    private void SpawnEntities()
    {
        this.player.transform.position = this.spawn.transform.position + new Vector3(ROOM_SIZE/2f, ROOM_SIZE/2f, 0);
        this.boss.transform.position = this.end.transform.position + new Vector3(ROOM_SIZE/2f, ROOM_SIZE/2f, 0);
    }
}
