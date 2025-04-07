using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

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

    public bool AdvancedFlashLightUnlocked => _advancedFlashLightUnlocked;
    public bool SonarUpgrade => _sonarUnlocked;
    public bool ThermalProtectionUpgrade => _thermalInsulationUnlocked;
    public bool BlastProtectionUpgrade => _blastProtectionUnlocked;

    private List<InventoryItem> _inventory;


    [SerializeField] float _drillSpeedMultiplier;
    [SerializeField] float _enginePower;
    [SerializeField] float _maxFuel;
    [SerializeField] float _passiveDrainRate;
    [SerializeField] float _movementDrainRateMod;
    [SerializeField] float _sonarCoolDown;
    [SerializeField] int _minSpeedForDamage;
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentArmour;
    [SerializeField] int _inventorySlots;
    [SerializeField] int _money;
    [SerializeField] int _lightEnableDepth;


    [SerializeField] Light _smallLight;
    [SerializeField] Light _spotLight;
    [SerializeField] SonarPulse _sonarPulse;
    [SerializeField] DrillShipAnimation _animation;
    [SerializeField] AudioSource _emergancyWarpSound;
    [SerializeField] AudioSource _sellSound;


    float _currentFuel;
    int _currentHealth;
    private bool _isMoving = false;
    private bool _advancedFlashLightUnlocked = false;
    private bool _sonarUnlocked = false;
    private bool _thermalInsulationUnlocked = false;
    private bool _blastProtectionUnlocked = false;
    private bool _sonarOffCooldown = true;
    private bool _gameOver = false;
    private Vector3 _emergancyWarpLocation;

    private void Start()
    {
        _currentFuel = _maxFuel;
        _currentHealth = _maxHealth;
        _emergancyWarpLocation = transform.position;
    }

    public void SetIsMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    private void FixedUpdate()
    {
        if (_gameOver)
        {
            return;
        }

        float finalFuelDrain = _passiveDrainRate;
        if(_isMoving)
        {
            finalFuelDrain *= _movementDrainRateMod;
        }

        ChangeFuelLevel(finalFuelDrain *-1);

        _animation.ToggleLights(transform.position.y <= _lightEnableDepth);

        _smallLight.enabled = transform.position.y <= _lightEnableDepth;
        if(_advancedFlashLightUnlocked)
        {
            _spotLight.enabled = transform.position.y <= _lightEnableDepth;
        }
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

        if (_inventory.Count+1 >= _inventorySlots)
        {
            UIManager.Instance.ShowInventoryFull(true);
        }

        _inventory.Add(item);
        UIManager.Instance.AddInventoryItem(item);
        return true;
    }

    public void ClearInventory()
    {
        if (_inventory == null)
        {
            _inventory = new List<InventoryItem>();
        }
        _inventory.Clear();
        UIManager.Instance.ClearInventory();
        UIManager.Instance.ShowInventoryFull(false);
    }

    public void SellInventory()
    {
        int totalSellValue = 0;
        foreach (var item in _inventory)
        {
            totalSellValue += item.ItemValue;
        }
        _money += totalSellValue;
        _sellSound.Play();
        ClearInventory();
        UIManager.Instance.UpdateMoney(_money);
    }

    public void DeductMoney(int amount)
    {
        _money -= amount;
        UIManager.Instance.UpdateMoney(_money);
    }

    public void FillFuelTank()
    {
        float fillAmmount = _maxFuel - _currentFuel;
        _money -= (int)fillAmmount * 2;
        UIManager.Instance.UpdateMoney(_money);
        ChangeFuelLevel(fillAmmount);
        UIManager.Instance.ShowFuelWarning(false);
    }

    public void DamageHull(int DamageValue)
    {
        int totalDamage = DamageValue - _currentArmour - _minSpeedForDamage;
        _currentHealth-= totalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UIManager.Instance.UpdateHealth((float)_currentHealth / (float)_maxHealth);
        if(_currentHealth <= _maxHealth * 0.2f)
        {
            UIManager.Instance.ShowHealthWarning(true);
        }
        if(_currentHealth <= 0)
        {
            EmergancyWarp();
        }
    }

    public void DamageHullArmourBypass(int DamageValue)
    {
        int totalDamage = DamageValue;
        _currentHealth -= totalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UIManager.Instance.UpdateHealth((float)_currentHealth / (float)_maxHealth);
        if (_currentHealth <= 0)
        {
            EmergancyWarp();
        }
    }

    public void HealHull()
    {
        int healAmmount = _maxHealth - _currentHealth;
        _money -= (int)healAmmount * 10;
        UIManager.Instance.UpdateMoney(_money);
        _currentHealth = Mathf.Clamp(_currentHealth + healAmmount, 0, _maxHealth);
        UIManager.Instance.UpdateHealth((float)_currentHealth / (float)_maxHealth);
        UIManager.Instance.ShowHealthWarning(false);
    }

    private void ChangeFuelLevel(float change)
    {
        _currentFuel = Mathf.Clamp(_currentFuel + change, 0, _maxFuel);
        UIManager.Instance.UpdateFuel(_currentFuel/_maxFuel);
        if (_currentFuel <= _maxFuel * 0.2f)
        {
            UIManager.Instance.ShowFuelWarning(true);
        }

        if (_currentFuel <= 0)
        {
            EmergancyWarp();
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

    public void ApplyOneTimeUpgrade(OneTimeUpgradeType upgradeType)
    {
        _animation.EnableUpgrade(upgradeType); 
        switch (upgradeType)
        {
            case OneTimeUpgradeType.FlashLight:
                _advancedFlashLightUnlocked = true;
                break;
            case OneTimeUpgradeType.Sonar:
                _sonarUnlocked = true;
                UIManager.Instance.UnlockSonar();
                break;
            case OneTimeUpgradeType.ThermalInsulation:
                _thermalInsulationUnlocked = true;
                break;
            case OneTimeUpgradeType.BlastProtection:
                _blastProtectionUnlocked = true;
                break;
            default:
                break;
        }
    }

    public void TrySonarPing()
    {
        if(!_sonarUnlocked || !_sonarOffCooldown)
        {
            return;
        }
        
        _sonarOffCooldown = false;
        WorldStateManager.Instance.SonarPing(transform.position);
        _sonarPulse.StartPulse();
        StartCoroutine(SonarCoolDown());
    }

    private IEnumerator SonarCoolDown()
    {
        float timer = 0;
        while(timer <= _sonarCoolDown)
        {
            UIManager.Instance.SonarCoolDown(timer / _sonarCoolDown);
            timer += Time.deltaTime;
            yield return null;
        }
        _sonarOffCooldown = true; 
    }

    private void EmergancyWarp()
    {
        _currentFuel = _maxFuel / 2;
        _currentHealth = _maxHealth / 2;
        _emergancyWarpSound.Play();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        transform.position = _emergancyWarpLocation;

        UIManager.Instance.ToggleEmergancyWarp();
        UIManager.Instance.UpdateHealth(0.5f);
        UIManager.Instance.UpdateFuel(0.5f);
        UIManager.Instance.ShowInventoryFull(false);
        UIManager.Instance.ShowFuelWarning(false);
        UIManager.Instance.ShowHealthWarning(false);
        ClearInventory();
        StartCoroutine(CloseEmergancyWarpText());
    }

    private IEnumerator CloseEmergancyWarpText()
    {
        yield return new WaitForSeconds(3f);
        UIManager.Instance.ToggleEmergancyWarp();
    }
}
