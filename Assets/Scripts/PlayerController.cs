using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Complete Player Controller with side-scrolling movement, raycast shooting, and laser projectile spawning.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float moveSpeedX = 12f;
    [SerializeField] private float moveSpeedY = 16f;
    [SerializeField] private float cameraScrollSpeed = 5f;
    [SerializeField] private float tiltAngle = 20f;

    [Header("Screen Edge Padding")]
    [SerializeField] private float paddingX = 1.0f;
    [SerializeField] private float paddingY = 0.3f;

    [Header("Shooting & Projectile Settings")]
    [SerializeField] private GameObject laserProjectilePrefab; // Drag laser prefab here
    [SerializeField] private Transform firePoint;             // Drag laser muzzle point here
    [SerializeField] private float weaponRange = 100f;
    [SerializeField] private LayerMask shootableLayer;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        HandleMovementAndClamping();
        HandleShooting();
    }

    private void HandleMovementAndClamping()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
        }

        float velX = (moveX * moveSpeedX) + cameraScrollSpeed;
        float velY = moveY * moveSpeedY;

        Vector3 movement = new Vector3(velX, velY, 0f) * Time.deltaTime;
        Vector3 targetPos = transform.position + movement;

        float distanceToCam = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        Vector3 minScreen = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceToCam));
        Vector3 maxScreen = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceToCam));

        targetPos.x = Mathf.Clamp(targetPos.x, minScreen.x + paddingX, maxScreen.x - paddingX);
        targetPos.y = Mathf.Clamp(targetPos.y, minScreen.y + paddingY, maxScreen.y - paddingY);
        targetPos.z = 0f;

        transform.position = targetPos;

        float targetTilt = Mathf.Clamp(moveY * tiltAngle, -tiltAngle, tiltAngle);
        transform.rotation = Quaternion.Euler(0f, 90f, targetTilt);
    }

    // Executes raycast detection and spawns a visual laser projectile.
    private void HandleShooting()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            // Spawn visual projectile starting position
            Vector3 spawnPoint = firePoint != null ? firePoint.position : transform.position;
            Quaternion spawnRotation = Quaternion.Euler(0f, 90f, 0f); // Default pointing right (+X)

            Debug.Log("Player fired raycast laser weapon.");

            // Determine direction to aim laser projectile
            if (Physics.Raycast(ray, out hit, weaponRange, shootableLayer))
            {
                Debug.Log("Raycast hit target: " + hit.transform.name);

                // Calculate trajectory towards raycast hit point
                Vector3 targetDirection = (hit.point - spawnPoint).normalized;
                spawnRotation = Quaternion.LookRotation(targetDirection);

                if (hit.collider.CompareTag("Asteroid") || hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Raycast hit target: " + hit.collider.name);
                    Destroy(hit.collider.gameObject);
                }
            }

            // Spawn visual laser bolt prefab
            if (laserProjectilePrefab != null)
            {
                Instantiate(laserProjectilePrefab, spawnPoint, spawnRotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only handle fatal asteroid collisions here
        if (other.CompareTag("Asteroid"))
        {
            Debug.Log("Player collided with an Asteroid!");
            GameManager.Instance.GameOver(false);
            Destroy(gameObject);
        }
    }
}