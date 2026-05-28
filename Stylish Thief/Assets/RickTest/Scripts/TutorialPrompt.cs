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
    public bool StopTime;
    public bool destroyOnCompletion;
    bool completed;
    [SerializeField] private GameObject canvas;
    public Vector3 facingDirection;
    public bool changeDirection;
    public bool changeStateToIdle;
    //public string stateName;
    public Vector3 speed;
    public bool setSpeed;
    public bool resetGrab;

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
                    if (inputName != "")
                    {
                        if (input.WasPerformedThisFrame())
                        {
                            EndPrompt();
                        }
                    }
                }
            }
        }
    }
    public void InitialisePrompt()
    {
        if (!completed)
        {
            canvas.SetActive(true);
            if (StopTime)
            {
                Time.timeScale = 0;
            }
            if (changeDirection)
            {
                player.ctx.facing = facingDirection;
            }
            if (changeStateToIdle)
            {
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.grounded.idle);
            }
            if (setSpeed)
            {
                player.SetVelocity(speed);
            }
            if (resetGrab)
            {
                player.ctx.hasGrabbed = false;
            }
        }
    }
    public void EndPrompt()
    {
        canvas.SetActive(false);
        completed = true;
        Time.timeScale = 1;
        player = null;
        if (destroyOnCompletion)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
        }
    }
    public void MarkUncompleted()
    {
        completed = false;
    }
}
//Disable every input except the target ones
//Set the player's state to idle or airborne
//Set the player's speed to 0
//Set the player's facing direction to whatever forward is