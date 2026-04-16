using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    PlayerStateDriver player;
    public float damage;
    public bool turnColliderOff;
    public Collider coll;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            player.TakeDamage(damage);
            if (turnColliderOff)
            {
                if (coll != null) { coll.enabled = false; }
            }
        }
    }
}
