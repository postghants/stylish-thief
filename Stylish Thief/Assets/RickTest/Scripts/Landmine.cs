using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    private MeshRenderer mesh;

    bool triggered;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            if (currentTime < timeBeforeBoom)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
                mesh.enabled = false;
                explosion.SetActive(true);
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime, gameObject);
                Destroy(gameObject, LengthOfBoom);
            }
        }
        
    }

    public void Explode()
    {
        triggered = true;
        Debug.Log("BEEP");
    }
}
