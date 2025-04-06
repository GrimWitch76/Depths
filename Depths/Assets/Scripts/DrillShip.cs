using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DrillShip : MonoBehaviour
{
    public float DrillSpeedMultiplier => _drillSpeedMultiplier;
    public float EnginePower => _enginePower;
    public float CurrentFuel => _currentFuel;
    public float MaxFuel => _maxFuel;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int InventorySlots => _inventorySlots;
    public int Money => _money;

    private List<InventoryItem> _inventory;


    [SerializeField] float _drillSpeedMultiplier;
    [SerializeField] float _enginePower;
    [SerializeField] float _maxFuel;
    [SerializeField] float _passiveDrainRate;
    [SerializeField] float _movementDrainRateMod;
    [SerializeField] int _maxHealth;
    [SerializeField] int _inventorySlots;
    [SerializeField] int _money;

    float _currentFuel;
    int _currentHealth;
    private bool _isMoving = false;

    private void Start()
    {
        _currentFuel = _maxFuel;
        _currentHealth = _maxHealth;
    }

    public void SetIsMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }

    private void FixedUpdate()
    {
        float finalFuelDrain = _passiveDrainRate;
        if(_isMoving)
        {
            finalFuelDrain *= _movementDrainRateMod;
        }

        ChangeFuelLevel(finalFuelDrain *-1);
    }

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
        UIManager.Instance.ClearInventory();
    }

    public void FillFuelTank()
    {
        ChangeFuelLevel(_maxFuel);
    }

    private void ChangeFuelLevel(float change)
    {
        _currentFuel = Mathf.Clamp(_currentFuel + change, 0, _maxFuel);
        UIManager.Instance.UpdateFuel(_currentFuel/_maxFuel);

        if(_currentFuel <= 0)
        {
            //Uh oh shit well you lost I guess?
            //TODO Add loss condition
        }
    }
}
