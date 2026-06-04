using UnityEngine;

public class TrailController : MonoBehaviour
{
    public TrailRenderer trail;

    public void EnableTrail()
    {
        trail.emitting = true;
    }

    public void DisableTrail()
    {
        trail.emitting = false;
        trail.Clear();
    }
    void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
    }
    public void DisableTrail1()
    {
        Debug.Log("DisableTrail called");
        trail.emitting = false;
        trail.Clear();
    }
}


