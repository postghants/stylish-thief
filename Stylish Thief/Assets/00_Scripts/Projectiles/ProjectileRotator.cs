using UnityEngine;

public class ProjectileRotator : MonoBehaviour
{
    public Vector3 angleVelocity;
    public Vector3 angleAccel;
    Projectile projectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectile = GetComponent<Projectile>();
        projectile.angleVelocity = angleVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        projectile.angleVelocity += angleAccel * Time.deltaTime;
    }
}
