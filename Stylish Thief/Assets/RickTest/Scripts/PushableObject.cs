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
    [Tooltip("Does the player need to Grab this object to push it?")] public bool requiresGrab;
    [Tooltip("How far does this object teleport forward when pushed? Used to clear the ledge it stands on before falling or it'll just instantly die.")] public float distance;
    [Tooltip("How fast will this object move forwards when pushed?")] public float force;
    [Tooltip("How fast will this object move upwards when pushed?")] public float upForce;
    [Tooltip("How fast will this object rotate forwards when pushed?")] public float rotationalForce;
    [Tooltip("How hard is gravity pulling on this object?")] public float gravity;
    //[Tooltip("Where should this object's center move when pushed? This determines the middle of its rotation. Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 centerAfterHit;
    //[Tooltip("Where should this object's scale change when pushed? Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 sizeAfterHit;
    //[Tooltip("Where should this object's visual 3D model move when pushed? Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 visualPositionAfterHit;
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
                player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
                if (requiresGrab)
                {
                    if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
                    {
                        //Move forward for X amount of seconds first. Then fall!
                        transform.position += transform.forward * distance;
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
                    //Move forward for X amount of seconds first. Then fall!
                    transform.position += transform.forward * distance;
                    //GetComponentInChildren<Transform>().localPosition = visualPositionAfterHit;
                    //coll.isTrigger = false;
                    rb.constraints = RigidbodyConstraints.None;
                    gameObject.layer = 22;
                    rb.linearVelocity = transform.forward * force;
                    rb.angularVelocity = transform.right * rotationalForce;
                    kicked = true;
                }
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
    private void OnCollisionEnter(Collision collision)
    {
        if (!kicked)
        {
            if (collision.gameObject.layer == 6)
            {
                if (requiresGrab)
                {
                    if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
                    {
                        //Move forward for X amount of seconds first. Then fall!
                        transform.position += transform.forward * distance;
                        player = collision.gameObject.GetComponentInParent<PlayerStateDriver>();
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
                    //Move forward for X amount of seconds first. Then fall!
                    transform.position += transform.forward * distance;
                    player = collision.gameObject.GetComponentInParent<PlayerStateDriver>();
                    //GetComponentInChildren<Transform>().localPosition = visualPositionAfterHit;
                    //coll.isTrigger = false;
                    rb.constraints = RigidbodyConstraints.None;
                    gameObject.layer = 22;
                    rb.linearVelocity = transform.forward * force;
                    rb.angularVelocity = transform.right * rotationalForce;
                    kicked = true;
                }
            }

        }
        else
        {
            if (collision.gameObject.layer != 6)
            {
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
                Destroy(gameObject);
            }
        }
    }
}
