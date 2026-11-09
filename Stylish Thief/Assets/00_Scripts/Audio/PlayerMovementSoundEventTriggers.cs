using System;
using Unity.VisualScripting;
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
            //Debug.Log(psd.ctx.rb.velocity.magnitude);
        }
    }

    public void TriggerWwiseJumpEvent()
    {
        AkUnitySoundEngine.PostEvent("Jump_Event", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseRightFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Right_Footstep_Event", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseLeftFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Left_Footstep_Event", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseLandingEvent()
    {
        AkUnitySoundEngine.PostEvent("Landing_Event", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseDashEvent()
    {
        AkUnitySoundEngine.PostEvent("Dash_Event", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseRollJumpBuildupEvent()
    {
        AkUnitySoundEngine.PostEvent("Stop_Rolling_Event", psd.ctx.player.gameObject);
        AkUnitySoundEngine.PostEvent("Roll_Jump_Buildup", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseRollJumpLaunchEvent()
    {
        AkUnitySoundEngine.PostEvent("Roll_Jump_Launch", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseRollJumpSplat()
    {
        AkUnitySoundEngine.PostEvent("Player_Splat", psd.ctx.player.gameObject);
    }

    public void TriggerWwiseSplatRecover()
    {
        AkUnitySoundEngine.PostEvent("Splat_Recover", psd.ctx.player.gameObject);
    }
}
