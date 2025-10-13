using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other) { OnTriggerEnterEvent?.Invoke(other); }
    private void OnTriggerExit(Collider other) { OnTriggerExitEvent?.Invoke(other); }
}
