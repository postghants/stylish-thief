using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float turnSpeedHorizontal;
    [SerializeField] private float turnSpeedVertical;

    private InputAction lookAction;
    private Camera cam;

    private void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
