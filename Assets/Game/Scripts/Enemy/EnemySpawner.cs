using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;

    public GameObject SpawnEnemy()
    {
        if (zombiePrefab == null)
        {
            Debug.LogError("Zombie Prefab belum dipasang!");
            return null;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Point belum dipasang!");
            return null;
        }

        int randomIndex =
            Random.Range(0, spawnPoints.Length);

        Transform selectedSpawnPoint =
            spawnPoints[randomIndex];

        GameObject enemy = Instantiate(
            zombiePrefab,
            selectedSpawnPoint.position,
            Quaternion.identity
        );

        return enemy;
    }
}