using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="TileType", menuName ="Grim/CustomData/"), Serializable]
public class TileType : ScriptableObject
{
    public string tileName;
    public TileBase tileImage;
}
