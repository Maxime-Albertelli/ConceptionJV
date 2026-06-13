using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private InputAction analogMoveAction; // Vector2 from GamePad
    private InputAction moveAction; // WASD or Arrow Keys | Vector2
    private InputAction jumpAction; // Button
    private InputAction dashAction; // Button
    private InputAction diveAction; // Button
    private InputAction spinAction; // Button
    private InputAction fastFallAction; // Button
    private bool isHooked;
    
    private string currentControlScheme = "Game";

    public Vector2 Move
    {
        get
        {
            if (analogMoveAction != null)
            {
                Vector2 analogMove = analogMoveAction.ReadValue<Vector2>();
                if (analogMove.sqrMagnitude > 0.0001f)
                {
                    return analogMove;
                }
            }

            if (moveAction != null)
            {
                if (moveAction.expectedControlType == "Vector2")
                {
                    return moveAction.ReadValue<Vector2>();
                }
            }

            return Vector2.zero;
        }
    }
    public bool JumpHeld => jumpAction != null && jumpAction.IsPressed();
    public bool FastFallHeld => fastFallAction != null && fastFallAction.IsPressed();
    public bool DiveHeld => diveAction != null && diveAction.IsPressed();

    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool DashPressed { get; private set; }
    public bool DivePressed { get; private set; }
    public bool SpinPressed { get; private set; }

    private void OnEnable()
    {
        playerInput ??= GetComponent<PlayerInput>();
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.LogError("PlayerInputReader requires a PlayerInput with an InputActionAsset.");
            return;
        }
        InputActionMap map = playerInput.actions.FindActionMap(currentControlScheme, false);
        if (map == null)
        {
            Debug.LogError("Input action map '" + currentControlScheme + "' was not found.");
            return;
        }

        moveAction = map.FindAction("Move", false);
        analogMoveAction = map.FindAction("AnalogMove", false);
        jumpAction = map.FindAction("Jump", false);
        dashAction = map.FindAction("Dash", false);
        diveAction = map.FindAction("Dive", false);
        spinAction = map.FindAction("Spin", false);
        fastFallAction = map.FindAction("FastFall", false);

        HookEvents(true);
    }

    private void OnDisable()
    {
        HookEvents(false);
    }


    private void HookEvents(bool subscribe)
    {
        if (isHooked == subscribe) return;

        if (jumpAction != null)
        {
            if (subscribe)
            {
                jumpAction.performed += OnJumpPerformed;
                jumpAction.canceled += OnJumpCanceled;
            }
            else
            {
                jumpAction.performed -= OnJumpPerformed;
                jumpAction.canceled -= OnJumpCanceled;
            }
        }

        if (dashAction != null)
        {
            if (subscribe) {
                dashAction.performed += OnDashPerformed;
                dashAction.canceled += ctx => DashPressed = false;
            }
            else dashAction.performed -= OnDashPerformed;
        }

        if (diveAction != null)
        {
            if (subscribe) diveAction.performed += OnDivePerformed;
            else diveAction.performed -= OnDivePerformed;
        }

        if (spinAction != null)
        {
            if (subscribe) spinAction.performed += OnSpinPerformed;
            else spinAction.performed -= OnSpinPerformed;
        }

        isHooked = subscribe;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        JumpPressed = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpReleased = true;
        JumpPressed = false; // Ensure we don't have both pressed and released as true
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        DashPressed = true;
    }

    private void OnDivePerformed(InputAction.CallbackContext ctx)
    {
        DivePressed = true;
    }

    private void OnSpinPerformed(InputAction.CallbackContext ctx)
    {
        SpinPressed = true;
    }

    // Used to reset the "pressed" and "released" states
    public void ClearFrameInput()
    {
        JumpPressed = false;
        JumpReleased = false;
        DashPressed = false;
        DivePressed = false;
        SpinPressed = false;
    }
}