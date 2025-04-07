using System.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class DrillShipAnimation : MonoBehaviour
{
    [SerializeField] Transform _mainBodyRotation;
    [SerializeField] Transform _horizontalDrill;
    [SerializeField] Transform _verticalDrill;
    [SerializeField] Transform _leftRocket;
    [SerializeField] Transform _rightRocket;

    [SerializeField] ParticleSystem _leftRocketParticles, _rightRocketParticles;
    [SerializeField] Light _leftRocketLight, _rightRocketLight;

    [SerializeField] GameObject _sonar;
    [SerializeField] GameObject _advancedLight;
    [SerializeField] GameObject[] _thermalVents;
    [SerializeField] GameObject[] _blastProtection;

    [SerializeField] Material _lightMaterial;

    [SerializeField] Vector3 _leftRocketRetractPos;
    [SerializeField] Vector3 _rightRocketRetractPos;
    [SerializeField] Vector3 _leftRocketExtendPos;
    [SerializeField] Vector3 _rightRocketExtendPos;
    [SerializeField] Vector3 _horizontalDrillExtendPos;
    [SerializeField] Vector3 _horizontalDrillRetractPos;

    [SerializeField] float _rocketExtendTime = 0.1f;
    [SerializeField] float _drillExtendTime = 0.1f;
    [SerializeField] float rotationSpeed = 10f;

    [SerializeField] float _idleVolume = 10f;
    [SerializeField] float _activeVolume = 10f;
    [SerializeField] float _engineResponsivness = 10f;

    [SerializeField] float _rocketVolume = 10f;

    [SerializeField] AudioSource _engineSounds;
    [SerializeField] AudioSource _rocketSound;


    private Vector3 lastMovementDirection = Vector3.forward;

    private Vector2 _currentMovementInput = Vector2.left;
    private bool _isFlying = false;
    private bool _isDiggingDown = false;
    private bool _isDiggingSideWays = false;
    private bool _verticalDrillExtended = false;
    private bool _horizontalDrillExtended = false;
    private bool _rocketsExtended = false;
    private bool _gameOver = false;


    public void GameOver()
    {
        _gameOver = true;
        StartCoroutine(FadeOutAudio());

    }
    public void EnableUpgrade(OneTimeUpgradeType upgrade)
    {
        switch (upgrade)
        {
            case OneTimeUpgradeType.FlashLight:
                _advancedLight.SetActive(true);
                break;
            case OneTimeUpgradeType.Sonar:
                _sonar.SetActive(true);
                break;
            case OneTimeUpgradeType.ThermalInsulation:
                foreach (var gameObject in _thermalVents)
                {
                    gameObject.SetActive(true);
                }
                break;
            case OneTimeUpgradeType.BlastProtection:
                foreach (var gameObject in _blastProtection)
                {
                    gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public void SetInput(Vector2 input)
    {
        _currentMovementInput = input;
    }

    public void SetFlying(bool flying)
    {
        _isFlying = flying;
    }
    public void SetDiggingDown(bool diggingDown)
    {
        _isDiggingDown = diggingDown;
    }
    public void SetDiggingHorizontal(bool diggingHorizontal)
    {
        _isDiggingSideWays = diggingHorizontal;
    }
    public void ToggleLights(bool lightsOn)
    {
        if(lightsOn)
        {
            _lightMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            _lightMaterial.DisableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if(_gameOver)
        {
            return;
        }

        if (Mathf.Abs(_currentMovementInput.x) > 0.01f)
        {
            //float targetYRotation = _currentMovementInput.x > 0 ? 0f : 180f;

            float angle = _currentMovementInput.x > 0 ? 180f : 0f;
            //_mainBodyRotation.rotation = Quaternion.Euler(-90f, angle, 0);

            Quaternion targetRotation = Quaternion.Euler(-90f, angle, 0f);
            _mainBodyRotation.rotation = Quaternion.Slerp(_mainBodyRotation.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if(_isFlying)
        {
            if(!_rocketsExtended)
            {
                _rocketsExtended = true;
                StartCoroutine(ExtendRockets());
            }
        }
        else
        {
            if (_rocketsExtended)
            {
                _rocketsExtended = false;
                StartCoroutine(RetractRockets());
            }
        }

        if(_isDiggingDown)
        {
            if(!_verticalDrillExtended)
            {
                _verticalDrillExtended = true;
                StartCoroutine(ExtendBottomDrill());
            }
        }
        else
        {
            if (_verticalDrillExtended)
            {
                _verticalDrillExtended = false;
                StartCoroutine(RetractBottomDrill());
            }
        }

        if (_isDiggingSideWays)
        {
            if (!_horizontalDrillExtended)
            {
                _horizontalDrillExtended = true;
                StartCoroutine(ExtendSideDrill());
            }
        }
        else
        {
            if (_horizontalDrillExtended)
            {
                _horizontalDrillExtended = false;
                StartCoroutine(RetractSideDrill());
            }
        }


        bool isMoving = _currentMovementInput.magnitude > 0.1f;

        float targetVolume = isMoving ? _activeVolume : _idleVolume;
        _engineSounds.volume = Mathf.Lerp(_engineSounds.volume, targetVolume, _engineResponsivness * Time.deltaTime);
        _engineSounds.pitch = Mathf.Lerp(_engineSounds.pitch, isMoving ? 1.2f : 1.0f, _engineResponsivness * Time.deltaTime);
    }




    private IEnumerator RetractSideDrill()
    {
        float timer = 0;
        while (timer <= _drillExtendTime)
        {
            _horizontalDrill.transform.localPosition = Vector3.Lerp(_horizontalDrillExtendPos, _horizontalDrillRetractPos, timer / _rocketExtendTime);
            timer += Time.deltaTime;
            yield return null;
        }
        _horizontalDrill.transform.localPosition = _horizontalDrillRetractPos;
    }

    private IEnumerator ExtendSideDrill()
    {
        float timer = 0;
        while (timer <= _drillExtendTime)
        {
            _horizontalDrill.transform.localPosition = Vector3.Lerp(_horizontalDrillRetractPos, _horizontalDrillExtendPos, timer / _rocketExtendTime);
            timer += Time.deltaTime;
            yield return null;
        }
        _horizontalDrill.transform.localPosition = _horizontalDrillExtendPos;

    }

    private IEnumerator RetractBottomDrill()
    {
        float timer = 0;
        float lerpValue = 0;
        while (timer <= _drillExtendTime)
        {
            lerpValue = Mathf.Lerp(1, 0, _drillExtendTime / timer);
            _verticalDrill.transform.localScale = new Vector3(_verticalDrill.transform.localScale.x, _verticalDrill.transform.localScale.y, lerpValue);
            timer += Time.deltaTime;
            yield return null;
        }
        _verticalDrill.transform.localScale = new Vector3(_verticalDrill.transform.localScale.x, _verticalDrill.transform.localScale.y, 0);
    }

    private IEnumerator ExtendBottomDrill()
    {
        float timer = 0;
        float lerpValue = 0;
        while (timer <= _drillExtendTime)
        {
            lerpValue = Mathf.Lerp(0, 1, _drillExtendTime / timer);
            _verticalDrill.transform.localScale = new Vector3(_verticalDrill.transform.localScale.x, _verticalDrill.transform.localScale.y, lerpValue);
            timer += Time.deltaTime;
            yield return null;
        }
        _verticalDrill.transform.localScale = new Vector3(_verticalDrill.transform.localScale.x, _verticalDrill.transform.localScale.y, 1);

    }

    private IEnumerator RetractRockets()
    {
        float timer = 0;
        while (timer <= _rocketExtendTime)
        {
            _rocketSound.volume = Mathf.Lerp(_rocketVolume, 0, timer / _rocketExtendTime);
            _leftRocket.transform.localPosition = Vector3.Lerp(_leftRocketExtendPos, _leftRocketRetractPos, timer / _rocketExtendTime);
            _rightRocket.transform.localPosition = Vector3.Lerp(_rightRocketExtendPos, _rightRocketRetractPos, timer / _rocketExtendTime);
            timer += Time.deltaTime;
            yield return null;
        }
        _rocketSound.volume = 0;

        _leftRocket.transform.localPosition = _leftRocketRetractPos;
        _rightRocket.transform.localPosition = _rightRocketRetractPos;

        _leftRocketLight.enabled = false;
        _rightRocketLight.enabled = false;

        _leftRocketParticles.Stop();
        _rightRocketParticles.Stop();
    }

    private IEnumerator ExtendRockets()
    {
        float timer = 0;
        while (timer <= _rocketExtendTime)
        {
            _rocketSound.volume = Mathf.Lerp(0, _rocketVolume, timer / _rocketExtendTime);
            _leftRocket.transform.localPosition = Vector3.Lerp(_leftRocketRetractPos, _leftRocketExtendPos, timer / _rocketExtendTime);
            _rightRocket.transform.localPosition = Vector3.Lerp(_rightRocketRetractPos, _rightRocketExtendPos, timer / _rocketExtendTime);
            timer += Time.deltaTime;
            yield return null;
        }

        _rocketSound.volume = _rocketVolume;
        _leftRocket.transform.localPosition = _leftRocketExtendPos;
        _rightRocket.transform.localPosition = _rightRocketExtendPos;

        _leftRocketLight.enabled = true;
        _rightRocketLight.enabled = true;

        _leftRocketParticles.Play();
        _rightRocketParticles.Play();
    }

    private IEnumerator FadeOutAudio()
    {
        float timer = 0;
        float engineStartVolume = _engineSounds.volume;
        float rocketStartVolume = _rocketSound.volume;
        while (timer <= 0.5f)
        {
            yield return null;
            timer += Time.deltaTime;
            _engineSounds.volume = Mathf.Lerp(engineStartVolume, 0, timer/ 0.5f);
            _rocketSound.volume = Mathf.Lerp(rocketStartVolume, 0, timer / 0.5f);
        }
        _engineSounds.volume = 0f;
        _rocketSound.volume = 0f;

    }
}
