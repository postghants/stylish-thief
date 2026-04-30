using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPrompt : MonoBehaviour
{
    [Header("Input")]
    private InputAction input;
    private InputAction input2;
    public string inputName;
    public string input2Name;
    bool completed;

    PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputSystem.actions.FindAction(inputName);
        input2 = InputSystem.actions.FindAction(input2Name);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (input2Name == null)
            {
                if (input.IsPressed())
                {
                    Debug.Log("input");
                }
                if (input2.IsPressed())
                {
                    Debug.Log("input2");
                }
                if (input.IsInProgress() && input2.IsInProgress())
                {
                    EndPrompt();
                }
            }
        }
        
        /*else
        {
            if (input.WasPerformedThisFrame())
            {
                EndPrompt();
            }
        }*/
    }
    public void InitialisePrompt()
    {
        if (!completed)
        {
            Time.timeScale = 0;
        }
    }
    public void EndPrompt()
    {
        Time.timeScale = 1;
        player = null;
    }
    public void DisableEverything()
    {
            player.ctx.disableGrab = true;
            player.ctx.disableJump = true;
            player.ctx.disablePound = true;
            player.ctx.disableRoll = true;
            player.ctx.disableRollJump = true;
            player.ctx.disableVault = true;
            player.ctx.disableVaultJump = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            //DisableEverything();
        }
    }
}
//Disable every input except the target ones
//Set the player's state to idle or airborne
//Set the player's speed to 0
//Set the player's facing direction to whatever forward is