using UnityEngine;

public class CrimeRing : MonoBehaviour
{
    private PlayerStateDriver player;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Crime crime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            renderer.enabled = false;
            crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
        }
    }

    public void ResetCrime()
    {
        renderer.enabled = true;
    }
}
