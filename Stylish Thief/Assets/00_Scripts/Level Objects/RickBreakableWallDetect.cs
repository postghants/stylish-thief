using UnityEngine;

public class RickBreakableWallDetect : MonoBehaviour
{
    [SerializeField] BoxCollider coll;
    PlayerStateDriver player;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                coll.enabled = false;
            }
            else
            {
                coll.enabled = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();

            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                coll.enabled = false;
            }
            else
            {
                coll.enabled = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            coll.enabled = true;
        }
    }
}
