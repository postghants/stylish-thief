using UnityEngine;

public class SpeedSetter : MonoBehaviour
{
    public bool setDirection;
    public bool replaceSpeed;
    public float speed;

    private PlayerStateDriver player;
    [SerializeField] private Crime crime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (replaceSpeed)
            {
                if (setDirection)
                {
                    player.ctx.rb.velocity = new Vector3(transform.forward.x, 0, transform.forward.z) * speed;
                }
                else
                {
                    player.ctx.rb.velocity = player.ctx.moveDirection * speed;
                }
            }
            else
            {
                if (setDirection)
                {
                    player.ctx.rb.velocity = new Vector3(transform.forward.x, 0, transform.forward.z) * speed;
                }
                player.ctx.rb.velocity += player.ctx.moveDirection * speed;
            }
        }
    }
}
