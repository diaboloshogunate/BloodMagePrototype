using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Rewired;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static int ROOM_SIZE = 19;
    
    public GameObject player;
    
    [SerializeField] private int seed;
    [SerializeField] private bool generateNewSeed = false;
    [SerializeField] private int desiredRoomCount = 25;
    [SerializeField] private int maxWidth = 6;
    [SerializeField] private int maxHeight  = 6;
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
        this.GenerateDungeon();
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
        this.end = null;
    }

    private bool isValidDungeon()
    {
        if (this.rooms.Count < this.desiredRoomCount || !this.end)
        {
            return false;
        }

        return true;
    }

    private List<Room> GetNeighboringRooms(Room room)
    {
        List<Room> neighbors = new List<Room>();
        Room roomUp, roomDown, roomRight, roomLeft = null;
        Transform trans = room.gameObject.transform;
        Vector2Int position = new Vector2Int(Mathf.FloorToInt(trans.position.x), Mathf.FloorToInt(trans.position.y));
        Vector2Int positionUp = position + Vector2Int.up * ROOM_SIZE;
        Vector2Int positionRight = position + Vector2Int.right * ROOM_SIZE;
        Vector2Int positionDown = position + Vector2Int.down * ROOM_SIZE;
        Vector2Int positionLeft = position + Vector2Int.left * ROOM_SIZE;
        this.rooms.TryGetValue(positionUp, out roomUp);
        this.rooms.TryGetValue(positionRight, out roomRight);
        this.rooms.TryGetValue(positionDown, out roomDown);
        this.rooms.TryGetValue(positionLeft, out roomLeft);
        if(roomUp) neighbors.Add(roomUp);
        if(roomRight) neighbors.Add(roomRight);
        if(roomDown) neighbors.Add(roomDown);
        if(roomLeft) neighbors.Add(roomLeft);
        
        return neighbors;
    }

    private List<Room> GetConnectedRooms(Room room)
    {
        List<Room> neighbors = new List<Room>();
        Room roomUp, roomDown, roomRight, roomLeft = null;
        Transform trans = room.gameObject.transform;
        Vector2Int position = new Vector2Int(Mathf.FloorToInt(trans.position.x), Mathf.FloorToInt(trans.position.y));
        Vector2Int positionUp = position + Vector2Int.up * ROOM_SIZE;
        Vector2Int positionRight = position + Vector2Int.right * ROOM_SIZE;
        Vector2Int positionDown = position + Vector2Int.down * ROOM_SIZE;
        Vector2Int positionLeft = position + Vector2Int.left * ROOM_SIZE;
        this.rooms.TryGetValue(positionUp, out roomUp);
        this.rooms.TryGetValue(positionRight, out roomRight);
        this.rooms.TryGetValue(positionDown, out roomDown);
        this.rooms.TryGetValue(positionLeft, out roomLeft);
        if(roomUp && roomUp.isOpenDown && room.isOpenUp) neighbors.Add(roomUp);
        if(roomRight && roomRight.isOpenLeft && room.isOpenRight) neighbors.Add(roomRight);
        if(roomDown && roomDown.isOpenUp && room.isOpenDown) neighbors.Add(roomDown);
        if(roomLeft && roomLeft.isOpenRight && room.isOpenLeft) neighbors.Add(roomLeft);
        
        return neighbors;
    }

    private void Seed()
    {
        if (this.generateNewSeed)
        {
            this.seed = System.Environment.TickCount;
        }
        Random.InitState(this.seed);
    }

    private void GenerateDungeon()
    {
        this.SpawnRoom(Vector2Int.zero);
        this.SpawnBossRoom();
        if (this.end)
        {
            this.ChooseCriticalPath();
            this.SpawnPlayer();
        }

        if (!this.isValidDungeon())
        {
            this.Clear();
            this.GenerateDungeon();
        }
    }

    [CanBeNull]
    private Room SpawnRoom(Vector2Int position, Room previousRoom = null)
    {
        Room alreadyExists;
        if (this.rooms.TryGetValue(position, out alreadyExists)) return null;

        List<Room> allowedPrefabs = this.prefabs;
        Room roomUp, roomDown, roomRight, roomLeft = null;
        Vector2Int positionUp = position + Vector2Int.up * ROOM_SIZE;
        Vector2Int positionRight = position + Vector2Int.right * ROOM_SIZE;
        Vector2Int positionDown = position + Vector2Int.down * ROOM_SIZE;
        Vector2Int positionLeft = position + Vector2Int.left * ROOM_SIZE;
        this.rooms.TryGetValue(positionUp, out roomUp);
        this.rooms.TryGetValue(positionRight, out roomRight);
        this.rooms.TryGetValue(positionDown, out roomDown);
        this.rooms.TryGetValue(positionLeft, out roomLeft);
        
        // remove rooms that connect to a dead end
        if (roomUp && !roomUp.isOpenDown) allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenUp).ToList();
        if (roomRight && !roomRight.isOpenLeft) allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenRight).ToList();
        if (roomDown && !roomDown.isOpenUp) allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenDown).ToList();
        if (roomLeft && !roomLeft.isOpenRight) allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenLeft).ToList();

        // select rooms that connect to neighbors with open paths to this room
        if (roomUp && roomUp.isOpenDown) allowedPrefabs = allowedPrefabs.Where(r => r.isOpenUp).ToList();
        if (roomRight && roomRight.isOpenLeft) allowedPrefabs = allowedPrefabs.Where(r => r.isOpenRight).ToList();
        if (roomDown && roomDown.isOpenUp) allowedPrefabs = allowedPrefabs.Where(r => r.isOpenDown).ToList();
        if (roomLeft && roomLeft.isOpenRight) allowedPrefabs = allowedPrefabs.Where(r => r.isOpenLeft).ToList();

        // dont spawn past bounds
        if (position.x <= ROOM_SIZE * (this.maxWidth/2 - 1) * -1)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenLeft).ToList();
        }
        if (position.x >= ROOM_SIZE * this.maxWidth/2)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenRight).ToList();
        }
        if (position.y <= ROOM_SIZE * (this.maxHeight/2 - 1) * -1)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenDown).ToList();
        }
        if (position.y >= ROOM_SIZE * this.maxHeight/2)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isOpenUp).ToList();
        }
        
        // minimize pathways to only connect with neighboring rooms
        // rooms is connected to a previous room so it can at most add 3 more rooms or roomcount - 1
        if (this.roomCount + 3 >= this.desiredRoomCount)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.numberOfPaths + this.roomCount - 1 <= this.desiredRoomCount || r.numberOfPaths == 1 ).ToList();
        }
        // not approaching room limit yet, dont choose an end room
        else
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.numberOfPaths > 1 ).ToList();
        }

        // cant spawn here, dead space
        if (allowedPrefabs.Count == 0)
        {
            return null;
        }

        // spawn or not
        allowedPrefabs = allowedPrefabs.Where(r => r.isSpawn == !previousRoom).ToList();
        
        // filter out bosses
        allowedPrefabs = allowedPrefabs.Where(r => !r.isBoss).ToList();
        
        // generate the room
        int weight = Random.Range(0, allowedPrefabs.Sum(r => r.weight));
        Room randomRoomPrefab = null;
        foreach (Room room in allowedPrefabs)
        {
            weight -= room.weight;
            if (weight <= 0)
            {
                randomRoomPrefab = room;
                break;
            }
        }
        GameObject roomObj = Instantiate(randomRoomPrefab.gameObject, this.gameObject.transform);
        roomObj.transform.position = (Vector3Int) position;
        Room roomInstance = roomObj.GetComponent<Room>();
        int distanceFromSpawn = previousRoom ? previousRoom.distanceFromSpawn + 1 : 0;
        roomInstance.distanceFromSpawn = distanceFromSpawn;
        roomInstance.previousRoom = previousRoom;
        this.rooms.Add(position, roomInstance);
        
        if (!previousRoom)
        {
            this.roomCount = 1;
            roomInstance.isSpawn = true;
            this.spawn = roomInstance;
        }

        // check to see if other rooms need to be spawned
        if (randomRoomPrefab.isOpenUp && !roomUp) this.roomCount += 1;
        if (randomRoomPrefab.isOpenRight && !roomRight) this.roomCount += 1;
        if (randomRoomPrefab.isOpenDown && !roomDown) this.roomCount += 1;
        if (randomRoomPrefab.isOpenLeft && !roomLeft) this.roomCount += 1;
        if (randomRoomPrefab.isOpenUp && !roomUp) roomInstance.roomUp = this.SpawnRoom(positionUp, roomInstance);
        if (randomRoomPrefab.isOpenRight && !roomRight) roomInstance.roomRight = this.SpawnRoom(positionRight, roomInstance);
        if (randomRoomPrefab.isOpenDown && !roomDown) roomInstance.roomDown = this.SpawnRoom(positionDown, roomInstance);
        if (randomRoomPrefab.isOpenLeft && !roomLeft)  roomInstance.roomLeft = this.SpawnRoom(positionLeft, roomInstance);

        return roomInstance;
    }

    private void SpawnBossRoom()
    {
        Debug.Log("Spawning a boss room");
        List<KeyValuePair<Vector2Int, Room>> possibleFinalRoom = this.rooms.Where(r => r.Value.numberOfPaths == 1).OrderBy(r => r.Value.distanceFromSpawn).ToList();
        if (possibleFinalRoom.Count > 0)
        {
            Vector2Int roomPosition = possibleFinalRoom.Last().Key;
            Room oldRoom = possibleFinalRoom.Last().Value;
            
            // generate the room
            List<Room> allowedPrefabs = this.prefabs.Where(r => r.isBoss && r.isOpenUp == oldRoom.isOpenUp && r.isOpenRight == oldRoom.isOpenRight && r.isOpenDown == oldRoom.isOpenDown && r.isOpenLeft == oldRoom.isOpenLeft).ToList();
            int weight = Random.Range(0, allowedPrefabs.Sum(r => r.weight));
            Room randomRoomPrefab = null;
            foreach (Room room in allowedPrefabs)
            {
                weight -= room.weight;
                if (weight <= 0)
                {
                    randomRoomPrefab = room;
                    break;
                }
            }
            GameObject bossObj = Instantiate(randomRoomPrefab.gameObject, this.gameObject.transform);
            Room bossRoom = bossObj.GetComponent<Room>();
            bossRoom.transform.position = oldRoom.transform.position;
            bossRoom.previousRoom = oldRoom.previousRoom;
            bossRoom.distanceFromSpawn = oldRoom.distanceFromSpawn;
            Destroy(oldRoom.gameObject);
            this.rooms.Remove(roomPosition);
            this.rooms.Add(roomPosition, bossRoom);
            this.end = bossRoom;
            this.criticalPath.Add(bossRoom);
        }
    }

    private void ChooseCriticalPath()
    {
        int distanceFromSpawn = this.criticalPath.Last().distanceFromSpawn;
        while (distanceFromSpawn > 0)
        {
            Room room = this.criticalPath.Last();
            room.isOnCriticalPath = true;
            List<Room> neightbors = this.GetConnectedRooms(room);
            neightbors = neightbors.OrderBy(r => r.distanceFromSpawn).ToList();
            room.previousRoom = neightbors.First();
            this.criticalPath.Add(neightbors.First());
            distanceFromSpawn = this.criticalPath.Last().distanceFromSpawn;
        }
    }

    private void SpawnPlayer()
    {
        this.player.transform.position = this.spawn.transform.position + new Vector3(ROOM_SIZE/2f, ROOM_SIZE/2f, 0);
    }
}
