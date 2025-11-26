using UnityEngine;

public class Valuable : MonoBehaviour, IGrabbable
{
    public float Value;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStateDriver player = collision.gameObject.GetComponentInParent<PlayerStateDriver>();
        if (player != null)
        {
            Collect(player.ctx);
        }
    }

    private void Collect(PlayerContext ctx)
    {
        ctx.scoreManager.AddScore(Value);

    }

}
