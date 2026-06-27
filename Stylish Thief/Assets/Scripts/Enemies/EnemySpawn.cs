using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private PatrolZone patrolZone;
    [SerializeField] private bool spawnOnlyOnce;
    [SerializeField] private bool spawnOnStart;
    [SerializeField] private bool spawnOnEnterPatrolZone;
    private PlayerStateDriver player;
    [SerializeField] private GameObject enemy;

    [SerializeField] private List<EnemySpawnInfo> enemySpawnInfo;

    private bool hasSpawned;
    public LayerMask patrolZoneLayer;

    private void Start()
    {
        if(patrolZone == null)
        {
            Collider[] zones = Physics.OverlapSphere(transform.position, 0.1f, patrolZoneLayer, QueryTriggerInteraction.Collide);
            if(zones.Length > 0 )
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
        if(spawnOnEnterPatrolZone)
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
        Instantiate(spawnInfo.enemy.gameObject, patrolZone.ClosestPoint(transform.position), Quaternion.identity).GetComponent<EnemyController>().Initialize(patrolZone, player);
        /*enemy = Instantiate(spawnInfo.enemy.gameObject, patrolZone.ClosestPoint(transform.position), Quaternion.identity);
        enemy.GetComponent<EnemyController>().ctx.activeZone = patrolZone;
        enemy.GetComponent<EnemyController>().ctx.player = player;*/
    }
}

[Serializable]
public class EnemySpawnInfo
{
    public EnemyController enemy;
    public int weight;
}
