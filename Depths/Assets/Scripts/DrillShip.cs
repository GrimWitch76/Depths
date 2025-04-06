using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DrillShip : MonoBehaviour
{
    public float DrillSpeedMultiplier => _drillSpeedMultiplier;
    public float EnginePower => _enginePower;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int InventorySlots => _inventorySlots;
    public int Money => _money;

    private List<InventoryItem> _inventory;


    [SerializeField] float _drillSpeedMultiplier;
    [SerializeField] float _enginePower;
    [SerializeField] int _maxHealth;
    [SerializeField] int _inventorySlots;
    [SerializeField] int _money;
    int _currentHealth;

    public bool TryAddItemToInventory(InventoryItem item)
    {
        if (_inventory == null)
        {
            _inventory = new List<InventoryItem>();
        }

        if(_inventory.Count >= _inventorySlots)
        {
            return false;
        }

        _inventory.Add(item);
        UIManager.Instance.AddInventoryItem(item);
        return true;
    }

    public void ClearInventory()
    {
        _inventory.Clear();
    }

    public void SellInventory()
    {
        int totalSellValue = 0;
        foreach (var item in _inventory)
        {
            totalSellValue += item.ItemValue;
        }
        _money += totalSellValue;
        ClearInventory();
    }
}
