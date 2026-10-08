using UnityEngine;

public class RickBirdTrigger : MonoBehaviour
{
    //public PlayerStateDriver player;
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public int givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;

    private RickBird bird;
    [Header("Internal no touchy")]
    [Tooltip("Doesn't have to be set!")] public GameObject player;
    private Crime crimeScript;

    void Start()
    {
        bird = GetComponentInChildren<RickBird>();
        crimeScript = GetComponent<Crime>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (bird.verticalSpeed < .1f)
        {
            if (other.gameObject.layer == 6)
            {
                player = other.gameObject;
                if (player != null)
                {
                    bird.FlyAway(player.transform);
                    crimeScript.DoMiniCrime(crime, givenScore);
                }
            }
        }
    }
    public void Reset()
    {
        bird.Reset();
    }
}
