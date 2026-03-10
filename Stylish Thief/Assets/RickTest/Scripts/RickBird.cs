using UnityEngine;

public class RickBird : MonoBehaviour
{
    RickBirdTrigger trigger;
    Vector3 targetDir;
    public float horizontalSpeed;
    public float verticalSpeed = 0;
    public float verticalAccelerationMin;
    public float verticalAccelerationMax;
    float verticalAcceleration;
    public float maxVerticalSpeed;
    public float destroyTime;
    public GameObject sitting;
    public GameObject flying;
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
