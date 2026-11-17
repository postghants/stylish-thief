using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public LayerMask mask;
    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;
    public delegate void ColliderEvent(Collider other);
    public ColliderEvent OnTriggerExitDelegate;
    public ColliderEvent OnTriggerEnterDelegate;

    private void OnTriggerEnter(Collider other) { if(mask == (mask | (1 << other.gameObject.layer))) OnTriggerEnterEvent?.Invoke(other); OnTriggerEnterDelegate?.Invoke(other); }
    private void OnTriggerExit(Collider other) { if (mask == (mask | (1 << other.gameObject.layer))) OnTriggerExitEvent?.Invoke(other); OnTriggerExitDelegate?.Invoke(other); }
}
