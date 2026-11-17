using UnityEngine;

public class Water : MonoBehaviour
{
    private PlayerStateDriver player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerPound")
            {

            }
            else
            {
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.drowning);
            }
        }
    }
}
