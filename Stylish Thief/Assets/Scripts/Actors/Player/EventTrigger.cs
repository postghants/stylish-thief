using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EventTrigger : MonoBehaviour
{
    public LayerMask mask;
    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other) { if(mask == (mask | (1 << other.gameObject.layer))) OnTriggerEnterEvent?.Invoke(other); }
    private void OnTriggerExit(Collider other) { if (mask == (mask | (1 << other.gameObject.layer))) OnTriggerExitEvent?.Invoke(other); }
}
