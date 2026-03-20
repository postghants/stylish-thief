using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class KickableObject : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider coll;
    PlayerStateDriver player;
    bool kicked;
    public float givenScore;
    public string crime;
    public float force;
    public float upForce;
    public float rotationalForce;
    public float gravity;
    public Vector3 centerAfterHit;
    public Vector3 sizeAfterHit;
    public Vector3 visualPositionAfterHit;
    //public float scaleMultiplier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        coll.isTrigger = true;
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
            if (!kicked)
            {
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
            }
            kicked = true;
            Destroy(gameObject, 5f);
        }
    }
}