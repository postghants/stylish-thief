using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 angleVelocity;

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }
}
