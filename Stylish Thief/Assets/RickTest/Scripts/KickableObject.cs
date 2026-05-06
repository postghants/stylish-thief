using FMODUnity;
using UnityEngine;

public class KickableObject : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider coll;
    PlayerStateDriver player;
    bool kicked;
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Tooltip("How hard is this object kicked forward?")] public float force;
    [Tooltip("How hard is this object kicked up?")] public float upForce;
    [Tooltip("How fast does this object rotate?")] public float rotationalForce;
    [Tooltip("How strong is the gravitational pull?")] public float gravity;
    [Tooltip("Where should this object's center move when kicked? This determines the middle of its rotation. Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 centerAfterHit;
    [Tooltip("What should this object's scale change to when kicked? Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 sizeAfterHit;
    [Tooltip("Where should this object's visual 3D model move when pushed? Recommended to leave at 0, 0, 0 in most cases.")] public Vector3 visualPositionAfterHit;

    //tom fmod EventReference
    [SerializeField] EventReference kickCanEvent;

    //public float scaleMultiplier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(0, gravity, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            coll.center = centerAfterHit;
            coll.size = sizeAfterHit;
            //GetComponentInChildren<Transform>().localPosition = visualPositionAfterHit;
            coll.isTrigger = false;
            rb.constraints = RigidbodyConstraints.None;
            gameObject.layer = 22;
            Vector3 targetDir = player.transform.position - transform.position;
            targetDir = -targetDir - new Vector3(0, -targetDir.y, 0) + new Vector3(0, upForce, 0);
            rb.linearVelocity = targetDir.normalized * force;
            rb.angularVelocity = Random.insideUnitSphere * rotationalForce;

            //tom audio event
            RuntimeManager.PlayOneShotAttached(kickCanEvent, gameObject);

            if (!kicked)
            {
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
            }
            kicked = true;
            Destroy(gameObject, 5f);
        }
    }
}