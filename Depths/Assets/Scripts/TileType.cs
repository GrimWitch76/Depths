using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "My Assets/TileData")]
public class TileType : ScriptableObject
{
    public string tileName;
    public TileBase tileImage;
    public InventoryItem containedValuable;
    public float hardness;
    public Light Light;
}
