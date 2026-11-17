using System;
using System.Collections.Generic;
using UnityEngine;
using static RickBingo;

public class ScoreBasedSpawning : MonoBehaviour
{
    private NeoCrimeSpreeManager manager;
    [SerializeField] private List<ScoreBasedSpawnInfo> scoreBasedSpawnInfo;
    private float recordedScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponentInParent<NeoCrimeSpreeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.points > recordedScore)
        {
            recordedScore = manager.points;
            SpawnThings();
        }
    }
    public void SpawnThings()
    {
        foreach (ScoreBasedSpawnInfo info in scoreBasedSpawnInfo)
        {
            if (!info.hasSpawned && recordedScore >= info.requiredScore)
            {
                if (info.spawnNewThing)
                {
                    info.spawnedThing = Instantiate(info.thingToSpawn, info.spawnLocation, Quaternion.identity);
                }
                else
                {
                    info.thingToSpawn.SetActive(true);
                }
                info.hasSpawned = true;
            }
        }
    }
}

[Serializable]
public class ScoreBasedSpawnInfo
{
    public bool spawnNewThing;
    public GameObject thingToSpawn;
    public GameObject spawnedThing;
    public Vector3 spawnLocation;
    public bool hasSpawned;
    public float requiredScore;
}

//A list with scores to be higher than, a bool for whether they've been hit previously already, and the gameObject they spawn + its location
//Then every update, run through the list until a score has been reached that hasn't been registered yet (if > X && !registered)
//Has something in the list not been registered? Register its bool as true, spawn the item, and return