using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemValue;

    public void Setup(InventoryItem item)
    {
        _itemImage.sprite = item.ItemImage;
        _itemName.text = item.ItemName;
        _itemValue.text = "$ " + item.ItemValue;  
    }
}
