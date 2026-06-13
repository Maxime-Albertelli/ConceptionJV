using UnityEngine;

[CreateAssetMenu(menuName = "Player/MovementConfig", fileName = "MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Horizontal Movement")]
    public float maxRunningSpeed = 5f;
    public float groundAcceleration = 10f;
    public float groundDeceleration = 10f;
    public float airAcceleration = 5f;
    public float airDeceleration = 5f;
    public float turnSharpness = 0.5f;
    public float overspeedFriction = 0.5f;
    public float absoluteMaxSpeed = 10f;

    [Header("Grounding")]
    public float groundProbeDistance = 0.1f;
    public float groundProbeRadius = 0.5f;
    [Range(0f, 1f)] public float maxSlopeAngle = 0.5f;
    public float groundStickiness = 5f;

    [Header("Jump Stack")]
    public float jumpHeight = 5f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    [Range(0f, 1f)] public float jumpCutoffMultiplier = 5f;
    public float gravityDown = 9.81f;
    public float fallMultiplier = 2.5f;
    public float maxFallSpeed = 20f;
    public float apexHangMultiplier = 0.5f;
    public float apexThreshold = 0.1f;
    public float fastFallMultiplier = 3f;

    [Header("Backflip / Spin Suspension")]
    public float spinSuspendDuration = 0.22f;
    public float spinHopForce = 3.4f;
    public float spinAirControlBoost = 2.2f;
    public float backflipHeight = 4.8f;
    public float backflipForwardSpeed = 6f;
    [Range(-1f, 1f)] public float backflipInputThreshold = -0.65f;

    [Header("Dash")]
    public float dashSpeed = 26f;
    public float dashDuration = 0.16f;
    public float dashCooldown = 0.28f;

    [Header("Dive / Slide / Roll")]
    public float diveForwardSpeed = 22f;
    public float diveDownSpeed = 8f;
    public float diveSteerStrength = 0.12f;
    public float slideFriction = 18f;
    public float rollPerfectWindow = 0.11f;
    public float rollPerfectBoost = 1.35f;
    [Range(0f, 1f)] public float popUpMomentumKeep = 0.65f;
    public float rollJumpImpulse = 4.6f;
    public float slideColliderHeight = 1f;

    [Header("Walls")]
    public float wallCheckDistance = 0.7f;
    public float wallJumpUpForce = 10f;
    public float wallJumpOutForce = 9f;
    public float wallJumpCooldown = 0.2f;
    public float wallRunGravityMultiplier = 0.35f;
    public float wallRunMinSpeed = 10f;
    public float wallRunDuration = 1.2f;

}
