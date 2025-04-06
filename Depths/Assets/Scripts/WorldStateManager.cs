using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldStateManager : MonoBehaviour
{
    [SerializeField] private GenerateGroundTiles _worldGenerator;
    [SerializeField] private Grid _worldGrid;
    [SerializeField] private Tilemap _highLightEffectGrid;
    [SerializeField] private TileBase _highLightEffect;
    [SerializeField] private TileMapEffects _tileMapEffects;
    public DrillShip DrillShip;


    private Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>> _worldData;
    private Dictionary<Vector2Int, Tilemap> _worldVisualData;


    public static WorldStateManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        StartGame();
    }


    void StartGame()
    {

        _worldData = _worldGenerator.GenerateWorld();
        _worldVisualData = _worldGenerator.GenerateVisuals(_worldData);
    }

    public bool IsValidTile(Vector3Int cords)
    {
        Vector2Int chunkCoord = ChunkCordsFromWorld(cords);
        Vector2Int localTileCoord = LocalTileCordsFromWorld(cords);

        if (_worldData.TryGetValue(chunkCoord, out var chunk))
        {
            if (chunk.TryGetValue(localTileCoord, out var tile))
            {
                return true;
            }
        }
        return false;
    }

    public float GetTileHardness(Vector3Int cords)
    {
        Vector2Int chunkCoord = ChunkCordsFromWorld(cords);
        Vector2Int localTileCoord = LocalTileCordsFromWorld(cords);

        if (_worldData.TryGetValue(chunkCoord, out var chunk))
        {
            if (chunk.TryGetValue(localTileCoord, out var tile))
            {
                return tile.hardness;
            }
        }
        return -1;
    }

    public InventoryItem GetTileValuable(Vector3Int cords)
    {
        Vector2Int chunkCoord = ChunkCordsFromWorld(cords);
        Vector2Int localTileCoord = LocalTileCordsFromWorld(cords);

        if (_worldData.TryGetValue(chunkCoord, out var chunk))
        {
            if (chunk.TryGetValue(localTileCoord, out var tile))
            {
                return tile.valuable;
            }
        }
        return null;
    }


    public void RemoveTile(Vector3Int cords)
    {
        Vector2Int chunkCoord = ChunkCordsFromWorld(cords);
        Vector2Int localTileCoord = LocalTileCordsFromWorld(cords);

        if (_worldData.TryGetValue(chunkCoord, out var chunk))
        {
            if (chunk.TryGetValue(localTileCoord, out var tile))
            {
                // Update visual tilemap
                Tilemap tilemap = _worldVisualData[DataPosToVisualPos(chunkCoord)]; // however you're tracking them
                Vector3Int localCell = new Vector3Int(localTileCoord.x, localTileCoord.y * -1, 0);
                tilemap.SetTile(localCell, null); // remove the tile
            }
        }
    }

    public void SetTileDigEffect(Vector3Int cords, TileBase image)
    {
        _tileMapEffects.SetTile(cords, image);
    }

    public void RemoveTileDigEffect(Vector3Int cords)
    {
        _tileMapEffects.SetTile(cords, null);
    }

    public void SonarPing(Vector3 playerPos)
    {
        Vector3Int centerCell = GridPosFromWorld(playerPos);
        int radius = 10;

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                Vector3Int checkCell = new Vector3Int(centerCell.x + dx, centerCell.y + dy, 0);

                float distance = Vector2Int.Distance(new Vector2Int(dx, dy), Vector2Int.zero);
                if (distance > radius) continue;

                InventoryItem tile = GetTileValuable(checkCell); // Your method for fetching from world
                if (tile == null) continue;

                HighLightCell(checkCell); // See below
            }
        }
    }

    public Vector3Int GridPosFromWorld(Vector3 worldPos)
    {
        return _worldGrid.WorldToCell(worldPos);
    }

    private Vector2Int DataPosToVisualPos(Vector2Int dataPos)
    {
        return new Vector2Int(dataPos.x * _worldGenerator._chunkSize, dataPos.y * _worldGenerator._chunkSize);
    }

    private Vector2Int ChunkCordsFromWorld(Vector3Int coords)
    {
        return new Vector2Int(
            Mathf.FloorToInt((float)coords.x / _worldGenerator._chunkSize),
            Mathf.FloorToInt((float)-coords.y / _worldGenerator._chunkSize) // Inverted Y
        );
    }

    private Vector2Int LocalTileCordsFromWorld(Vector3Int cords)
    {
        return new Vector2Int(
            Mathf.Abs(cords.x % _worldGenerator._chunkSize),
            Mathf.Abs(cords.y % _worldGenerator._chunkSize)
        );
    }

    private TileData GetTileData(Vector3Int cords)
    {
        Vector2Int chunkCoord = ChunkCordsFromWorld(cords);
        Vector2Int localTileCoord = LocalTileCordsFromWorld(cords);

        if (_worldData.TryGetValue(chunkCoord, out var chunk))
        {
            if (chunk.TryGetValue(localTileCoord, out var tile))
            {
                return tile;
            }
        }
        return null;
    }

    private void HighLightCell(Vector3Int cords)
    {
        _highLightEffectGrid.SetTile(cords, _highLightEffect);
        StartCoroutine(HighLightTimer(cords));
    }

    private IEnumerator HighLightTimer(Vector3Int cords)
    {
        float timePassed = 0;
        while(timePassed <= 5)
        {
            yield return null;
            timePassed += Time.deltaTime;
        }
        _highLightEffectGrid.SetTile(cords, null);
    }
}
