
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/TerrainGenData")]
public class TileGenerationRules : ScriptableObject
{
    public TileType type;
    public int minDepth;
    public int maxDepth;
    [Range(0, 1)] public float spawnChance; // 0–1
}
