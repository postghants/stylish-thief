using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    PlayerStateDriver player;

    [Tooltip("How much damage to deal on hit")] public float damage;

    [Tooltip("Should a collider turn off when dealing damage? Typically yes or the move can deal damage multiple times in one go!")] public bool turnColliderOff;

    [Tooltip("If TurnColliderOff, which one?")] public Collider coll;
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
