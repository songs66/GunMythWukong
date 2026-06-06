using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("敌人预制体")]
    public GameObject enemyPrefab;

    [Header("场景引用")]
    public Transform enemyParent;             // 生成出来的敌人放到 Enemies 节点下面
    public Transform spawnPointParent;        // EnemySpawnPoints 父节点

    [Header("刷新设置")]
    public int maxAliveEnemies = 10;          // 场上最多敌人数
    public float spawnInterval = 2f;          // 每隔几秒尝试刷新一次
    public float firstSpawnDelay = 1f;        // 开始游戏后多久开始刷怪
    public bool spawnOnStart = true;          // 是否游戏开始自动刷怪

    [Header("调试信息")]
    public int currentAliveCount = 0;
    public int poolCount = 0;

    private readonly List<GameObject> enemyPool = new List<GameObject>();
    private readonly List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        CollectSpawnPoints();
        CreateEnemyPool();

        if (spawnOnStart)
        {
            StartCoroutine(SpawnLoop());
        }
    }

    void CollectSpawnPoints()
    {
        spawnPoints.Clear();

        if (spawnPointParent == null)
        {
            Debug.LogError("EnemySpawnManager 没有设置 spawnPointParent。");
            return;
        }

        foreach (Transform child in spawnPointParent)
        {
            spawnPoints.Add(child);
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("EnemySpawnPoints 下面没有任何刷新点。");
        }
    }

    void CreateEnemyPool()
    {
        enemyPool.Clear();

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawnManager 没有设置 enemyPrefab。");
            return;
        }

        for (int i = 0; i < maxAliveEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);

            enemy.name = "Enemy_Trooper_Pooled_" + (i + 1).ToString("00");

            if (enemyParent != null)
            {
                enemy.transform.SetParent(enemyParent);
            }

            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }

        poolCount = enemyPool.Count;
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(firstSpawnDelay);

        while (true)
        {
            UpdateAliveCount();

            if (currentAliveCount < maxAliveEnemies)
            {
                SpawnOneEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            return;
        }

        GameObject enemy = GetInactiveEnemyFromPool();

        if (enemy == null)
        {
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        if (spawnPoint == null)
        {
            return;
        }

        PlaceEnemyAtSpawnPoint(enemy, spawnPoint);

        enemy.SetActive(true);

        UpdateAliveCount();
    }

    GameObject GetInactiveEnemyFromPool()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy != null && !enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        return null;
    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index];
    }

    void PlaceEnemyAtSpawnPoint(GameObject enemy, Transform spawnPoint)
    {
        /*
         * CharacterController 在移动位置时容易产生碰撞残留，
         * 所以传送前先临时禁用，设置位置后再启用。
         */
        CharacterController controller = enemy.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    void UpdateAliveCount()
    {
        int count = 0;

        foreach (GameObject enemy in enemyPool)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                count++;
            }
        }

        currentAliveCount = count;
    }
}