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
    public bool has2Inputs;
    public bool destroyOnCompletion;
    bool completed;
    InputDisabler disabler;

    PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputSystem.actions.FindAction(inputName);
        input2 = InputSystem.actions.FindAction(input2Name);
        disabler = GetComponent<InputDisabler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!completed)
        {
            if (player != null)
            {
                if (has2Inputs)
                {
                    if (input.inProgress && input2.inProgress)
                    {
                        EndPrompt();
                    }
                }
                else
                {
                    if (input.WasPerformedThisFrame())
                    {
                        EndPrompt();
                    }
                }
            }
        }
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
        completed = true;
        Time.timeScale = 1;
        player = null;
        disabler.EnableEverything();
        if (destroyOnCompletion)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
        }
    }
}
//Disable every input except the target ones
//Set the player's state to idle or airborne
//Set the player's speed to 0
//Set the player's facing direction to whatever forward is