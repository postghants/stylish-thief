using UnityEngine;

public class Collection : MonoBehaviour
{
    public float rotationSpeed;

    [SerializeField] private Crime crime;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Collider collider;
    [SerializeField] private Light light;
    [SerializeField] private GameObject particles;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    public void Collect()
    {
        renderer.enabled = false;
        collider.enabled = false;
        light.enabled = false;
        particles.SetActive(false);

        crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
    }
    public void ResetCrime()
    {
        renderer.enabled = true;
        collider.enabled = true;
        light.enabled = true;
        particles.SetActive(true);
    }
}
