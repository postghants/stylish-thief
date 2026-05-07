using UnityEngine;
using FMODUnity;

public class PlayerAnimEventHandler : MonoBehaviour
{

    [SerializeField] EventReference footstepLeftEvent;
    [SerializeField] EventReference footstepRightEvent;
    [SerializeField] EventReference onJumpEvent;
    [SerializeField] EventReference onGrabEvent;
    [SerializeField] EventReference onBumpEvent;

    public void FootstepLeft()
    {
        RuntimeManager.PlayOneShotAttached(footstepLeftEvent, gameObject);
    }

    public void FootstepRight()
    {
        RuntimeManager.PlayOneShotAttached(footstepRightEvent, gameObject);
    }

    public void Jump()
    {
        RuntimeManager.PlayOneShotAttached(onJumpEvent, gameObject);
    }

    public void Grab()
    {
        RuntimeManager.PlayOneShotAttached(onGrabEvent, gameObject);
    }

    public void Bump()
    {
        RuntimeManager.PlayOneShotAttached(onBumpEvent, gameObject);
    }
}
