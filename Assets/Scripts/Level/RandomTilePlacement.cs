using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTilePlacement : MonoBehaviour
{
    [SerializeField] private RandomTile[] tiles;

    private void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(x,y,0), null);
                    float roll = Random.Range(0, 100)/100f;
                    float total = 0;
                    foreach (RandomTile t in this.tiles)
                    {
                        total += t.chance;
                        if (roll < total)
                        {
                            GameObject obj = Instantiate(t.tile, this.transform);
                            obj.transform.position = new Vector3(x,y,0) + new Vector3(0.5f, 0.5f) + this.gameObject.transform.position;
                            break;
                        }
                    }
                }
            }
        }        
    }
}
