using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var hit = other.GetComponentInParent<IDamageable>();
        hit?.TakeDamage(9999999);
    }
}
