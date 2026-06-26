using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public GameObject attackPrefab;
    GameObject attack;
    public float timeBetweenSpawns;
    float currentTime;
    PlayerStateDriver player;

    [Header("Behavior")]
    public bool rotateAroundTarget;
    public bool targetIsPlayer;
    public Vector3 rotationTarget;
    public float rotationSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
        player = FindFirstObjectByType<PlayerStateDriver>();
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
        if (rotateAroundTarget)
        {
            if (targetIsPlayer)
            {
                rotationTarget = player.transform.position;
            }
            transform.RotateAround(rotationTarget, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
            
        }
        else
        {
            //attack.transform.parent = transform;
        }
    }
}
