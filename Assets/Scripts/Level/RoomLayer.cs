using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomLayer
{
    public enum LAYERS {FLOOR, WALL}
    public LAYERS layer;
    public Tilemap map;
}
