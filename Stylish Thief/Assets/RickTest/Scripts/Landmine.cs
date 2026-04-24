using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float timeBeforeBoom;
    float currentTime;

    public float LengthOfBoom;

    [SerializeField] private GameObject explosion;
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
