using UnityEngine;
using UnityEngine.Audio;

public class SingleChipSpitter : MonoBehaviour
{
    public ReturnToBase chip;
    public float delay;
    float counting;
    public float projectileSpeed;

    bool warned;
    public AudioSource warning;
    public AudioSource pop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chip = GetComponentInChildren<ReturnToBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!warned && counting > 2)
        {
            warned = true;
            warning.Play();
        }
        if (counting > delay)
        {
            counting = 0f;
            SpitAChip();
            warned = false;
            pop.Play();
        }
        counting += Time.deltaTime;
    }

    public void SpitAChip()
    {
        chip.InitiateLaunch(projectileSpeed);
    }
}
