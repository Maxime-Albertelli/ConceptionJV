using UnityEditor;
using UnityEngine;
using static GroundProbe;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(GroundProbe))]
public class OptimizedPlayerMotor : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private MovementConfig config;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform modelRoot;

    private CharacterController controller;
    private PlayerInputReader input;
    private MovementState movementState;
    private GroundProbe groundProbe;

    // --- CORE ---
    private Vector3 currentVelocity;
    private bool isGrounded;
    private MovementState currentState = MovementState.Grounded;

    // --- TIMERS ---
    private float jumpBufferTimer;
    private float coyoteTimer;
    private float dashTimer;
    private float dashCooldownTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputReader>();
        groundProbe = GetComponent<GroundProbe>();
    }

    private void Update()
    {
        if (config == null)
        {
            Debug.LogWarning("Please assign a MovementConfig!");
            return;
        }

        float dt = Time.deltaTime;
        jumpBufferTimer -= dt;
        coyoteTimer -= dt;
        dashCooldownTimer -= dt;

        // Step 1: Check the environment (are we on the ground?)
        UpdateGrounding();
        CheckJump();
        CheckDash();

        // Evaluate state-specific behavior
        switch (currentState)
        {
            case MovementState.Grounded:
                CalculateGravity(dt);
                CalculateHorizontalMovement(dt);
                break;

            case MovementState.Airborne:
                CalculateGravity(dt);
                CalculateHorizontalMovement(dt);
                break;

            case MovementState.Dashing:
                dashTimer -= dt;
                break;
        }

        // Step 2.75: Rotate character to face movement direction
        RotateCharacter(dt);

        // Step 3: Move the character
        // We multiply velocity by Time.deltaTime to make movement frame-rate independent.
        controller.Move(currentVelocity * dt);
    }

    private void CheckJump()
    {
        if (input.JumpPressed) {
            jumpBufferTimer = config.jumpBufferTime;
        }

        if (jumpBufferTimer > 0f && coyoteTimer > 0f) {
            currentState = MovementState.Airborne;
            currentVelocity.y = Mathf.Sqrt(2f * config.jumpHeight * config.gravityDown);
            
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void CheckDash(){
        if (input.DashPressed && dashCooldownTimer <= 0f) {
            currentState = MovementState.Dashing;
            dashCooldownTimer = config.dashCooldown;
            dashTimer = config.dashDuration;
            currentVelocity = modelRoot.forward * config.dashSpeed;
            currentVelocity.y = 0.1f; // Small upward force
        }

        if (dashTimer <= 0f && currentState == MovementState.Dashing) {
            currentState = isGrounded ? MovementState.Grounded : MovementState.Airborne;
        }
    }

    private void RotateCharacter(float dt) {
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        
        // We check if we are actually moving fast enough, because Quaternion.LookRotation 
        // will print an error if we pass it a Vector3.zero (a direction of nowhere!).
        if (horizontalVelocity.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            modelRoot.rotation = Quaternion.RotateTowards(modelRoot.rotation, targetRotation, config.turnSharpness * dt * 50f);
        }
    } 

    private void CalculateHorizontalMovement(float dt)
    {
        // 1. Read Input
        Vector2 moveInput = input.Move;

        // 2. Calculate Camera-relative direction
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Combine inputs with camera vectors:
        // moveInput.y (up/down) acts along camera's forward.
        // moveInput.x (left/right) acts along camera's right.
        Vector3 moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // Clamp magnitude so diagonal movement isn't faster than moving straight, and multiply by max speed.
        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
        Vector3 targetVelocity = moveDirection * inputMagnitude * config.maxRunningSpeed;

        // 3. Select Acceleration or Deceleration
        // If the player is providing input, we accelerate. If they let go, we decelerate.
        bool isAccelerating = moveInput.sqrMagnitude > 0.01f;
        float speedRate;

        if (isGrounded) 
            speedRate = isAccelerating ? config.groundAcceleration : config.groundDeceleration;
        else 
            speedRate = isAccelerating ? config.airAcceleration : config.airDeceleration;

        // 4. Extract purely horizontal current velocity
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        // 5. Smoothly move our current horizontal speed towards our target
        Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetVelocity, speedRate * dt);

        // 6. Recombine
        currentVelocity = new Vector3(newHorizontal.x, currentVelocity.y, newHorizontal.z);
    }

    private void UpdateGrounding()
    {
        GroundHit hit = groundProbe.Probe(config.groundProbeRadius, config.groundProbeDistance, config.maxSlopeAngle);
        isGrounded = hit.isGrounded;
        if (isGrounded)
        {
            currentState = MovementState.Grounded;
            coyoteTimer = config.coyoteTime;
        } else {
            currentState = MovementState.Airborne;
        }
    }

    private void CalculateGravity(float dt)
    {
        if (isGrounded && currentVelocity.y <= 0f) {
            // If we're grounded, we want to "stick" to the ground by applying a small downward velocity.
            // Not zero because that can cause issues with slopes and uneven terrain.
            currentVelocity.y = -config.groundStickiness;
        }
        else
        {
            // If we're not grounded, we need to apply gravity over time.
            float finalGravityDown = config.gravityDown;
            if (currentVelocity.y < 0f) {
                // If we're already falling, apply fall multiplier for snappier falls.
                finalGravityDown *= config.fallMultiplier;
                
                // Fast Falling
                if (input.FastFallHeld) {
                    finalGravityDown *= config.fastFallMultiplier; 
                }
            } 
            else if (currentVelocity.y > 0f && !input.JumpHeld) {
                // If we're rising but the player has let go of the jump button, we want to cutoff the jump.
                finalGravityDown *= config.jumpCutoffMultiplier;
            }

            currentVelocity.y -= finalGravityDown * dt;
            // We also want to clamp our fall speed to prevent it from getting too high.
            if (currentVelocity.y < -config.maxFallSpeed)
            {
                currentVelocity.y = -config.maxFallSpeed;
            }
        }
    }
}