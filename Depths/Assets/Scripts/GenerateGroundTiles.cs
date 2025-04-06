using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class GenerateGroundTiles : MonoBehaviour
{
    [SerializeField] private int _worldWidth;
    [SerializeField] private int _worldDepth; //TODO move this to a data object containing world gen data
    [SerializeField] public int _chunkSize;

    [SerializeField] private TileGenerationRules _defaultTile;

    [SerializeField] private Transform _worldRoot;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private List<TileGenerationRules> _genRules;

    public Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>> GenerateWorld()
    {
        //Spit world into chunks
        int chunksPerWidth = _worldWidth/_chunkSize;
        int chunksPerDepth = _worldDepth/_chunkSize;
        var data = new Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>>();
        for (int y = 0;  y < chunksPerDepth; y++)
        {
            for (int x = 0; x < chunksPerWidth; x++)
            {
                //TODO CHANGE
                var chunkData = GenerateChunk(new Vector2Int(x* _chunkSize, y*_chunkSize));

                //Add chunk data to world
                data.Add(new Vector2Int(x, y), chunkData);

            }
        }
        return data;
    }

    public Dictionary<Vector2Int, Tilemap> GenerateVisuals(Dictionary<Vector2Int, Dictionary<Vector2Int, TileData>> worldData)
    {
        Dictionary<Vector2Int, Tilemap> _worldVisualGridsData = new Dictionary<Vector2Int, Tilemap>();
        foreach (var chunk in worldData)
        {
            Transform grid = Instantiate(_chunkPrefab, _worldRoot);
            grid.transform.position = new Vector3Int(chunk.Key.x * _chunkSize, chunk.Key.y * _chunkSize * -1, 0);
            Tilemap tileMap = grid.GetComponent<Tilemap>();
            _worldVisualGridsData.Add(new Vector2Int(chunk.Key.x * _chunkSize, chunk.Key.y * _chunkSize * -1), tileMap);

            foreach (var tile in chunk.Value)
            {
                Vector3Int tilePos = new Vector3Int(
                    tile.Key.x,
                    tile.Key.y * -1,
                    0);
                tileMap.SetTile(tilePos, tile.Value.tmpSprite);
            }
        }
        return _worldVisualGridsData;
    }


    private Dictionary<Vector2Int, TileData> GenerateChunk(Vector2Int chunkCord)
    {
        Dictionary <Vector2Int, TileData > _chunkData = new Dictionary<Vector2Int, TileData>();

        int chunkWorldY = chunkCord.y;

        for (int x = 0; x < _chunkSize; x++)
        {
            for(int y = 0; y < _chunkSize; y++)
            {
                int globalY = chunkWorldY + y; // Chunk start plus current y pos in chunk;
                TileGenerationRules selectedRule = PickTileForDepth(globalY);

                //This will pull from an array of valid tile data in each world gen data chunk. For now lets manually set it
                TileData data = new TileData();
                data.hardness = selectedRule.type.hardness;
                data.isBroken = false;
                data.tmpSprite = selectedRule.type.tileImage;
                data.valuable = selectedRule.type.containedValuable;
                Vector2Int tilePos = new Vector2Int(x,y);
                _chunkData.Add(tilePos, data);
            }
        }
        return _chunkData;
    }

    private TileGenerationRules PickTileForDepth(int globalY)
    {
        //Make list of valid possible tiles based on depth
        List<TileGenerationRules> validTiles = _genRules
            .Where(rule => globalY >= rule.minDepth && globalY <= rule.maxDepth)
            .ToList();
        
        float totalChance = validTiles.Sum(r => r.spawnChance);
        float pick = UnityEngine.Random.Range(0f, totalChance);

        float accumulator = 0f;
        foreach (var rule in validTiles)
        {
            accumulator += rule.spawnChance;
            if (pick <= accumulator)
                return rule;
        }

        return _defaultTile; // fallback
    }

}
