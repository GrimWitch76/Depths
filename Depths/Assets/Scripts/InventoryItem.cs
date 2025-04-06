using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "My Assets/ItemData")]
public class InventoryItem : ScriptableObject
{
    public string ItemName;
    public Sprite ItemImage;
    public int ItemValue;
}
