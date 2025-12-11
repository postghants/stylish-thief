using UnityEngine;

public class Valuable : MonoBehaviour, IGrabbable
{
    public float Value;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStateDriver player = other.GetComponentInParent<PlayerStateDriver>();
        if (player != null)
        {
            Collect(player.ctx);
        }
    }

    private void Collect(PlayerContext ctx)
    {
        if (CrimeSpreeManager.instance != null) CrimeSpreeManager.instance.CollectedValuable(this);

        if (transform.parent != null && transform.parent.TryGetComponent(out ValuableParent parent))
        {
            foreach(EnemySpawn spawner in parent.spawners)
            {
                spawner.SpawnRandomEnemy();
            }
        }

    }
}
