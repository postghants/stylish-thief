using UnityEngine;
using FMODUnity;

public class PlayerAnimEventHandler : MonoBehaviour
{

    [SerializeField] EventReference footstepLeftEvent;
    [SerializeField] EventReference footstepRightEvent;
    [SerializeField] EventReference onJumpEvent;

    public void FootstepLeft()
    {
        RuntimeManager.PlayOneShotAttached(footstepLeftEvent, gameObject);
    }

    public void FootstepRight()
    {
        RuntimeManager.PlayOneShotAttached(footstepRightEvent, gameObject);
    }

    public void OnJump()
    {
        RuntimeManager.PlayOneShotAttached(onJumpEvent, gameObject);
    }
}
