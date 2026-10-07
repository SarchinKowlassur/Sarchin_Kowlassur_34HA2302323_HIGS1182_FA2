using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns asteroid prefabs continuously ahead of the player in a designated side-scrolling spawn window with dynamic scaling.
/// </summary>
public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefab & Target References")]
    [SerializeField] private GameObject[] asteroidPrefabs; // Array supports multiple asteroid designs
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Timing")]
    [SerializeField] private float baseSpawnInterval = 1.5f;
    [SerializeField] private float minSpawnInterval = 0.5f;   // Fastest limit
    [SerializeField] private float initialDelay = 1.0f;

    [Header("Designated Spawn Bounds")]
    [SerializeField] private float spawnXDistance = 25f;    // Distance ahead of camera/player
    [SerializeField] private Vector2 spawnYRange = new Vector2(-6f, 6f); // Top and bottom bounds
    [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 2.2f); // Size variety

    private Camera mainCamera;
    private DifficultyManager difficultyManager;

    private void Start()
    {
        mainCamera = Camera.main;
        difficultyManager = Object.FindFirstObjectByType<DifficultyManager>();

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        StartCoroutine(SpawnRoutine());
    }

    // Coroutine loop that continuously instantiates asteroids while scaling speed with difficulty.
    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
            {
                SpawnObstacle();
            }

            // Calculate current interval adjusted by difficulty manager
            float currentMultiplier = (difficultyManager != null) ? difficultyManager.CurrentDifficultyMultiplier : 1f;
            float dynamicInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval / currentMultiplier);

            yield return new WaitForSeconds(dynamicInterval);
        }
    }

    // Custom method to instantiate an asteroid within designated random boundaries.
    public void SpawnObstacle()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        // Select random prefab from array
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);
        GameObject selectedPrefab = asteroidPrefabs[randomIndex];

        // Calculate spawn position ahead of the camera view
        float spawnX = mainCamera != null ? mainCamera.transform.position.x + spawnXDistance : transform.position.x + spawnXDistance;
        float spawnY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        // Instantiate and apply random scale
        GameObject newAsteroid = Instantiate(selectedPrefab, spawnPos, Random.rotation);
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        newAsteroid.transform.localScale = Vector3.one * randomScale;

        Debug.Log("Obstacle Spawned: " + newAsteroid.name + " at " + spawnPos);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizes spawn bounds in Editor
        Gizmos.color = Color.red;
        Vector3 center = transform.position + new Vector3(spawnXDistance, (spawnYRange.x + spawnYRange.y) / 2f, 0f);
        Vector3 size = new Vector3(2f, Mathf.Abs(spawnYRange.y - spawnYRange.x), 2f);
        Gizmos.DrawWireCube(center, size);
    }
}