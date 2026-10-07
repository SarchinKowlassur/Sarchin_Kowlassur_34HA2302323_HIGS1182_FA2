using System.Collections;
using UnityEngine;

/// <summary>
/// Handles automatic dynamic spawning of scrap collectibles in the play area.
/// </summary>
public class CollectibleManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float initialDelay = 2f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnXDistance = 22f;
    [SerializeField] private Vector2 spawnYRange = new Vector2(-5f, 5f);

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
            {
                SpawnCollectible();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Spawns a single scrap item ahead of the camera view.
    public void SpawnCollectible()
    {
        if (collectiblePrefab == null) return;

        float spawnX = mainCamera != null ? mainCamera.transform.position.x + spawnXDistance : transform.position.x + spawnXDistance;
        float spawnY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
    }

    // Helper method to drop scrap at specific world coordinates
    public void SpawnAtPosition(Vector3 position)
    {
        if (collectiblePrefab == null) return;
        Instantiate(collectiblePrefab, position, Quaternion.identity);
    }
}