using UnityEngine;

public class ProjectileVerticalBounce : MonoBehaviour
{
    public LayerMask layers;
    public int bouncesBeforeDestruction;
    public float speedPercentKept;
    Projectile projectile;
    int bounces = 0;

    void Start()
    {
        projectile = GetComponent<Projectile>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.isActiveAndEnabled)
        {
            if ((layers & (1 << other.gameObject.layer)) != 0)
            {
                Debug.Log("Layer recognised");
                projectile.velocity = new Vector3(projectile.velocity.x, -projectile.velocity.y / 100 * speedPercentKept, projectile.velocity.z);
                bounces++;
                if (bounces == bouncesBeforeDestruction)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("Layer not in mask");
            }
        }
    }
}
//Current issues: Falls through floor when bouncing too low