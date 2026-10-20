using UnityEngine;

public class BarSwing : MonoBehaviour
{
    private PlayerStateDriver player;
    private Vector3 entryRotation;
    private Quaternion barStartRotation;
    private Transform animator;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            animator = player.ctx.anim.transform;

            player.transform.position = transform.position;
            entryRotation = animator.transform.rotation.eulerAngles;
            barStartRotation = transform.rotation;
            CalculateAngles();
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.barSwing);
        }
    }
    private void CalculateAngles()
    {
        float difference = barStartRotation.eulerAngles.y - entryRotation.y;

        if ((difference > 90 && difference < 270) || (difference < -90 && difference > -270))
        {
            animator.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        }
        else
        {
            animator.transform.rotation = transform.rotation;
        }
    }
}
    //Place player in middle of bar
    //Force player to look in the direction the bar looks
    //Turn the player around if coming from other direction
//Give player new state