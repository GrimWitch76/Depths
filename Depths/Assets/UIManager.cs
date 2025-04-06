using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Inventory _inventory;
    [SerializeField] private RectTransform _sellPrompt;

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
    }

    public void AddInventoryItem(InventoryItem item)
    {
        _inventory.AddInventoryItem(item);
    }

    public void AddInventoryItems(InventoryItem[] item)
    {
        _inventory.AddMultipleItems(item);
    }

    public void ClearInventory()
    {
        _inventory.ClearInventoryUI();
    }

    public void ToggleUIVisiblity()
    {
        _inventory.ToggleInventory();
    }

    public void ShowSellPrompt()
    {
        _sellPrompt.gameObject.SetActive(true);
    }

    public void HideSellPrompt()
    {
        _sellPrompt.gameObject.SetActive(false);
    }

}
