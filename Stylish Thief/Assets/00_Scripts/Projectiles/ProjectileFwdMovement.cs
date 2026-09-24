using UnityEngine;

public class ProjectileFwdMovement : MonoBehaviour
{
    public float speed;
    public float acceleration;
    private Projectile projectile;

    void Start()
    {
        projectile = GetComponent<Projectile>();
        projectile.velocity.x += speed * Time.deltaTime;
    }

    void Update()
    {
        projectile.velocity.x += acceleration * Time.deltaTime;
    }
}
