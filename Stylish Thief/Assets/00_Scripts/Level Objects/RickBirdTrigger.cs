using UnityEngine;

public class RickBirdTrigger : MonoBehaviour
{
    [SerializeField] private Crime crime;

    private RickBird bird;
    [Header("Internal no touchy")]
    [Tooltip("Doesn't have to be set!")] public GameObject player;

    void Start()
    {
        bird = GetComponentInChildren<RickBird>();
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
                    crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
                }
            }
        }
    }
    public void ResetCrime()
    {
        bird.Reset();
    }
}
