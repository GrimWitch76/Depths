using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateGroundTiles : MonoBehaviour
{
    [SerializeField] private int _worldWidth;
    [SerializeField] private int _worldDepth; //TODO move this to a data object containing world gen data
    [SerializeField] private int _chunkSize;

    [SerializeField] private TileBase _defaultTileImage;

    [SerializeField] private Transform _worldRoot;
    [SerializeField] private Transform _chunkPrefab;

    private Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>> _worldData;
    private Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>> _worldVisualGrids;

    private void Awake()
    {
        if( _worldData == null )
            _worldData = new Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>>();
}
    // Generate world
    private void Start()
    {
        GenerateWorld();
        GenerateVisuals();
    }

    public void GenerateWorld()
    {
        //Spit world into chunks
        int chunksPerWidth = _worldWidth/_chunkSize;
        int chunksPerDepth = _worldDepth/_chunkSize;

        for (int y = 0;  y < chunksPerDepth; y++)
        {
            for (int x = 0; x < chunksPerWidth; x++)
            {
                //TODO CHANGE
                var chunkData = GenerateChunk();

                //Add chunk data to world
                _worldData.Add(new Vector2Int(x, y), chunkData);

            }
        }
    }

    public void GenerateVisuals()
    {
        foreach (var chunk in _worldData)
        {
            GenerateGrids(chunk.Key, chunk.Value);
        }
    }


    private Dictionary<Vector2Int, TileData> GenerateChunk()
    {
        Dictionary <Vector2Int, TileData > _chunkData = new Dictionary<Vector2Int, TileData>();
        for (int x = 0; x < _chunkSize; x++)
        {
            for(int y = 0; y < _chunkSize; y++)
            {
                //This will pull from an array of valid tile data in each world gen data chunk. For now lets manually set it
                TileData data = new TileData();
                data.durability = 1;
                data.isBroken = false;
                data.tmpSprite = _defaultTileImage;
                Vector2Int tilePos = new Vector2Int(x,y);
                _chunkData.Add(tilePos, data);
            }
        }
        return _chunkData;
    }

    private void GenerateGrids(Vector2Int chunkOrigin, Dictionary<Vector2Int, TileData> chunkData)
    {
        //Generate new grid
        Transform grid = Instantiate(_chunkPrefab, _worldRoot);
        grid.transform.position = new Vector3Int(chunkOrigin.x * _chunkSize, chunkOrigin.y * _chunkSize * -1, 0);
        Tilemap tileMap = grid.GetComponent<Tilemap>();

        foreach (var tile in chunkData)
        {
            Vector3Int tilePos = new Vector3Int(
                tile.Key.x,
                tile.Key.y *-1,
                0);
            tileMap.SetTile(tilePos, tile.Value.tmpSprite);
        }
    }
}
