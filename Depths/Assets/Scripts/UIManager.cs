using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Inventory _inventory;
    [SerializeField] private Inventory _upgradeScreen;
    [SerializeField] private RectTransform _sellPrompt;
    [SerializeField] private RectTransform _emergancyWarpPopup;
    [SerializeField] private RectTransform _fuelPrompt;
    [SerializeField] private RectTransform _repairPrompt;
    [SerializeField] private RectTransform _upgradePrompt;
    [SerializeField] private RectTransform _InventoryFull;
    [SerializeField] private RectTransform _LowFuel;
    [SerializeField] private RectTransform _LowHealth;
    [SerializeField] private RectTransform _quitGameScreen;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _depthText;

    [SerializeField] private Image _healthBarFill;
    [SerializeField] private Image _fuelBarFill;

    [SerializeField] private Image _sonarCoolDownFill;
    [SerializeField] private RectTransform _sonarTransform;


    [SerializeField] private float countSpeed = 500f;
    [SerializeField] private float sonarRemaining = 0;

    private int displayedValue = 0;
    private int targetValue = 0;
    private bool _sonarUnlocked = false;
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
    public void ShowRepairPrompt()
    {
        _repairPrompt.gameObject.SetActive(true);
    }

    public void HideRepairPrompt()
    {
        _repairPrompt.gameObject.SetActive(false);
    }

    public void ShowUpgradePrompt()
    {
        _upgradePrompt.gameObject.SetActive(true);
    }

    public void HideUpgradePrompt()
    {
        _upgradePrompt.gameObject.SetActive(false);
    }

    public void ToggleUpgradePrompt()
    {
        _upgradeScreen.gameObject.SetActive(!_upgradeScreen.gameObject.activeInHierarchy);
    }

    public void ToggleEmergancyWarp()
    {
        _emergancyWarpPopup.gameObject.SetActive(!_emergancyWarpPopup.gameObject.activeInHierarchy);
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
        targetValue = money;
    }

    public void UpdateDepth(int depth)
    {
        _depthText.text = depth.ToString() + " M";
    }

    public void ShowInventoryFull(bool show)
    {
        _InventoryFull.gameObject.SetActive(show);
    }
    public void ShowFuelWarning(bool show)
    {
        _LowFuel.gameObject.SetActive(show);
    }
    public void ShowHealthWarning(bool show)
    {
        _LowHealth.gameObject.SetActive(show);
    }

    public void UnlockSonar()
    {
        _sonarTransform.gameObject.SetActive(true);
        _sonarUnlocked = true;
    }

    public void SonarCoolDown(float percent)
    {
        sonarRemaining = percent;
    }

    public void ToggleQuitScreen()
    {
        _quitGameScreen.gameObject.SetActive(!_quitGameScreen.gameObject.activeInHierarchy);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {

        if (displayedValue != targetValue)
        {
            // Move towards the target value
            float dynamicSpeed = Mathf.Max(countSpeed, Mathf.Abs(targetValue - displayedValue) * 3f);
            displayedValue = (int)Mathf.MoveTowards(displayedValue, targetValue, dynamicSpeed * Time.deltaTime);
            _moneyText.text = $"${displayedValue:N0}"; // Adds comma formatting
        }

        if(_sonarUnlocked)
        {
            _sonarCoolDownFill.fillAmount = 1 - sonarRemaining;
        }
    }
}
