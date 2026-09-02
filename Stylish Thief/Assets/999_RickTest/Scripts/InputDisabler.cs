using UnityEngine;
using UnityEngine.InputSystem;

public class InputDisabler : MonoBehaviour
{
    private InputAction jumpAction;
    private InputAction grabAction;
    private InputAction poundAction;
    private InputAction bumperLeftAction;
    private InputAction bumperRightAction;
    private InputAction moveAction;
    private InputAction lookAction;
    public bool jump;
    public bool grab;
    public bool pound;
    public bool bumperLeft;
    public bool bumperRight;
    public bool move;
    public bool look;
    void Start()
    {
        SetActions();
    }
    private void SetActions()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        grabAction = InputSystem.actions.FindAction("Grab");
        poundAction = InputSystem.actions.FindAction("Pound");
        bumperLeftAction = InputSystem.actions.FindAction("BumperLeft");
        bumperRightAction = InputSystem.actions.FindAction("BumperRight");
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void DisableSelection()
    {
        SetActions();
        EnableEverything();
        if (jump) { DisableJumpInput(); }
        if (grab) { DisableGrabInput(); }
        if (pound) { DisablePoundInput(); }
        if (bumperLeft) { DisableBumperLeftInput(); }
        if (bumperRight) { DisableBumperRightInput(); }
        if (move) { DisableMoveInput(); }
        if (look) { DisableLookInput(); }
    }
    public void EnableEverything()
    {
        SetActions();
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
        jumpAction.Enable();
    }
    public void DisableJumpInput()
    {
        jumpAction.Disable();
    }
    public void EnableGrabInput()
    {
        grabAction.Enable();
    }
    public void DisableGrabInput()
    {
        grabAction.Disable();
    }
    public void EnablePoundInput()
    {
        poundAction.Enable();
    }
    public void DisablePoundInput()
    {
        poundAction.Disable();
    }
    public void EnableBumperLeftInput()
    {
        bumperLeftAction.Enable();
    }
    public void DisableBumperLeftInput()
    {
        bumperLeftAction.Disable();
    }
    public void EnableBumperRightInput()
    {
        bumperRightAction.Enable();
    }
    public void DisableBumperRightInput()
    {
        bumperRightAction.Disable();
    }
    public void EnableMoveInput()
    {
        moveAction.Enable();
    }
    public void DisableMoveInput()
    {
        moveAction.Disable();
    }
    public void EnableLookInput()
    {
        lookAction.Enable();
    }
    public void DisableLookInput()
    {
        lookAction.Disable();
    }
}
