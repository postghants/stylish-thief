using System;
using UnityEngine;

public class PlayerMovementSoundEventTriggers : MonoBehaviour
{
    private PlayerStateDriver psd;

    [SerializeField]
    private AK.Wwise.RTPC WwiseVelocityCheck;


    public void Start()
    {
        psd = GetComponentInParent<PlayerStateDriver>();
    }

    public void Update()
    {
        if (psd.ctx != null)
        {
            WwiseVelocityCheck.SetGlobalValue(psd.ctx.rb.velocity.magnitude);
            Debug.Log(psd.ctx.rb.velocity.magnitude);
        }
    }

    public void TriggerWwiseJumpEvent()
    {
        AkUnitySoundEngine.PostEvent("Jump_Event", gameObject);
    }

    public void TriggerWwiseRightFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Right_Footstep_Event", gameObject);
    }

    public void TriggerWwiseLeftFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Left_Footstep_Event", gameObject);
    }

    public void TriggerWwiseLandingEvent()
    {
        AkUnitySoundEngine.PostEvent("Landing_Event", gameObject);
    }

    public void TriggerWwiseDashEvent()
    {
        AkUnitySoundEngine.PostEvent("Dash_Event", gameObject);
    }
}
