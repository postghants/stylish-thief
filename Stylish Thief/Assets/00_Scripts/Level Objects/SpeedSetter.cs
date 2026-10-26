using UnityEngine;

public class SpeedSetter : MonoBehaviour
{
    public bool setDirection;
    public bool replaceSpeed;
    public float speed;

    private PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
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
