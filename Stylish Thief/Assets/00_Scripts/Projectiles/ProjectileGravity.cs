using UnityEngine;

public class ProjectileGravity : MonoBehaviour
{
    public Vector3 gravityDirection;
    public float gravityStrength;
    private Projectile projectile;

    void Start()
    {
        projectile = GetComponent<Projectile>();
    }

    void Update()
    {
        projectile.velocity += gravityDirection * gravityStrength * Time.deltaTime;
    }
}
