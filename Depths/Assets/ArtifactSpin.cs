using UnityEngine;

public class ArtifactSpin : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float minSpeed = 10f;
    public float maxSpeed = 45f;

    private Vector3 rotationAxis;
    private float rotationSpeed;

    void Start()
    {
        // Generate a random rotation axis (normalized to prevent speed stacking)
        rotationAxis = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        // Pick a random speed within your desired range
        rotationSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // Apply the rotation
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
