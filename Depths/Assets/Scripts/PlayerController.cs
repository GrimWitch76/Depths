using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputSettings;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _playerRB;
    [SerializeField] float _movementForce;
    [SerializeField] float _flightForce;
    [SerializeField] Dig _dig;
    [SerializeField] Transform _drillRoot;

    private Collider2D _drillCollider;
    private Vector2 _playerMovementInput = Vector2.zero;
    private bool _canSell = false;
    private bool _canRefuel = false;
    private bool _canRepair = false;
    private bool _canUpgrade = false;

    private void Start()
    {
        _drillCollider = _dig.GetComponent<Collider2D>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            _playerMovementInput += Vector2.left;
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            _playerMovementInput -= Vector2.left;
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            _playerMovementInput += Vector2.right;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _playerMovementInput -= Vector2.right;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _playerMovementInput += Vector2.up;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            _playerMovementInput -= Vector2.up;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _playerMovementInput += Vector2.down;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _playerMovementInput -= Vector2.down;
        }

        _dig.SetDigOffset(_playerMovementInput);
        if (_playerMovementInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(Mathf.Clamp(_playerMovementInput.y, -1, 0), _playerMovementInput.x) * Mathf.Rad2Deg;
            _drillRoot.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.ToggleUIVisiblity();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            WorldStateManager.Instance.DrillShip.TrySonarPing();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(_canSell)
            {
                WorldStateManager.Instance.DrillShip.SellInventory();
                _canSell = false;
                UIManager.Instance.HideSellPrompt();
                return;
            }

            if(_canRefuel)
            {
                _canRefuel = false;
                WorldStateManager.Instance.DrillShip.FillFuelTank();
                UIManager.Instance.HideFuelPrompt();
                return;
            }

            if (_canUpgrade)
            {
                UIManager.Instance.ToggleUpgradePrompt();
                return;
            }

            if(_canRepair)
            {
                _canRepair = false;
                WorldStateManager.Instance.DrillShip.HealHull();
                UIManager.Instance.HideRepairPrompt();
                return;
            }
        }
    }

    private void FixedUpdate()
    {

        if (_playerMovementInput == Vector2.zero)
        {
            _playerRB.linearDamping = 1f;
        }
        else
        {
            _playerRB.linearDamping = 0.0f;
        }

        if (_playerMovementInput == Vector2.up)
        {
            _drillCollider.enabled = false;
            _playerRB.AddForce(_playerMovementInput * _flightForce * WorldStateManager.Instance.DrillShip.EnginePower, ForceMode2D.Force);
        }
        else
        {
            _drillCollider.enabled = true;
            _playerRB.AddForce(_playerMovementInput * _movementForce * WorldStateManager.Instance.DrillShip.EnginePower, ForceMode2D.Force);
        }


        UIManager.Instance.UpdateDepth((int)transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 velocity = collision.relativeVelocity;

        if(velocity.magnitude > WorldStateManager.Instance.DrillShip.MinSpeedForDamage)
        {
            WorldStateManager.Instance.DrillShip.DamageHull((int)Mathf.Floor(velocity.magnitude));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Factory"))
        {
            _canSell = true;
            UIManager.Instance.ShowSellPrompt();
        }

        if (collision.CompareTag("FuelDepot"))
        {
            _canRefuel = true;
            UIManager.Instance.ShowFuelPrompt();
        }

        if (collision.CompareTag("UpgradeShop"))
        {
            _canUpgrade = true;
            UIManager.Instance.ShowUpgradePrompt();
        }

        if (collision.CompareTag("RepairDepot"))
        {
            _canRepair = true;
            UIManager.Instance.ShowRepairPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Factory"))
        {
            _canSell = false;
            UIManager.Instance.HideSellPrompt();
        }

        if (collision.CompareTag("FuelDepot"))
        {
            _canRefuel = false;
            UIManager.Instance.HideFuelPrompt();
        }

        if (collision.CompareTag("UpgradeShop"))
        {
            _canUpgrade = false;
            UIManager.Instance.HideUpgradePrompt();
        }

        if (collision.CompareTag("RepairDepot"))
        {
            _canRepair = false;
            UIManager.Instance.HideRepairPrompt();
        }
    }
}
