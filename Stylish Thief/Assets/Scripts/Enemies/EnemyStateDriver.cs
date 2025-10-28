using System;
using UnityEngine;

public abstract class EnemyStateDriver : Actor
{
    public abstract void Initialize(PatrolZone zone, PlayerStateDriver player);

    public abstract void SwitchActiveZone(PatrolZone zone);
}

[Serializable]
public class EnemyContext
{
    public PatrolZone activeZone;
    public PlayerStateDriver player;
}
