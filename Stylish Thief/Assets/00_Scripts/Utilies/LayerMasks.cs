using UnityEngine;

public static class LayerMasks
{
    public static LayerMask movingPlatformPushed = LayerMask.GetMask("Player");
    public static LayerMask actorCollision = LayerMask.GetMask("Default", "LevelAssets");
}
