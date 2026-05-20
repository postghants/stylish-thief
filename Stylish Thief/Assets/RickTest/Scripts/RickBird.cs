using UnityEngine;

public class RickBird : MonoBehaviour
{
    RickBirdTrigger trigger;
    Vector3 targetDir;
    [Tooltip("How fast should it fly horizontally?")] public float horizontalSpeed;
    [Tooltip("What's the slowest the bird can accelerate upward?")] public float verticalAccelerationMin;
    [Tooltip("What's the fastest the bird can accelerate upward?")] public float verticalAccelerationMax;
    float verticalAcceleration;
    [Tooltip("What's the fastest the bird can fly up?")] public float maxVerticalSpeed;
    [Tooltip("How long does it take for the bird to disappear?")] public float destroyTime;
    [Tooltip("Sitting (or standing) version of the bird's visuals")] public GameObject sitting;
    [Tooltip("Flying version of the bird's visuals")] public GameObject flying;
    [Header("Internal no touchy")]
    [Tooltip("Do not touch! Leave at 0!")] public float verticalSpeed = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetDir != null)
        {
            transform.Translate(new Vector3(-(targetDir.normalized.x) * horizontalSpeed, verticalSpeed, -(targetDir.normalized.z) * horizontalSpeed) * Time.deltaTime);
            
            if (verticalSpeed < maxVerticalSpeed)
            {
                verticalSpeed += verticalAcceleration;
            }
            else if (verticalSpeed > .1)
            {
                Destroy(gameObject, destroyTime);
            }
        }
    }
    public void FlyAway(Transform player)
    {
        sitting.SetActive(false);
        flying.SetActive(true);
        trigger = GetComponentInParent<RickBirdTrigger>();
        verticalAcceleration = Random.Range(verticalAccelerationMin, verticalAccelerationMax);
        targetDir = player.position - transform.position;
    }
}
