using System.Collections;
using TMPro;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // First person controllers references
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TMP_Text hoverText;
    [SerializeField] private PlayerGrabSystem playerPickup;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchTransitionSpeed = 10.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Pick Up Parameters")]
    [SerializeField] private float pickUpDistance = 3.0f;
    [SerializeField] private LayerMask pickUpLayer;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip footStepSfx;

    [Header("Audio Parameters")]
    [SerializeField] private float walkStepDelay = 0.6f;
    [SerializeField] private float sprintStepDelay = 0.3f;
    [SerializeField] private float crouchStepDelay = 0.9f;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverDistance = 3.0f;
    [SerializeField] private LayerMask hoverLayer;

    [Header("Dialogue")]
    [SerializeField] GameObject dialogueBox;
    public Vector3 PlayerVelocity { get; private set; }

    // private variables
    private bool isCrouching = false;
    private bool isSprinting = false;

    private Vector3 currentMovement;
    private float currentSpeed;

    private float verticalRotation;

    private float standingHeight;
    private float currentHeight;
    private float targetHeight;

    private Vector3 initCamPos;

    public bool rotationLock = false;
    [SerializeField] GameObject doctor;

    private Vector3 lastPos;
    private bool isHolding;

    private float mouseXRotation => playerInputHandler.RotationInput.x * mouseSensitivity;
    private float mouseYRotation => playerInputHandler.RotationInput.y * mouseSensitivity;

    private void Awake()
    {
        if (hoverText != null)
        {
            hoverText.text = "";
        }
    }

    private void OnEnable()
    {
        PickupItem.OnItemGrabbed += HandleItemGrabbed;
        PickupItem.OnItemDropped += HandleItemDropped;
    }

    private void OnDisable()
    {
        PickupItem.OnItemGrabbed -= HandleItemGrabbed;
        PickupItem.OnItemDropped -= HandleItemDropped;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        standingHeight = currentHeight = characterController.height;
        targetHeight = standingHeight;
        initCamPos = cam.transform.localPosition;
        StartCoroutine(PlayFootSteps());
    }

    private void Update()
    {
        UpdateHoverText();
        HandleInteract();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        PlayerVelocity = (transform.position - lastPos) / Time.fixedDeltaTime;
        lastPos = transform.position;

        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleRotation();
    }

    private void HandleItemGrabbed(PickupItem item)
    {
        isHolding = true;
        if (hoverText != null)
        {
            hoverText.text = "";
        }
    }

    private void HandleItemDropped(PickupItem item)
    {
        isHolding = false;
    }

    private void UpdateHoverText()
    {
        if (cam == null || hoverText == null) return;

        if (isHolding)
        {
            hoverText.text = "";
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hoverDistance, hoverLayer))
        {
            if (hit.collider.CompareTag("Doctor"))
            {
                HoverName hoverName = hit.collider.GetComponentInParent<HoverName>();
                if (hoverName != null)
                {
                    hoverText.text = hoverName.DisplayName;
                    return;
                }
            }
            else
            {
                Objectiveitem objItem = hit.collider.GetComponentInParent<Objectiveitem>();
                if (objItem != null && objItem.IsCompleted)
                {
                    hoverText.text = "";
                    return;
                }

                HoverName hoverName = hit.collider.GetComponentInParent<HoverName>();
                if (hoverName != null)
                {
                    hoverText.text = hoverName.DisplayName;
                    return;
                }
            }

        }

        hoverText.text = "";
    }

    private Vector3 CalculateWorldDirection()
    {
        var inputDir = new Vector3(playerInputHandler.MovementInput.x, 0.0f, playerInputHandler.MovementInput.y);
        var worldDirection = transform.TransformDirection(inputDir);

        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTriggered && !isCrouching)
            {
                playerInputHandler.JumpTriggered = false;
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        if (playerInputHandler.SprintTriggered && !isCrouching)
        {
            isSprinting = true;
            currentSpeed = walkSpeed * sprintMultiplier;
        }
        else if (!isSprinting && playerInputHandler.CrouchTriggered)
        {
            currentSpeed = walkSpeed * crouchSpeedMultiplier;
        }
        else
        {
            isSprinting = false;
            currentSpeed = walkSpeed;
        }

        var worldDir = CalculateWorldDirection();
        currentMovement.x = worldDir.x * currentSpeed;
        currentMovement.z = worldDir.z * currentSpeed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0.0f, rotationAmount, 0.0f);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        cam.transform.localRotation = Quaternion.Euler(verticalRotation, 0.0f, 0.0f);
    }

    private void HandleRotation()
    {
        if (!rotationLock)
        {
            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }
        else
        {
            transform.LookAt(doctor.transform.position);
        }
    }

    private void HandleInteract()
    {
        if (playerInputHandler.InteractTriggered)
        {
            playerInputHandler.InteractTriggered = false;

            var ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3.0f))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    var door = hit.collider.GetComponentInParent<Door>();
                    if (door != null)
                    {
                        door.Interact();
                    }
                }
                else if (hit.collider.CompareTag("Doctor"))
                {
                    dialogueBox.SetActive(true);
                }
            }
        }
    }

    private void HandleCrouch()
    {
        if (playerInputHandler.CrouchTriggered && !isSprinting)
        {
            isCrouching = true;
            targetHeight = crouchHeight;
        }
        else
        {
            isCrouching = false;
            var rayCastPos = transform.position + new Vector3(0.0f, currentHeight / 2, 0.0f);
            var rayCastDist = 0.2f;

            if (Physics.Raycast(rayCastPos, Vector3.up, out RaycastHit hit, rayCastDist))
            {
                var distToCeiling = hit.point.y - rayCastPos.y;
                targetHeight = Mathf.Max(currentHeight + distToCeiling - 0.1f, crouchHeight);
            }
            else
            {
                targetHeight = standingHeight;
            }
        }

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        var halfHeight = new Vector3(0.0f, (standingHeight - currentHeight) / 2.0f, 0.0f);
        var newCamPos = initCamPos - halfHeight;

        cam.transform.localPosition = newCamPos;
        characterController.height = currentHeight;
    }

    private IEnumerator PlayFootSteps()
    {
        while (true)
        {
            if (playerInputHandler.MovementInput.magnitude > 0.1f)
            {
                AudioManager.Instance.PlaySfx(footStepSfx);
            }

            if (isSprinting)
            {
                yield return new WaitForSeconds(sprintStepDelay);
            }
            else if (isCrouching)
            {
                yield return new WaitForSeconds(crouchStepDelay);
            }
            else
            {
                yield return new WaitForSeconds(walkStepDelay);
            }
        }
    }
}
