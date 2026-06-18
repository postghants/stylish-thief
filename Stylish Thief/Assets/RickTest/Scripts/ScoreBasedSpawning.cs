using System;
using System.Collections.Generic;
using UnityEngine;
using static RickBingo;

public class ScoreBasedSpawning : MonoBehaviour
{
    private CrimeSpreeManager manager;
    [SerializeField] private List<ScoreBasedSpawnInfo> scoreBasedSpawnInfo;
    private float recordedScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponentInParent<CrimeSpreeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.Score > recordedScore)
        {
            recordedScore = manager.Score;
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
                    info.spawnedThing = Instantiate(info.thingToSpawn, new Vector3(0, -500, 0), Quaternion.identity);
                }
                else
                {
                    info.thingToSpawn.SetActive(true);
                }
                info.hasSpawned = true;
                info.spawnedThing.transform.position = info.spawnLocation;
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