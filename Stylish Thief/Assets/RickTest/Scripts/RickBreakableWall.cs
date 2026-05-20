using UnityEngine;

public class RickBreakableWall : MonoBehaviour
{
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    PlayerStateDriver player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();

            //Check if current state is something specific
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
