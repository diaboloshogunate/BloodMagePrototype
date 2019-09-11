using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomTiles
{
    private const int ROOM_SIZE = 11;
    
    [HideInInspector]
    public Tilemap map;
    
    [PropertyOrder(1)]
    [EnumToggleButtons]
    public RoomLayer.LAYERS layer;

    [ShowInInspector]
    [BoxGroup("Tiles")]
    [TableMatrix(HorizontalTitle = "X", VerticalTitle = "Y", SquareCells = true)]
    [PropertyOrder(3)]
    private TileBase[,] tiles = new TileBase[ROOM_SIZE, ROOM_SIZE];
    
    [HideInInspector]
    [SerializeField] 
    private TileBase[] serialized = new TileBase[ROOM_SIZE*ROOM_SIZE];
    
    [PropertyOrder(2)]
    [HorizontalGroup("Split", 0.5f)]
    [Button(ButtonSizes.Large)]
    private void LoadTiles()
    {
        for (int i = 0; i < this.serialized.Length; i++)
        {
            int x = i / 11;
            int y = i % 11;
            this.tiles[x, y] = this.serialized[i];
        }
    }
    
    [PropertyOrder(3)]
    [HorizontalGroup("Split", 0.5f)]
    [Button(ButtonSizes.Large)]
    void SaveTiles()
    {
        int i = 0;
        for (int x = 0; x < this.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < this.tiles.GetLength(1); y++)
            {
                this.serialized[i] = this.tiles[x, y];
                i++;
            }
        }
    }

    public void DrawTiles(Vector2Int position)
    {
        this.LoadTiles();
        for (int x = 0; x < 11; x++) 
        {
            for (int y = 0; y < 11; y++)
            {
                Vector3Int tilePosition = (Vector3Int) (position + new Vector2Int(x, ROOM_SIZE - y));
                this.map.SetTile(tilePosition, this.tiles[x, y]);
            }
        }
    }

    public void ClearTiles()
    {
        this.map.ClearAllTiles();
    }
}
