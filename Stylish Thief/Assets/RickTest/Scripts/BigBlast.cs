using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BigBlast : MonoBehaviour
{
    [SerializeField] private DecalProjector decal;
    [SerializeField] private CapsuleCollider coll;
    [SerializeField] private MeshRenderer mesh;
    public float telegraphTime;
    float currentTelegraphTime;
    bool fired;
    public float lingerTime;
    float currentLingerTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll.enabled = false;
        mesh.enabled = false;
        currentTelegraphTime = 0;
    }
    private void OnEnable()
    {
        coll.enabled = false;
        mesh.enabled = false;
        currentTelegraphTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fired)
        {
            if (currentTelegraphTime < telegraphTime)
            {
                currentTelegraphTime += Time.deltaTime;
            }
            else
            {
                currentTelegraphTime = 0;
                coll.enabled = true;
                mesh.enabled = true;
                fired = true;
            }
        }
        else
        {
            if (currentLingerTime < lingerTime)
            {
                currentLingerTime += Time.deltaTime;
            }
            else
            {
                if (transform.parent != null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    FullReset();
                }
            }
        }
    }
    private void FullReset()
    {
        coll.enabled = false;
        mesh.enabled = false;
        fired = false;
        currentLingerTime = 0;
        currentTelegraphTime = 0;
        gameObject.SetActive(false);
    }
}