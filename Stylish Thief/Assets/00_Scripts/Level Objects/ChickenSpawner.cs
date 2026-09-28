using System;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public bool spawnSequentially;
    public GameObject lastSpawnedThing;
    [SerializeField] private List<SpawnInfo> spawnInfo;
    float timer;
    int i = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timeBetweenSpawns)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            if (spawnSequentially)
            {
                Instantiate(spawnInfo[i].thingToSpawn, transform.position, Quaternion.identity);
                i++;
                if (i >= spawnInfo.Count)
                {
                    i = 0;
                }
            }
            else
            {
                i = UnityEngine.Random.Range(0, spawnInfo.Count-1);
                Instantiate(spawnInfo[i].thingToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}

[Serializable]
public class SpawnInfo
{
    public GameObject thingToSpawn;
}