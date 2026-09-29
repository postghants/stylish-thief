using UnityEngine;


public class Teleporter : MonoBehaviour
{
    public Transform Destination;
    PlayerStateDriver player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            player.transform.position = Destination.position;
        }
    }
}