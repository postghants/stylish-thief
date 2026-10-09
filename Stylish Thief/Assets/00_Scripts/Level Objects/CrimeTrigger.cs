using UnityEngine;

public class CrimeTrigger : MonoBehaviour
{
    Crime crime;
    public int crimeTier;
    public string crimeName;
    public int points;
    public bool useResetCooldown;
    private bool triggered;

    void Start()
    {
        crime = GetComponent<Crime>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (!triggered || !useResetCooldown)
            {
                crime.DoCrimeOfTier(crimeTier, crimeName, points);
                triggered = true;
            }
        }
    }
    public void Reset()
    {
        triggered = false;
    }
}
