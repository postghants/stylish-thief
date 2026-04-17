using FMODUnity;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PushableObject : MonoBehaviour
{
    Rigidbody rb;
    Collider coll;
    PlayerStateDriver player;
    bool kicked;
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    public float distance;
    public float force;
    public float upForce;
    public float rotationalForce;
    public float gravity;
    public Vector3 centerAfterHit;
    public Vector3 sizeAfterHit;
    public Vector3 visualPositionAfterHit;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();

    }
    void FixedUpdate()
    {
        rb.AddForce(0, gravity, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!kicked)
        {
            if (other.gameObject.layer == 6)
            {
                //Move forward for X amount of seconds first. Then fall!
                transform.position += transform.forward * distance;
                player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
                //GetComponentInChildren<Transform>().localPosition = visualPositionAfterHit;
                //coll.isTrigger = false;
                rb.constraints = RigidbodyConstraints.None;
                gameObject.layer = 22;
                rb.linearVelocity = transform.forward * force;
                rb.angularVelocity = transform.right * rotationalForce;
                kicked = true;
            }
                
        }
        else
        {
            if (other.gameObject.layer != 6)
            {
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
                Destroy(gameObject);
            }
        }
    }
}
