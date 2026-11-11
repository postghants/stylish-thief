using UnityEngine;

public class CrimeRing : MonoBehaviour
{
    public int crimeTier;
    public string crimeName;
    public int givenScore;

    private PlayerStateDriver player;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Crime crime;
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
            renderer.enabled = false;
            crime.DoCrimeOfTier(crimeTier, crimeName, givenScore);
        }
    }

    public void Reset()
    {
        renderer.enabled = true;
    }
}
