using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class SimpleMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Setează aici obiectul CameraPivot (EMPTY) din Player.")]
    public Transform cameraPivot;

    [Header("Look")]
    [Tooltip("Sensibilitatea pentru mouse. Încearcă 0.15 - 0.35 pentru Input System.")]
    public float mouseSensitivity = 0.2f;
    public bool invertY = false;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 5.0f;
    public bool allowSprint = true;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float groundedStickForce = -2f;

    [Header("Crouch")]
    public bool crouchToggle = true;
    public float standingHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float cameraStandingY = 1.6f;
    public float cameraCrouchY = 1.0f;
    public float crouchLerpSpeed = 12f;

    [Header("Options")]
    public bool freezeWhenCursorUnlocked = true;

    private CharacterController controller;
    private float pitch;
    private float yVelocity;
    private bool isCrouching;
    private bool canMove = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraPivot == null)
        {
            Debug.LogError("SimpleMovement: cameraPivot NU este setat. Trage CameraPivot în Inspector.");
        }
    }

    void Start()
    {
        // controller baseline
        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingHeight / 2f, 0f);

        // pivot baseline
        if (cameraPivot != null)
        {
            Vector3 p = cameraPivot.localPosition;
            p.y = cameraStandingY;
            cameraPivot.localPosition = p;
            cameraPivot.localRotation = Quaternion.identity;
        }

        LockCursor(true);
    }

    void Update()
    {
        // dacă UI-ul e deschis, colegul deblochează cursorul -> înghețăm controlul
        if (!canMove || (freezeWhenCursorUnlocked && Cursor.lockState == CursorLockMode.None))
        {
            ApplyGravityOnly();
            ApplyCrouch();
            return;
        }

        HandleLook();
        HandleCrouchInput();
        HandleMove();
        ApplyCrouch();
    }

    // ================= LOOK =================
    void HandleLook()
    {
        if (cameraPivot == null) return;

        Vector2 look = ReadLookInput(); // delta mouse

        // IMPORTANT: Mouse.delta este deja "per frame" -> NU îl mai înmulțim cu Time.deltaTime
        float yaw = look.x * mouseSensitivity;
        float pitchDelta = look.y * mouseSensitivity;

        if (invertY) pitchDelta = -pitchDelta;

        // yaw pe corp
        transform.Rotate(Vector3.up * yaw);

        // pitch pe pivot
        pitch -= pitchDelta;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // ================= MOVE =================
    void HandleMove()
    {
        Vector2 input = ReadMoveInput();

        float speed = walkSpeed;
        if (allowSprint && ReadSprintHeld() && !isCrouching)
            speed = sprintSpeed;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * speed * Time.deltaTime);

        ApplyGravityOnly();
    }

    void ApplyGravityOnly()
    {
        if (controller.isGrounded && yVelocity < 0f)
            yVelocity = groundedStickForce;

        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);
    }

    // ================= CROUCH =================
    void HandleCrouchInput()
    {
        if (crouchToggle)
        {
            if (ReadCrouchPressedThisFrame())
                isCrouching = !isCrouching;
        }
        else
        {
            isCrouching = ReadCrouchHeld();
        }
    }

    void ApplyCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchLerpSpeed);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (cameraPivot != null)
        {
            float targetY = isCrouching ? cameraCrouchY : cameraStandingY;
            Vector3 p = cameraPivot.localPosition;
            p.y = Mathf.Lerp(p.y, targetY, Time.deltaTime * crouchLerpSpeed);
            cameraPivot.localPosition = p;
        }
    }

    // ================= INPUT (Input System + fallback) =================

    Vector2 ReadMoveInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;

            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.wKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;

            Vector2 v = new Vector2(x, y);
            return v.sqrMagnitude > 1f ? v.normalized : v;
        }
#endif
        // fallback vechi (când Keyboard.current e null)
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    Vector2 ReadLookInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
            return Mouse.current.delta.ReadValue();
#endif
        // fallback vechi
        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 10f;
    }

    bool ReadSprintHeld()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            return Keyboard.current.leftShiftKey.isPressed;
#endif
        return Input.GetKey(KeyCode.LeftShift);
    }

    bool ReadCrouchPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            return Keyboard.current.cKey.wasPressedThisFrame;
#endif
        return Input.GetKeyDown(KeyCode.C);
    }

    bool ReadCrouchHeld()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
            return Keyboard.current.cKey.isPressed;
#endif
        return Input.GetKey(KeyCode.C);
    }

    // ================= API =================
    public void SetCanMove(bool enabled)
    {
        canMove = enabled;
        if (!enabled) yVelocity = 0f;
    }

    public void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
