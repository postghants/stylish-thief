using UnityEngine;

public class VerticalPole : MonoBehaviour
{
    PlayerStateDriver player;
    [SerializeField] private Crime crime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.GetComponentInParent<PlayerStateDriver>();
            Vector3 position = transform.position;
            position.y = player.transform.position.y;
            player.transform.position = position;
            player.transform.position += new Vector3(0, .2f, 0);
            player.ctx.rb.isGrounded = false;

            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.poleSpin);
            crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
        }
    }
}
