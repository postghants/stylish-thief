using UnityEngine;

public class PlayerJuice
{
    public void Init(PlayerContext ctx)
    {
        // Add Events
        ctx.OnJump += OnJump;
        ctx.OnGrab += OnGrab;
        ctx.OnLand += OnLand;
    }

    private void OnJump(PlayerContext ctx)
    {
        ctx.particleManager.StartGroup("Jump");
    }

    private void OnGrab(PlayerContext ctx)
    {
        ctx.particleManager.StartGroup("Grab");
    }

    private void OnLand(PlayerContext ctx)
    {
        ctx.particleManager.StartGroup("Land");
    }
}
