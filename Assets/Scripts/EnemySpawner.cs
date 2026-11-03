using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 一条波次配置：使用哪种敌人、生成多少个，以及生成节奏。
    [Serializable]
    private class EnemyWave
    {
        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField, Min(1)]
        private int enemyCount = 3;

        [SerializeField, Min(0f)]
        private float spawnInterval = 1f;

        [SerializeField, Min(0f)]
        private float delayAfterClear = 2f;

        public GameObject EnemyPrefab => enemyPrefab;
        public int EnemyCount => enemyCount;
        public float SpawnInterval => spawnInterval;
        public float DelayAfterClear => delayAfterClear;
    }

    // ===== 波次设置 =====
    [SerializeField]
    private EnemyWave[] waves;

    [SerializeField]
    private float spawnY = 6f;

    [SerializeField]
    private float minX = -7f;

    [SerializeField]
    private float maxX = 7f;

    // ===== Boss 设置 =====
    [SerializeField]
    private GameObject bossPrefab;

    [SerializeField, Min(0f)]
    private float bossSpawnDelay = 2f;

    [SerializeField]
    private BossHealthBar bossHealthBar;

    // ===== 运行时状态 =====
    private bool bossSpawned;

    // ===== Unity 生命周期函数 =====
    private IEnumerator Start()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning("EnemySpawner 尚未配置任何波次。");
            yield break;
        }

        yield return RunAllWaves();
        yield return new WaitForSeconds(bossSpawnDelay);

        SpawnBoss();
    }

    // ===== 波次流程 =====
    private IEnumerator RunAllWaves()
    {
        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            EnemyWave wave = waves[waveIndex];

            if (wave == null || wave.EnemyPrefab == null)
            {
                Debug.LogWarning($"第 {waveIndex + 1} 波没有设置敌人 Prefab，已跳过。");
                continue;
            }

            Debug.Log($"第 {waveIndex + 1} 波开始");

            yield return SpawnWave(wave);
            yield return new WaitUntil(AreAllEnemiesDefeated);
            yield return new WaitForSeconds(wave.DelayAfterClear);
        }
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        for (int i = 0; i < wave.EnemyCount; i++)
        {
            SpawnEnemy(wave.EnemyPrefab);

            if (i < wave.EnemyCount - 1)
            {
                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        return FindObjectsOfType<EnemyHealth>().Length == 0;
    }

    // ===== 普通敌人生成 =====
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        Vector2 spawnPosition = new Vector2(randomX, spawnY);

        Instantiate(
            enemyPrefab,
            spawnPosition,
            enemyPrefab.transform.rotation
        );
    }

    // ===== Boss 生成 =====
    private void SpawnBoss()
    {
        if (bossSpawned || bossPrefab == null)
        {
            return;
        }

        Vector2 spawnPosition = new Vector2(0f, spawnY);

        GameObject bossObject = Instantiate(
            bossPrefab,
            spawnPosition,
            bossPrefab.transform.rotation
        );

        EnemyHealth bossHealth =
            bossObject.GetComponent<EnemyHealth>();

        if (bossHealthBar != null && bossHealth != null)
        {
            bossHealthBar.Show(bossHealth);
        }

        bossSpawned = true;
    }
}
