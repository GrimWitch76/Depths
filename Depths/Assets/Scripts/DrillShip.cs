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
    public float MinSpeedForDamage => _minSpeedForDamage;
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
    [SerializeField] int _minSpeedForDamage;
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentArmour;
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
        UIManager.Instance.UpdateMoney(_money);
    }

    public void FillFuelTank()
    {
        float fillAmmount = _maxFuel - _currentFuel;
        _money -= (int)fillAmmount * 2;
        ChangeFuelLevel(fillAmmount);
    }

    public void DamageHull(int DamageValue)
    {
        int totalDamage = DamageValue - _currentArmour - _minSpeedForDamage;
        _currentHealth-= totalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UIManager.Instance.UpdateHealth((float)_currentHealth / (float)_maxHealth);
        if(_currentHealth <= 0)
        {
            //Dead :(
        }
    }

    public void HealHull()
    {
        int healAmmount = _maxHealth - _currentHealth;
        _money -= (int)healAmmount * 10;

        _currentHealth = Mathf.Clamp(_currentHealth + healAmmount, 0, _maxHealth);
        UIManager.Instance.UpdateHealth((float)_currentHealth / (float)_maxHealth);
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

    public void ApplyUpgrade(UpgradeData upgrade, int currentLevel)
    {
        switch (upgrade.type)
        {
            case UpgradeType.DrillSpeed:
                _drillSpeedMultiplier = upgrade.valuePerLevel[currentLevel];
                break;
            case UpgradeType.InventorySize:
                _inventorySlots = (int)upgrade.valuePerLevel[currentLevel];
                break;
            case UpgradeType.Health:
                _maxHealth = (int)upgrade.valuePerLevel[currentLevel];
                _currentArmour = (int)(upgrade.valuePerLevel[currentLevel] / 10);
                break;
            case UpgradeType.FuelCapacity:
                _maxFuel = (int)upgrade.valuePerLevel[currentLevel];
                break;
            case UpgradeType.MoveSpeed:
                _enginePower = upgrade.valuePerLevel[currentLevel];
                break;
            default:
                break;
        }
    }
}
