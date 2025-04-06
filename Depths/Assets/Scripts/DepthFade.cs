using UnityEngine;

public class DepthFade : MonoBehaviour
{
    [SerializeField] private Gradient _ambientColourByDepth;
    [SerializeField] private Gradient _redLevelsColourByDepth;
    [SerializeField] private Light _mainLight;
    [SerializeField] private float _minDepth = 0f;
    [SerializeField] private float _maxDepth = -400f;


    [SerializeField] private float _minDepthRed = -800f;
    [SerializeField] private float _maxDepthRed = -950f;

    private GameObject _player;
    void Update()
    {
        if(_player == null )
        {
            _player = WorldStateManager.Instance.DrillShip.transform.gameObject;
        }

        float depth = _player.transform.position.y;

        if(depth > _minDepthRed)
        {
            float t = Mathf.InverseLerp(_minDepth, _maxDepth, depth);
            Color ambient = _ambientColourByDepth.Evaluate(t);
            RenderSettings.ambientLight = ambient;
            _mainLight.intensity = 1 - t;
            _mainLight.color = ambient;
        }
        else
        {
            float t = Mathf.InverseLerp(_minDepthRed, _maxDepthRed, depth);
            Color ambient = _redLevelsColourByDepth.Evaluate(t);
            RenderSettings.ambientLight = ambient;
            _mainLight.intensity = t;
            _mainLight.color = ambient;
        }
    }
}
