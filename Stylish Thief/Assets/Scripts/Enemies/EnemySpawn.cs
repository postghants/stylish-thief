using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private PatrolZone patrolZone;
    [SerializeField] private bool spawnOnlyOnce;
    private PlayerStateDriver player;

    [SerializeField] private List<EnemySpawnInfo> enemySpawnInfo;

    private bool hasSpawned;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStateDriver>();
    }

    public void SpawnRandomEnemy()
    {
        if(hasSpawned) { return; }

        if(spawnOnlyOnce) { hasSpawned = true; }
        int totalWeight = 0;
        foreach (var enemy in enemySpawnInfo) { totalWeight += enemy.weight; }

        int randomWeight = UnityEngine.Random.Range(1, totalWeight + 1);
        foreach (var enemy in enemySpawnInfo)
        {
            if (randomWeight <= enemy.weight)
            {
                SpawnEnemy(enemy);
                break;
            }
            else { randomWeight -= enemy.weight; }
        }
    }

    public void SpawnEnemy(EnemySpawnInfo spawnInfo)
    {
        Instantiate(spawnInfo.enemy.gameObject).GetComponent<EnemyStateDriver>().Initialize(patrolZone, player);
    }
}

[Serializable]
public class EnemySpawnInfo
{
    public EnemyStateDriver enemy;
    public int weight;
}
