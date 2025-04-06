using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Inventory _inventory;
    [SerializeField] private RectTransform _sellPrompt;
    [SerializeField] private RectTransform _fuelPrompt;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _depthText;

    [SerializeField] private Image _healthBarFill;
    [SerializeField] private Image _fuelBarFill;

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

    public void ShowFuelPrompt()
    {
        _fuelPrompt.gameObject.SetActive(true);
    }

    public void HideFuelPrompt()
    {
        _fuelPrompt.gameObject.SetActive(false);
    }

    public void UpdateHealth(float percent)
    {
        _healthBarFill.fillAmount = percent;
    }

    public void UpdateFuel(float percent)
    {
        _fuelBarFill.fillAmount = percent;
    }

    public void UpdateMoney(int money)
    {
        _moneyText.text = "$" + money;
    }

    public void UpdateDepth(int depth)
    {
        _moneyText.text = depth.ToString() + " M";
    }

}
