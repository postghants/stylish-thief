using UnityEngine;

public class ProjectileFwdMovement : MonoBehaviour
{
    public float speed;
    public float acceleration;
    private Projectile projectile;

    void Start()
    {
        projectile = GetComponent<Projectile>();
        projectile.velocity.z += speed;
    }

    void Update()
    {
        projectile.velocity.z += acceleration * Time.deltaTime;
    }
}
