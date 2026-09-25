using UnityEngine;

public class ProjectileTimedSelfDestruct : MonoBehaviour
{
    public float selfDestructTime;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isActiveAndEnabled)
        {
            if (selfDestructTime != 0)
            {
                if (timer < selfDestructTime)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
