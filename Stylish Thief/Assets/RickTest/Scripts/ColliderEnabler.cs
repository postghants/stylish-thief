using UnityEngine;

public class ColliderEnabler : MonoBehaviour
{
    [Tooltip("Whether to enable the collider immediately or not")] public bool dontEnableOnStart;
    [Tooltip("Whether to do the opposite and actually disable it instead")] public bool disableInstead;
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
