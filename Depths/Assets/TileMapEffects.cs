using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapEffects : MonoBehaviour
{
    [SerializeField] Tilemap _tM;

    public void SetTile(Vector3Int cords, TileBase image)
    {
        // Update visual tilemap
        Vector3Int localCell = new Vector3Int(cords.x, cords.y, 0);
        _tM.SetTile(localCell, image); // remove the tile
    }
}
