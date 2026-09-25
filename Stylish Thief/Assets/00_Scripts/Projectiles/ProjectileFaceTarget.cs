using UnityEngine;
using UnityEngine.ProBuilder;

public class ProjectileFaceTarget : MonoBehaviour
{
    public bool lookConstantly;
    public float turnSpeed;
    public bool includeX;
    public bool includeY;
    public bool includeZ;
    Projectile projectile;
    PlayerStateDriver player;
    Vector3 target;

    void Start()
    {
        projectile = GetComponent<Projectile>();
        //projectile.targetLocation = projectile.player.transform.position;
        player = FindFirstObjectByType<PlayerStateDriver>(); //Yes this is bad. We should make a convenient way to find the player
        projectile.transform.LookAt(target);
        Vector3 adjustedTarget = transform.eulerAngles;
        if (!includeX)
        {
            adjustedTarget.x = 0;
        }
        if (!includeY)
        {
            adjustedTarget.y = 0;
        }
        if (!includeZ)
        {
            adjustedTarget.z = 0;
        }
        transform.eulerAngles = adjustedTarget;
    }

    void Update()
    {
        if (lookConstantly)
        {
            if (turnSpeed != 0)
            {
                
            }
            else
            {
                projectile.transform.LookAt(target);
                Vector3 adjustedTarget = transform.eulerAngles;
                if (!includeX)
                {
                    adjustedTarget.x = 0;
                }
                if (!includeY)
                {
                    adjustedTarget.y = 0;
                }
                if (!includeZ)
                {
                    adjustedTarget.z = 0;
                }
                transform.eulerAngles = adjustedTarget;
            }
        }
    }
}
//Bool to include X and Z rotation
//Bool to determine whether it's constant or not
//Float to determine how gradually to face the target