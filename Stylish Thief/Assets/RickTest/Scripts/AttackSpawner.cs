using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public GameObject attackPrefab;
    GameObject attack;
    public float timeBetweenSpawns;
    float currentTime;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < timeBetweenSpawns)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            attack = Instantiate(attackPrefab, transform.position, Quaternion.identity);
        }
    }
}
