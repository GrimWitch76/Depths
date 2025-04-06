using UnityEngine;

public class DepthFade : MonoBehaviour
{
    [SerializeField] private Gradient _ambientColourByDepth;
    [SerializeField] private Light _mainLight;
    [SerializeField] private float _minDepth = 0f;
    [SerializeField] private float _maxDepth = -400f;

    private GameObject _player;
    void Update()
    {
        if(_player == null )
        {
            _player = WorldStateManager.Instance.DrillShip.transform.gameObject;
        }

        float depth = _player.transform.position.y;
        float t = Mathf.InverseLerp(_minDepth, _maxDepth, depth);
        Color ambient = _ambientColourByDepth.Evaluate(t);
        RenderSettings.ambientLight = ambient;
        _mainLight.intensity = t;
    }
}
