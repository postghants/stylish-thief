using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnemySpawn : MonoBehaviour
{
    [SerializeField] private PatrolZone patrolZone;
    [SerializeField] private bool spawnOnlyOnce;
    [SerializeField] private bool spawnOnStart;
    [SerializeField] private bool spawnOnEnterPatrolZone;
    private PlayerStateDriver player;

    [SerializeField] private List<EnemySpawnInfo> enemySpawnInfo;

    private bool hasSpawned;
    public LayerMask patrolZoneLayer;

    private void Start()
    {
        if (patrolZone == null)
        {
            Collider[] zones = Physics.OverlapSphere(transform.position, 0.1f, patrolZoneLayer, QueryTriggerInteraction.Collide);
            if (zones.Length > 0)
            {
                patrolZone = zones[0].GetComponent<PatrolZone>();
            }
            else
            {
                Debug.Log($"{name} couldn't find patrol zone!");
                Destroy(gameObject);

                return;
            }
        }
        player = FindAnyObjectByType<PlayerStateDriver>();
        if (spawnOnStart)
        {
            SpawnRandomEnemy();
        }
        if (spawnOnEnterPatrolZone)
        {
            patrolZone.OnPlayerEnter.AddListener(SpawnRandomEnemy);
        }
    }

    public void SpawnRandomEnemy()
    {
        if (hasSpawned) { return; }

        if (spawnOnlyOnce) { hasSpawned = true; }
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
        Instantiate(spawnInfo.enemy.gameObject, transform.position, Quaternion.identity).GetComponent<EnemyController>().Initialize(patrolZone, player);
    }
}

[Serializable]
public class DynamicEnemySpawnInfo
{
    public EnemyController enemy;
    public int weight;
    public int minIQ;
}
