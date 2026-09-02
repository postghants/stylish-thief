using UnityEngine;

public class GrabberController : EnemyController
{
    protected override void Update()
    {
        if (!ctx.activeZone.IsPointInZone(transform.position))
        {
            transform.position = ctx.activeZone.ClosestPoint(transform.position);
        }
        base.Update();
    }
}
