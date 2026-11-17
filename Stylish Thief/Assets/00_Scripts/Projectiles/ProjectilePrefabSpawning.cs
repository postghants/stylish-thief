using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePrefabSpawning : MonoBehaviour
{
    public bool spawnAll;
    public bool spawnSequentially;
    [SerializeField] private List<GameObject> spawnableObjects;
    public GameObject lastSpawnedThing;
    int i = 0;
    private void OnDestroy()
    {
        if (spawnSequentially)
        {
            Instantiate(spawnableObjects[i], transform.position, Quaternion.identity);
            i++;
            if (i >= spawnableObjects.Count)
            {
                i = 0;
            }
        }
        else if (spawnAll)
        {
            foreach (GameObject obj in spawnableObjects)
            {
                Instantiate(obj, transform.position, Quaternion.identity);
            }
        }
        else
        {
            i = UnityEngine.Random.Range(0, spawnableObjects.Count - 1);
            Instantiate(spawnableObjects[i], transform.position, Quaternion.identity);
        }
    }
}