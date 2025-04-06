using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dig : MonoBehaviour
{
    [SerializeField] private DrillShip _playerShip;
    [SerializeField] private float _baseDigTime;
    [SerializeField] private TileBase[] destructionTiles;
    public Vector2 _digOffset;
    public Vector3Int _activeDiggingPos;

    private bool _isDigging = false;

    public void SetDigOffset(Vector2 digOffset)
    {
        _digOffset = digOffset;

        if(_isDigging && _digOffset == Vector2.zero)
        {
            StopAllCoroutines();
            _isDigging = false;
            WorldStateManager.Instance.RemoveTileDigEffect(_activeDiggingPos);
        }
    }

    public void TryStartDrillTile(Vector3 offset)
    {
        Vector3 worldPos = transform.position + offset;
        Vector3Int cellPos = WorldStateManager.Instance.GridPosFromWorld(worldPos);
        _activeDiggingPos = cellPos;
        if (WorldStateManager.Instance.IsValidTile(cellPos))
        {
            //Check for lava
            if(WorldStateManager.Instance.GetTileType(cellPos).damageOnContact != 0)
            {
                if (!_playerShip.ThermalProtectionUpgrade)
                {
                    _playerShip.DamageHull(WorldStateManager.Instance.GetTileType(cellPos).damageOnContact);
                }
                return;
            }

            //Check for methane
            if (WorldStateManager.Instance.GetTileType(cellPos).isExplosive)
            {
                if (!_playerShip.BlastProtectionUpgrade)
                {
                    _playerShip.DamageHull(UnityEngine.Random.Range(3, 5));
                }
                throw new NotImplementedException();
            }

            float hardness = WorldStateManager.Instance.GetTileHardness(cellPos);
            if(hardness == -1)
            {
                //Can't dig this block
                return;
            }


            _isDigging = true;
            StartCoroutine(Digging(hardness, cellPos));
        }
        
    }

    private IEnumerator Digging(float hardness, Vector3Int currentCellPos)
    {
        float totalDigTime = (_baseDigTime + hardness) / _playerShip.DrillSpeedMultiplier;
        float diggingTime = 0;

        while (_isDigging && diggingTime <= totalDigTime)
        {
            diggingTime += Time.deltaTime;
            Debug.Log("Digging: " + diggingTime + " / " + totalDigTime);

            switch ((diggingTime / totalDigTime) * 100)
            {
                case < 0:
                    WorldStateManager.Instance.SetTileDigEffect(currentCellPos, destructionTiles[0]);
                    break;
                case < 25:
                    WorldStateManager.Instance.SetTileDigEffect(currentCellPos, destructionTiles[1]);
                    break;
                case < 50:
                    WorldStateManager.Instance.SetTileDigEffect(currentCellPos, destructionTiles[2]);
                    break;
                case < 75:
                    WorldStateManager.Instance.SetTileDigEffect(currentCellPos, destructionTiles[3]);
                    break;
            }
            yield return null;
        }

        CheckForValuables(currentCellPos);
        WorldStateManager.Instance.RemoveTileDigEffect(currentCellPos);
        WorldStateManager.Instance.RemoveTile(currentCellPos);
        _isDigging = false;

    }

    private void CheckForValuables(Vector3Int currentCellPos)
    {
       var valuable = WorldStateManager.Instance.GetTileValuable(currentCellPos);
        if(valuable != null)
        {
            _playerShip.TryAddItemToInventory(valuable);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Maybe make dig colliders?
        if (_digOffset != Vector2.zero && !_isDigging)
        {
            TryStartDrillTile(_digOffset);
        }
    }
}
