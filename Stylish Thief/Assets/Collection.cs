using UnityEngine;

public class Collection : MonoBehaviour
{
    public float rotationSpeed;
    public float givenTime;
    public float respawnTime;
    public float givenScore;
    public string crimeName;
    public bool theftIsMinor;

    float currentTime;

    Renderer renderer;
    Collider collider;
    Light light;
    [SerializeField] GameObject particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        collider = GetComponentInChildren<Collider>();
        light = GetComponentInChildren<Light>();
        particles.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (currentTime <= respawnTime)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            renderer.enabled = true;
            collider.enabled = true;
            light.enabled = true;
            particles.SetActive(true);
        }
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
        if (theftIsMinor)
        {
            CrimeSpreeManager.instance.DoMinorTheftCrime(givenScore, crimeName, null);
        }
        else
        {
            CrimeSpreeManager.instance.DoTheftCrime(givenScore, crimeName, gameObject);
        }
    }
}
