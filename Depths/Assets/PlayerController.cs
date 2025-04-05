using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _playerRB;
    [SerializeField] float _movementForce;
    [SerializeField] Dig _dig;
    [SerializeField] Transform _drillRoot;
    private Vector2 _playerMovementInput = Vector2.zero;
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
            float angle = Mathf.Atan2(_playerMovementInput.y, _playerMovementInput.x) * Mathf.Rad2Deg;
            _drillRoot.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void FixedUpdate()
    {
        _playerRB.AddForce(_playerMovementInput * _movementForce, ForceMode2D.Force);
    }



}
