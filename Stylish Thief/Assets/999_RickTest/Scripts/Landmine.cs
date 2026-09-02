using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using FMODUnity;

public class Landmine : MonoBehaviour
{
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Header("Setup")]
    [Tooltip("How long does it take for it to blow up after activation?")] public float timeBeforeBoom;
    float currentTime;

    [Tooltip("How long does the explosion last?")] public float LengthOfBoom;
    
    [Header("References, don't touch")]
    [Tooltip("Internal no touchy")] [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject visual;
    private Light light;
    [SerializeField] private GameObject vfxPrefab;

    [SerializeField] EventReference onExplode;

    bool triggered;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
        light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            light.enabled = true;
            if (currentTime < timeBeforeBoom)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
                visual.SetActive(false);
                explosion.SetActive(true);
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime, gameObject);
                Destroy(gameObject, LengthOfBoom);
                GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            }
        }
        
    }

    public void Explode()
    {
        triggered = true;
        Debug.Log("BEEP");
        RuntimeManager.PlayOneShotAttached(onExplode, gameObject);
    }
}
