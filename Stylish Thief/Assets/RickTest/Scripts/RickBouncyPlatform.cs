using UnityEngine;

public class RickBouncyPlatform : MonoBehaviour
{
    public float launchForce;
    public bool overRideLaunchDirection;
    public Vector3 forcedLaunchDirection;
    Vector3 directionalForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per fuck
    void Update()
    {
        Debug.Log(transform.up);
        directionalForce = transform.up * launchForce;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("Registered player");
            collision.gameObject.GetComponentInParent<PlayerStateDriver>().TakeKnockback(directionalForce);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Registered player");
            if (overRideLaunchDirection)
            {
                other.gameObject.GetComponentInParent<PlayerStateDriver>().TakeKnockback(forcedLaunchDirection * launchForce);
            }
            other.gameObject.GetComponentInParent<PlayerStateDriver>().TakeKnockback(directionalForce);
        }
    }
}


//On colision find the player
//Figure out which vector 3 direction is up for the platform
//Multiply that direction with a set launchforce
//Apply the force to the player