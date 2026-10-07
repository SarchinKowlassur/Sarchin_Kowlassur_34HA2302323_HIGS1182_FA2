using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns hostile enemy drones ahead of the player in the side-scrolling window with dynamic scaling.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Prefab & Timing")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float baseSpawnInterval = 8f;
    [SerializeField] private float minSpawnInterval = 2.5f; // Fastest limit
    [SerializeField] private float initialDelay = 5f;

    [Header("Spawn Bounds")]
    [SerializeField] private float spawnXDistance = 26f;
    [SerializeField] private Vector2 spawnYRange = new Vector2(-4f, 4f);

    private Camera mainCamera;
    private DifficultyManager difficultyManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        difficultyManager = Object.FindFirstObjectByType<DifficultyManager>();

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
            {
                SpawnEnemy();
            }

            // Calculate current interval adjusted by difficulty manager
            float currentMultiplier = (difficultyManager != null) ? difficultyManager.CurrentDifficultyMultiplier : 1f;
            float dynamicInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval / currentMultiplier);

            yield return new WaitForSeconds(dynamicInterval);
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float spawnX = mainCamera != null ? mainCamera.transform.position.x + spawnXDistance : transform.position.x + spawnXDistance;
        float spawnY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}