using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateGroundTiles : MonoBehaviour
{
    [SerializeField] private int _worldWidth;
    [SerializeField] private int _worldDepth; //TODO move this to a data object containing world gen data
    [SerializeField] public int _chunkSize;

    [SerializeField] private TileGenerationRules _defaultTile;
    [SerializeField] private TileGenerationRules _airTile;
    [SerializeField] private TileGenerationRules _borderTile;
    [SerializeField] private TileType _artifactTile;
    [SerializeField] private TileType _roomBorderTile;

    [SerializeField] private Transform _worldRoot;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private Transform _backgroundChunkPrefab;
    [SerializeField] private Light _lightPrefab;
    [SerializeField] private List<TileGenerationRules> _genRules;

    [SerializeField] private int _roomWidth;
    [SerializeField] private int _roomHeight;


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
                //TODO CHANGE TO GENERATE AS THE GAME RUNS? 
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

            Transform backgrid = Instantiate(_backgroundChunkPrefab, _worldRoot);
            backgrid.transform.position = new Vector3Int(chunk.Key.x * _chunkSize, chunk.Key.y * _chunkSize * -1, 1);
            tileMap = backgrid.GetComponent<Tilemap>();

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
                int globalX = chunkCord.x + x; // Chunk start plus current y pos in chunk;

                bool isBorder =
                    (globalX == 0 || globalX == _worldWidth - 1) ||
                    (globalY == _worldDepth - 1);

                TileData data = new TileData();
                Vector2Int tilePos = new Vector2Int(x, y);

                if (isBorder)
                {
  
                    data.hardness = _borderTile.type.hardness;
                    data.isBroken = false;
                    data.tmpSprite = _borderTile.type.tileImage;
                    data.valuable = _borderTile.type.containedValuable;
                    data.isExplosive = _borderTile.type.IsExplosive;
                    data.damageOnContact = _borderTile.type.damageOnContact;
                }
                else
                {
                    TileGenerationRules selectedRule = PickTileForDepth(globalY);

                    //This will pull from an array of valid tile data in each world gen data chunk. For now lets manually set it
                    data.hardness = selectedRule.type.hardness;
                    data.isBroken = false;
                    data.tmpSprite = selectedRule.type.tileImage;
                    data.valuable = selectedRule.type.containedValuable;
                    data.isExplosive = selectedRule.type.IsExplosive;
                    data.damageOnContact = selectedRule.type.damageOnContact;

                    if (selectedRule.type.Light != null)
                    {
                        Light newLight = GameObject.Instantiate<Light>(selectedRule.type.Light);
                        newLight.name = selectedRule.name;
                        newLight.transform.position = new Vector3(globalX + (.5f), (globalY - .5f) * -1, -0.25f);
                        data.light = newLight;
                    }
                }

             
                _chunkData.Add(tilePos, data);
            }
        }
        return _chunkData;
    }

    public void GenerateBottomRoom()
    {
        int startX = (_worldWidth / 2) - (_roomWidth / 2);
        int startY = _worldDepth - _roomHeight; // bottom of world

        for (int x = startX; x < startX + _roomWidth; x++)
        {
            for (int y = startY; y < startY + _roomHeight; y++)
            {
                Vector3Int WorldPos = new Vector3Int(x, -y, 0);
                // Set tile to empty space
                Vector2Int chunkCoord = WorldStateManager.Instance.ChunkCordsFromWorld(WorldPos);
                Vector2Int localPos = WorldStateManager.Instance.LocalTileCordsFromWorld(WorldPos);

                if (WorldStateManager.Instance._worldData.TryGetValue(chunkCoord, out var chunk) &&
                    chunk.ContainsKey(localPos))
                {
                    TileData data = new TileData();
                    if ((x == startX || x == startX + _roomWidth -1) || y == startY + _roomHeight -1)
                    {
                        data.hardness = -1;
                        data.isBroken = false;
                        data.tmpSprite = _roomBorderTile.tileImage;
                        data.valuable = null;
                        data.isExplosive = _borderTile.type.IsExplosive;
                        data.damageOnContact = _borderTile.type.damageOnContact;
                    }
                    else
                    {
                        data.hardness = -1;
                        data.isBroken = false;
                        data.tmpSprite = null;
                        data.valuable = null;
                        data.isExplosive = false;
                        data.damageOnContact = 0;
                    }

                    chunk[localPos] = data;
                    WorldStateManager.Instance._worldData[chunkCoord] = chunk;
                }

            }
        }
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
 