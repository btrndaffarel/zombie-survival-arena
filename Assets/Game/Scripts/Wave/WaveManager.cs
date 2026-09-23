using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Wave Settings")]
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private int startingEnemyCount = 3;
    [SerializeField] private int enemyIncreasePerWave = 2;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float timeBetweenWaves = 5f;

    private int currentWave = 0;
    private int enemiesRemaining = 0;

    private bool isSpawning = false;
    private bool waitingForNextWave = false;

    public int CurrentWave => currentWave;
    public int MaxWaves => maxWaves;
    public int EnemiesRemaining => enemiesRemaining;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        currentWave++;

        int enemyCount =
            startingEnemyCount +
            ((currentWave - 1) * enemyIncreasePerWave);

        enemiesRemaining = enemyCount;

        Debug.Log(
            "===== WAVE " +
            currentWave +
            " DIMULAI ====="
        );

        Debug.Log(
            "Jumlah Zombie: " +
            enemyCount
        );

        isSpawning = true;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy =
                enemySpawner.SpawnEnemy();

            if (enemy != null)
            {
                EnemyHealth enemyHealth =
                    enemy.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.OnDied += HandleEnemyDied;
                }
            }

            yield return new WaitForSeconds(
                spawnInterval
            );
        }

        isSpawning = false;

        if (enemiesRemaining <= 0)
        {
            WaveComplete();
        }
    }

    private void HandleEnemyDied(
        EnemyHealth enemy
    )
    {
        enemy.OnDied -= HandleEnemyDied;

        enemiesRemaining--;

        Debug.Log(
            "Zombie Remaining: " +
            enemiesRemaining
        );

        if (
            enemiesRemaining <= 0 &&
            !isSpawning
        )
        {
            WaveComplete();
        }
    }

    private void WaveComplete()
    {
        if (waitingForNextWave)
            return;

        Debug.Log(
            "===== WAVE " +
            currentWave +
            " COMPLETE ====="
        );

        if (currentWave >= maxWaves)
        {
            Debug.Log(
                "SEMUA WAVE TESTING SELESAI!"
            );

            return;
        }

        StartCoroutine(
            NextWaveCountdown()
        );
    }

    private IEnumerator NextWaveCountdown()
    {
        waitingForNextWave = true;

        Debug.Log(
            "Wave berikutnya dalam " +
            timeBetweenWaves +
            " detik..."
        );

        yield return new WaitForSeconds(
            timeBetweenWaves
        );

        waitingForNextWave = false;

        StartCoroutine(
            StartNextWave()
        );
    }
}