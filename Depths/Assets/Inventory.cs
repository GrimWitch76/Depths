using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject _inventoryRoot;
    [SerializeField] GameObject _invObjectPrefab;

    [SerializeField] RectTransform _scrollArea;
    [SerializeField] RectTransform _scrollBar; //Going to hide the scroll bar when not needed
    [SerializeField] TextMeshProUGUI _inventoryCapacityText;

    public void ToggleInventory()
    {
        _inventoryRoot.SetActive(!_inventoryRoot.activeInHierarchy);
    }

    public void HideInventory()
    {
        _inventoryRoot.SetActive(false);
    }

    public void AddMultipleItems(InventoryItem[] items)
    {
        ClearInventoryUI();
        foreach (var item in items)
        {
            AddInventoryItem(item);
        }
    }

    public void AddInventoryItem(InventoryItem item)
    {
        InstantiateInventoryItem(item);
        if(_scrollArea.transform.childCount > 20)
        {
            _scrollBar.gameObject.SetActive(true);
        }
        else
        {
            _scrollBar.gameObject.SetActive(false);
        }
        UpdateInventoryCount();
    }
    

    private void InstantiateInventoryItem(InventoryItem item)
    {
        InventoryItemUI uiItem = Instantiate<GameObject>(_invObjectPrefab, _scrollArea).GetComponent<InventoryItemUI>();
        uiItem.Setup(item);
    }

    public void ClearInventoryUI()
    {
        foreach (Transform child in _scrollArea)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateInventoryCount()
    {
        _inventoryCapacityText.text = WorldStateManager.Instance.DrillShip.InventorySlots + "/" + _scrollArea.transform.childCount.ToString();
    }
}
