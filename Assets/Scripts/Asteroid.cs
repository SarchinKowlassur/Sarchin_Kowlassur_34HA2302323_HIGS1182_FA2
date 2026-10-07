using UnityEngine;

/// <summary>
/// Controls individual asteroid drift movement, rotation, and off-screen cleanup.
/// </summary>
public class Asteroid : MonoBehaviour
{
    [Header("Drift & Speed Settings")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float minRotationSpeed = 20f;
    [SerializeField] private float maxRotationSpeed = 100f;

    [Header("Cleanup Bounds")]
    [SerializeField] private float destroyXOffset = 25f; // Distance behind camera before destroying

    private float driftSpeed;
    private Vector3 rotationAxis;
    private float rotationSpeed;
    private Transform mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // Randomize speed and tumble rotation for visual variety
        driftSpeed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationAxis = Random.onUnitSphere; // Random 3D rotational axis
    }

    private void Update()
    {
        // Move left
        transform.Translate(Vector3.left * driftSpeed * Time.deltaTime, Space.World);

        // Tumble asteroid
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        if (mainCameraTransform != null)
        {
            if (transform.position.x < mainCameraTransform.position.x - destroyXOffset)
            {
                Destroy(gameObject);
            }
        }
    }
}
