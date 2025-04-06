using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;

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

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(_canSell)
            {
                WorldStateManager.Instance.DrillShip.SellInventory();
                _canSell = false;
                UIManager.Instance.HideSellPrompt();
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_playerMovementInput == Vector2.up)
        {
            _drillCollider.enabled = false;
            _playerRB.AddForce(_playerMovementInput * _flightForce, ForceMode2D.Force);
        }
        else
        {
            _drillCollider.enabled = true;
            _playerRB.AddForce(_playerMovementInput * _movementForce, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Factory"))
        {
            _canSell = true;
            UIManager.Instance.ShowSellPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Factory"))
        {
            _canSell = false;
            UIManager.Instance.HideSellPrompt();
        }
    }
}
