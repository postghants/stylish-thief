using UnityEngine;
using UnityEngine.InputSystem;

public class InputDisabler : MonoBehaviour
{
    private InputAction jump;
    private InputAction grab;
    private InputAction pound;
    private InputAction bumperLeft;
    private InputAction bumperRight;
    private InputAction move;
    private InputAction look;
    void Start()
    {
        jump = InputSystem.actions.FindAction("Jump");
        grab = InputSystem.actions.FindAction("Grab");
        pound = InputSystem.actions.FindAction("Pound");
        bumperLeft = InputSystem.actions.FindAction("BumperLeft");
        bumperRight = InputSystem.actions.FindAction("BumperRight");
        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
    }
    public void EnableEverything()
    {
        EnableJumpInput();
        EnableGrabInput();
        EnablePoundInput();
        EnableBumperLeftInput();
        EnableBumperRightInput();
        EnableMoveInput();
        EnableLookInput();
    }
    public void EnableJumpInput()
    {
        jump.Enable();
    }
    public void DisableJumpInput()
    {
        jump.Disable();
    }
    public void EnableGrabInput()
    {
        grab.Enable();
    }
    public void DisableGrabInput()
    {
        grab.Disable();
    }
    public void EnablePoundInput()
    {
        pound.Enable();
    }
    public void DisablePoundInput()
    {
        pound.Disable();
    }
    public void EnableBumperLeftInput()
    {
        bumperLeft.Enable();
    }
    public void DisableBumperLeftInput()
    {
        bumperLeft.Disable();
    }
    public void EnableBumperRightInput()
    {
        bumperRight.Enable();
    }
    public void DisableBumperRightInput()
    {
        bumperRight.Disable();
    }
    public void EnableMoveInput()
    {
        move.Enable();
    }
    public void DisableMoveInput()
    {
        move.Disable();
    }
    public void EnableLookInput()
    {
        look.Enable();
    }
    public void DisableLookInput()
    {
        look.Disable();
    }
}
