using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 angleVelocity;
    public Vector3 targetLocation;
    public PlayerStateDriver player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerStateDriver>(); //Yes this is bad. We should make a convenient way to find the player
    }
    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(angleVelocity * Time.deltaTime);
    }
}
