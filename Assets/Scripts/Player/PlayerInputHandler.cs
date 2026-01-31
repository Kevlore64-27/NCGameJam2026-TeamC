using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Move";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string crouch = "Crouch";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction crouchAction;

    public Vector2 MovementInput { get; set; }
    public Vector2 RotationInput { get; set; }
    public bool JumpTriggered { get; set; }
    public bool SprintTriggered { get; set; }
    public bool InteractTriggered { get; set; }
    public bool CrouchTriggered { get; set; }

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);
        
        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        interactAction = mapReference.FindAction(interact);
        crouchAction = mapReference.FindAction(crouch);

        SubscribeActionValuesToInputEvents();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            return;
        }

        var map = playerControls.FindActionMap(actionMapName);
        if (map != null)
        {
            map.Enable();
        }
    }

    private void OnDisable()
    {
        if (playerControls == null)
        {
            return;
        }

        var map = playerControls.FindActionMap(actionMapName);
        if (map != null)
        {
            map.Disable();
        }
    }

    private void Update()
    {
        if (rotationAction == null)
        {
            return;
        }

        RotationInput = rotationAction.ReadValue<Vector2>();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        if (movementAction != null)
        {
            movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
            movementAction.canceled += inputInfo => MovementInput = Vector2.zero;
        }
        else Debug.LogError("Movement action not found!");

        if (rotationAction != null)
        {
            rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
            rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;
        }
        else Debug.LogError("Rotation action not found!");

        if (jumpAction != null)
        {
            jumpAction.performed += inputInfo => JumpTriggered = true;
            jumpAction.canceled += inputInfo => JumpTriggered = false;
        }
        else Debug.LogError("Jump action not found!");

        if (interactAction != null)
        {
            interactAction.performed += inputInfo => InteractTriggered = true;
            interactAction.canceled += inputInfo => InteractTriggered = false;
        }
        else Debug.LogError("Interact action not found!");

        if (sprintAction != null)
        {
            sprintAction.performed += inputInfo => SprintTriggered = true;
            sprintAction.canceled += inputInfo => SprintTriggered = false;
        }
        else Debug.LogError("Sprint action not found!");

        if (interactAction != null)
        {
            crouchAction.performed += inputInfo => CrouchTriggered = true;
            crouchAction.canceled += inputInfo => CrouchTriggered = false;
        }
        else Debug.LogError("Crouch action not found!");
    }
}
