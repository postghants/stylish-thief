using FMODUnity;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public float rotationSpeed;
    public float givenTime;
    public float respawnTime;
    public int givenScore;
    public string crimeName;
    public bool theftIsMinor;

    [SerializeField] EventReference onCollectEvent;

    float currentTime;

    Renderer renderer;
    Collider collider;
    Light light;
    [SerializeField] GameObject particles;
    private Crime crime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        collider = GetComponentInChildren<Collider>();
        light = GetComponentInChildren<Light>();
        particles.SetActive(true);
        crime = GetComponent<Crime>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    public void Collect()
    {
        if (CrimeSpreeManager.instance != null)
        {
            CrimeSpreeManager.instance.ChaseTimer += givenTime;
        }
        renderer.enabled = false;
        collider.enabled = false;
        light.enabled = false;
        particles.SetActive(false);
        currentTime = 0;
        RuntimeManager.PlayOneShotAttached(onCollectEvent, gameObject);

        if (theftIsMinor)
        {
            crime.DoMinorCrime(crimeName, givenScore);
        }
        else
        {
            crime.DoMajorCrime(crimeName, givenScore);
        }
    }
    public void Reset()
    {
        renderer.enabled = true;
        collider.enabled = true;
        light.enabled = true;
        particles.SetActive(true);
    }
}
