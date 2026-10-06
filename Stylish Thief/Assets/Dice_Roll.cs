using UnityEngine;
using UnityEngine.UI;

public class Dice_Roll : MonoBehaviour
{
    Rigidbody rb;
    public PlayerStateDriver player;
    public float ExplosionForce = 50f;
    public float ExplosionRadius = 5f;
    public float ForwardForce;
    public float UpwardsMod = 1f;
    public float rotation;
    public int DiceNumber = 0;

    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (rb.isKinematic == true)
        {
            transform.RotateAround(transform.position, Vector3.up, rotation * Time.deltaTime);
        }

        if (DiceNumber != 0 && rb.angularVelocity == Vector3.zero)
        {
            Debug.Log(DiceNumber);
        }

    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 6)
        {
            rb.isKinematic = false;
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing" || player.Root.Leaf().ToString() == "HSM.PlayerPound")
            {     
                RollHard();
            }
            else
            {
                RollNormal();
            }
        }
    }

    private void RollNormal()
    {
        rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, UpwardsMod);
        rb.AddForce(Vector3.forward * ForwardForce);
    }

    private void RollHard()
    {
        rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, UpwardsMod);
        rb.AddForce(Vector3.forward * (ForwardForce * 1.75f));
    }
    
    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

}
