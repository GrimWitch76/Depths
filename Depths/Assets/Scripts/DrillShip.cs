using UnityEngine;
using UnityEngine.Rendering;

public class DrillShip : MonoBehaviour
{
    public float DrillSpeedMultiplier => _drillSpeedMultiplier;
    public float EnginePower => _enginePower;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int InventorySlots => _inventorySlots;



    [SerializeField] float _drillSpeedMultiplier;
    [SerializeField] float _enginePower;
    [SerializeField] int _maxHealth;
    [SerializeField] int _inventorySlots;
    int _currentHealth;
}
