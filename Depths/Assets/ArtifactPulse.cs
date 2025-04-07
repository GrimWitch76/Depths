using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ArtifactPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseScale = 1.1f;         // Max size scale
    public float pulseDuration = 0.3f;      // Duration of full pulse cycle
    public float heartbeatInterval = 1.5f;  // 40 BPM = 60/40 = 1.5s per beat

    [Header("VFX")]
    public Light artifactLight;            // Optional: assign a Unity Light
    public float lightFlashIntensity = 2f;
    public ParticleSystem pulseBurstFX;    // Optional: assign a burst-style particle system
    public CinemachineImpulseSource _cameraShake;

    [SerializeField] float startDepth = -350f;   // When heartbeat begins
    [SerializeField] float coreDepth = -480f;    // When it reaches full intensity
    [SerializeField] float minVolume = 0f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] float volumeLerpSpeed = 2f;
    [SerializeField] AudioSource heartbeatAudio;
    [SerializeField] AudioClip[] heartBeatClips;
    [SerializeField] Transform _player;

    private Vector3 baseScale;


    private float heartbeatTimer = 0f;
    private float pulseTimer = 0f;
    private bool isPulsing = false;

    private float baseLightIntensity;


    private void Start()
    {
        // Initialize base scale and spin values
        baseScale = transform.localScale;

        // Store light's base intensity
        if (artifactLight != null)
            baseLightIntensity = artifactLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        heartbeatTimer += Time.deltaTime;

        if (heartbeatTimer >= heartbeatInterval)
        {
            isPulsing = true;
            pulseTimer = 0f;
            heartbeatTimer = 0f;

            // Light flash
            if (artifactLight != null)
                artifactLight.intensity = lightFlashIntensity;

            // Particle burst
            if (pulseBurstFX != null)
                pulseBurstFX.Play();

            _cameraShake.GenerateImpulse();

            heartbeatAudio.clip = heartBeatClips[Random.Range(0, heartBeatClips.Length-1)];
            heartbeatAudio.Play();
        }

        if (isPulsing)
        {
            pulseTimer += Time.deltaTime;
            float t = pulseTimer / pulseDuration;

            if (t < 1f)
            {
                float scaleFactor = Mathf.SmoothStep(1f, pulseScale, 1f - Mathf.Abs(2f * t - 1f));
                transform.localScale = baseScale * scaleFactor;

                if (artifactLight != null)
                {
                    float lightFactor = Mathf.Lerp(lightFlashIntensity, baseLightIntensity, t);
                    artifactLight.intensity = lightFactor;
                }
            }
            else
            {
                isPulsing = false;
                transform.localScale = baseScale;

                if (artifactLight != null)
                    artifactLight.intensity = baseLightIntensity;
            }
        }


        // Before heartbeat zone
        if (_player.position.y > startDepth)
        {
            heartbeatAudio.volume = Mathf.Lerp(heartbeatAudio.volume, 0f, volumeLerpSpeed * Time.deltaTime);
        }

        // In the heartbeat zone
        float depthPercent = Mathf.InverseLerp(startDepth, coreDepth, _player.position.y);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, depthPercent);
        heartbeatAudio.volume = Mathf.Lerp(heartbeatAudio.volume, targetVolume, volumeLerpSpeed * Time.deltaTime);
    }
}
