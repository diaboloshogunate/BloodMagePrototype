using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isOpenUp = true;
    public bool isOpenRight = true;
    public bool isOpenDown = true;
    public bool isOpenLeft = true;
    public bool isSpawn = false;
    public bool isBoss = false;
    public bool canBePlacedOnCriticalPath = true;
    public bool isOnCriticalPath = false;
    public Transform spawnPoint;
    public Room previousRoom;
    public int distanceFromSpawn;
    
    public int numberOfPaths {
        get
        {
            int i = 0;
            if (this.isOpenUp) i++;
            if (this.isOpenRight) i++;
            if (this.isOpenDown) i++;
            if (this.isOpenLeft) i++;
            
            return i; 
        }
    }

    private void DrawDebugSphere(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(this.transform.position + new Vector3(Generator.ROOM_SIZE/2f, Generator.ROOM_SIZE/2f, 0), 1);
    }

    private void DrawDebugPath(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(this.transform.position + new Vector3(Generator.ROOM_SIZE/2f, Generator.ROOM_SIZE/2f, 0), this.previousRoom.transform.position + new Vector3(Generator.ROOM_SIZE/2f, Generator.ROOM_SIZE/2f, 0));
    }

    public void OnDrawGizmos()
    {
        if (this.isSpawn)
        {
            this.DrawDebugSphere(Color.green);
        }
        else if (this.isBoss)
        {
            this.DrawDebugSphere(Color.red);
            this.DrawDebugPath(Color.yellow);
        }
        else if (this.isOnCriticalPath)
        {
            this.DrawDebugPath(Color.yellow);
        }
    }
}
