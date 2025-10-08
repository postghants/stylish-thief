using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var hit = other.GetComponentInParent<IDamageable>();
        Debug.Log(hit);
        hit?.TakeDamage(9999999);
    }
}
