using FMODUnity;
using UnityEngine;

public class RickBird : MonoBehaviour
{
    RickBirdTrigger trigger;
    [Tooltip("How fast should it fly horizontally?")] public float horizontalSpeed;
    [Tooltip("What's the slowest the bird can accelerate upward?")] public float verticalAccelerationMin;
    [Tooltip("What's the fastest the bird can accelerate upward?")] public float verticalAccelerationMax;
    [Tooltip("What's the fastest the bird can fly up?")] public float maxVerticalSpeed;
    [Tooltip("How long does it take for the bird to disappear?")] public float destroyTime;
    [Tooltip("Sitting (or standing) version of the bird's visuals")] public GameObject sitting;
    [Tooltip("Flying version of the bird's visuals")] public GameObject flying;
    [Header("Internal no touchy")]
    [Tooltip("Do not touch! Leave at 0!")] public float verticalSpeed = 0;
    public float verticalAcceleration;
    public Vector3 targetDir;
    private Vector3 startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //tom audio
    [SerializeField] EventReference birdFlyEvent;

    void Start()
    {

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetDir != Vector3.zero)
        {
            transform.Translate(new Vector3(-(targetDir.normalized.x) * horizontalSpeed, verticalSpeed, -(targetDir.normalized.z) * horizontalSpeed) * Time.deltaTime);
            
            if (verticalSpeed < maxVerticalSpeed)
            {
                verticalSpeed += verticalAcceleration;
            }
        }
    }
    public void FlyAway(Transform player)
    {
        RuntimeManager.PlayOneShotAttached(birdFlyEvent, gameObject);
        sitting.SetActive(false);
        flying.SetActive(true);
        trigger = GetComponentInParent<RickBirdTrigger>();
        verticalAcceleration = Random.Range(verticalAccelerationMin, verticalAccelerationMax);
        targetDir = player.position - transform.position;
    }
    public void Reset()
    {
        transform.position = startPosition;
        verticalSpeed = 0;
        verticalAcceleration = 0;
        targetDir = Vector3.zero;
        sitting.SetActive(true);
        flying.SetActive(false);
    }
}
