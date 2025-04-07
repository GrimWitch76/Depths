using UnityEngine;

public class ArtifactPulse : MonoBehaviour
{
    [Header("Pulsing Settings")]
    public float pulseSpeed = 2f;           // How fast the pulse happens
    public float pulseMagnitude = 0.1f;     // How big the pulse gets (0.1 = 10% scale change)
    private Vector3 baseScale;

    private void Start()
    {
        // Initialize base scale and spin values
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Pulse the scale over time
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseMagnitude;
        transform.localScale = baseScale * (1f + scaleOffset);
    }
}
