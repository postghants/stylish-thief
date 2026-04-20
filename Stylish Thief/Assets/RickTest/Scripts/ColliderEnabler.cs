using UnityEngine;

public class ColliderEnabler : MonoBehaviour
{
    public bool dontEnableOnStart;
    public bool disableInstead;
    private Collider coll;
    void Start()
    {
        coll = GetComponent<Collider>();
        if (!dontEnableOnStart)
        {
            EnableHitbox();
        }
        if (disableInstead)
        {
            DisableHitbox();
        }
    }
    public void EnableHitbox()
    {
        coll.enabled = true;
    }
    public void DisableHitbox()
    {
        coll.enabled = false;
    }
}
