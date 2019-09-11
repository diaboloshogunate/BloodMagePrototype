using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public bool isUpOpen;
    public bool isRightOpen;
    public bool isDownOpen;
    public bool isLeftOpen;
    public bool isEnd
    {
        get
        {
            if (this.isUpOpen && !this.isRightOpen && !this.isDownOpen && !this.isLeftOpen) return true;
            if (!this.isUpOpen && this.isRightOpen && !this.isDownOpen && !this.isLeftOpen) return true;
            if (!this.isUpOpen && !this.isRightOpen && this.isDownOpen && !this.isLeftOpen) return true;
            if (!this.isUpOpen && !this.isRightOpen && !this.isDownOpen && this.isLeftOpen) return true;
            return false;
        }
    }

    public bool isSpawn;
    public bool isBoss;
    public bool isOnCriticalPath { set; get; }
    public int distanceFromSpawn { set; get; }
    public Room previousRoom { set; get; }

    public List<RoomTiles> tiles = new List<RoomTiles>();

    void OnDrawGizmos()
    {
        if (this.isSpawn || this.isBoss || this.isOnCriticalPath)
        {
            if(this.isSpawn) Gizmos.color = Color.green;
            if(this.isBoss) Gizmos.color = Color.red;
            if(this.isOnCriticalPath) Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + new Vector3(6.5f, 6.5f, 0), 1);
        }
    }

    public void DrawRoomAt(Vector2Int position)
    {
        foreach (RoomTiles layer in this.tiles)
        {
            layer.DrawTiles(position);
        }
    }

    public void ClearRoom()
    {
        foreach (RoomTiles layer in this.tiles)
        {
            layer.ClearTiles();
        }
    }

    public void PassTilemaps(List<RoomLayer> layers)
    {
        foreach (RoomLayer layer in layers)
        {
            foreach (RoomTiles tileLayer in this.tiles)
            {
                if (tileLayer.layer == layer.layer)
                {
                    tileLayer.map = layer.map;
                }
            }
        }
    }
}
