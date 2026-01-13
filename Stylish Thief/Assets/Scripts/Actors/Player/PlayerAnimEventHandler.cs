using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEventHandler : MonoBehaviour
{
    public UnityEvent OnFootstep;

    public void Footstep()
    {
        OnFootstep?.Invoke();
    }
}
