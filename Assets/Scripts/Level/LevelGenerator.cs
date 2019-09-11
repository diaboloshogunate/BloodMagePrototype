using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public int seed = 1;
    public bool useRandomSeed = true;
    private const int ROOM_SIZE = 11;
    public int minRooms = 10;
    public int minDistanceForFinalRoom = 8;
    public List<RoomLayer> layers;
    public List<Room> roomPrefabs;
    private Dictionary<Vector2Int, Room> rooms;
    private Room spawn;
    private Room final;


    private void Start()
    {
        this.rooms = new Dictionary<Vector2Int, Room>();
        Random.InitState(this.useRandomSeed ? Guid.NewGuid().GetHashCode() : 1);
        this.Rebuild();
    }

    private bool IsDungeonValid()
    {
        if (this.rooms.Count < this.minRooms || !this.final)
        {
            return false;
        }

        return true;
    }

    private void Rebuild()
    {
        do
        {
            foreach (KeyValuePair<Vector2Int, Room> room in this.rooms)
            {
                Destroy(room.Value.gameObject);
            }

            foreach (RoomLayer layer in this.layers)
            {
                layer.map.ClearAllTiles();
            }
            this.rooms.Clear();
            this.spawn = null;
            this.final = null;
            this.SpawnRoom(Vector2Int.zero);
            this.SelectFinalRoom();
            this.MarkCriticalPath();
        } while (!this.IsDungeonValid());

        StartCoroutine("DrawTiles");
    }

    private void SpawnRoom(Vector2Int position, Room previousRoom = null)
    {
        Room alreadyExists;
        if (this.rooms.TryGetValue(position, out alreadyExists))
        {
            return;
        }
        
        List<Room> allowedPrefabs = this.roomPrefabs;
        Room roomUp, roomDown, roomRight, roomLeft = null;
        Vector2Int positionUp = position + Vector2Int.up * ROOM_SIZE;
        Vector2Int positionRight = position + Vector2Int.right * ROOM_SIZE;
        Vector2Int positionBelow = position + Vector2Int.down * ROOM_SIZE;
        Vector2Int positionLeft = position + Vector2Int.left * ROOM_SIZE;
        this.rooms.TryGetValue(positionRight, out roomRight);
        this.rooms.TryGetValue(positionLeft, out roomLeft);
        this.rooms.TryGetValue(positionUp, out roomUp);
        this.rooms.TryGetValue(positionBelow, out roomDown);
        
        // dont connect to nearby rooms with out a passage to this one
        if (roomRight && !roomRight.isLeftOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isRightOpen).ToList();
        }
        if (roomLeft && !roomLeft.isRightOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isLeftOpen).ToList();
        }
        if (roomUp && !roomUp.isDownOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isUpOpen).ToList();
        }
        if (roomDown && !roomDown.isUpOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => !r.isDownOpen).ToList();
        }
        
        // force connection to nearby rooms with a passage to this one
        if (roomRight && roomRight.isLeftOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isRightOpen).ToList();
        }
        if (roomLeft && roomLeft.isRightOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isLeftOpen).ToList();
        }
        if (roomUp && roomUp.isDownOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isUpOpen).ToList();
        }
        if (roomDown && roomDown.isUpOpen)
        {
            allowedPrefabs = allowedPrefabs.Where(r => r.isDownOpen).ToList();
        }
        
        // if reached min room count start closing off the map by not generating new room spawn points
        if (this.rooms.Count >= this.minRooms)
        {
            if (!roomRight)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isRightOpen).ToList();
            }
            if (!roomLeft)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isLeftOpen).ToList();
            }
            if (!roomUp)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isUpOpen).ToList();
            }
            if (!roomDown)
            {
                allowedPrefabs = allowedPrefabs.Where(r => !r.isDownOpen).ToList();
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
        roomInstance.PassTilemaps(this.layers);
        this.rooms.Add(position, roomInstance);
        
        if (!previousRoom)
        {
            roomInstance.isSpawn = true;
            this.spawn = roomInstance;
        }

        // check to see if other rooms need to be spawned
        if (randomRoomPrefab.isUpOpen && !roomUp)
        {
            this.SpawnRoom(positionUp, roomInstance);
        }
        if (randomRoomPrefab.isRightOpen && !roomRight)
        {
            this.SpawnRoom(positionRight, roomInstance);
        }
        if (randomRoomPrefab.isDownOpen && !roomDown)
        {
            this.SpawnRoom(positionBelow, roomInstance);
        }
        if (randomRoomPrefab.isLeftOpen && !roomLeft)
        {
            this.SpawnRoom(positionLeft, roomInstance);
        }
    }

    private void SelectFinalRoom()
    {
        List<KeyValuePair<Vector2Int, Room>> possibleFinalRoom = this.rooms.Where(r => r.Value.isEnd && r.Value.distanceFromSpawn >= this.minDistanceForFinalRoom).ToList();
        if (possibleFinalRoom.Count > 0)
        {
            Room room = possibleFinalRoom.ElementAt(Random.Range(0, possibleFinalRoom.Count)).Value;
            room.isBoss = true;
            this.final = room;
        }
    }

    private void MarkCriticalPath()
    {
        if (!this.final) return;
            
        Room criticalRoom = this.final.previousRoom;
        do
        {
            criticalRoom.isOnCriticalPath = true;
            criticalRoom = criticalRoom.previousRoom;
        } while (criticalRoom.distanceFromSpawn > 0);
    }

    private IEnumerator DrawTiles()
    {
        foreach (KeyValuePair<Vector2Int, Room> room in this.rooms)
        {
            room.Value.DrawRoomAt(room.Key);
            yield return null;
        }
    }
}
