using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Indispensable pour l'Input System

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _cam;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem dustTrail;
    

    private float _groundCheckRadius = 0.3f;
    [SerializeField] private float _speed = 10;
    private float _turnSpeed = 1500f;
    [SerializeField] private float _jumpForce = 100f;

    private Rigidbody _rigidbody;
    private GravityBody _gravityBody;

    // Remplacement de _direction par un Vector2 pour correspondre au nouveau système
    private Vector2 _moveInput;
    private bool _isGrounded; // Variable globale pour la partager avec OnJump()

    void Start()
    {
        var emission = dustTrail.emission;
        emission.enabled = false;
        _rigidbody = transform.GetComponent<Rigidbody>();
        _gravityBody = transform.GetComponent<GravityBody>();

        if (_cam == null) _cam = Camera.main.transform;
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }

    void Update()
    {
        // On gère uniquement la détection du sol et les animations ici
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask, QueryTriggerInteraction.Ignore);

        if (_animator != null)
        {
            _animator.SetBool("isJumping", !_isGrounded);
        }
    }

    // --- NOUVEAU SYSTÈME D'INPUT ---

    void OnMove(InputValue value)
    {
        // Récupère l'input du stick ou de ZQSD
        _moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        
        // Le saut est déclenché par l'event, plus besoin de Input.GetKeyDown
        if (_isGrounded)
        {
            // Le saut se fait toujours à l'opposé de la gravité planétaire
            _rigidbody.AddForce(-_gravityBody.GravityDirection * _jumpForce, ForceMode.Impulse);
        }
    }

    // -------------------------------

    void FixedUpdate()
    {
        bool shouldEmit = _moveInput.magnitude > 0.1f && _isGrounded;

        // 2. On récupère le module et on applique l'état (true ou false) d'un coup
        var emission = dustTrail.emission;
        emission.enabled = shouldEmit;

        // 1. On récupère la vraie direction "Haut"
        Vector3 gravityUp = -_gravityBody.GravityDirection;
        if (gravityUp == Vector3.zero) gravityUp = transform.up;

        bool isRunning = _moveInput.magnitude > 0.1f;

        if (isRunning)
        {
            // 2. Projection de la caméra plus ROBUSTE
            Vector3 camForwardOnSurface = Vector3.ProjectOnPlane(_cam.forward, gravityUp);

            // SÉCURITÉ : Si la caméra regarde pile vers le sol (ou le ciel), la projection s'annule.
            // On utilise alors le "Haut" de la caméra comme "Avant" pour le joueur.
            if (camForwardOnSurface.sqrMagnitude < 0.01f)
            {
                camForwardOnSurface = Vector3.ProjectOnPlane(_cam.up, gravityUp);
            }
            camForwardOnSurface.Normalize();

            // 3. Calcul de la "Droite" via un Produit Vectoriel (Cross Product)
            // Cela garantit un angle droit parfait, sans aucun tremblement
            Vector3 camRightOnSurface = Vector3.Cross(gravityUp, camForwardOnSurface).normalized;

            // 4. Calcul du déplacement
            Vector3 moveDir = (camForwardOnSurface * _moveInput.y + camRightOnSurface * _moveInput.x).normalized;

            // Déplacement
            _rigidbody.MovePosition(_rigidbody.position + moveDir * (_speed * Time.fixedDeltaTime));

            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, gravityUp);
            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(newRotation);
        }
        // ... (garde ton 'else' pour la rotation au repos)
        else
        {
            // 4. ROTATION AU REPOS : Si on s'arrête, on continue d'aligner le joueur à la courbure de la planète
            Quaternion targetUpRotation = Quaternion.FromToRotation(transform.up, gravityUp) * _rigidbody.rotation;
            Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, targetUpRotation, Time.fixedDeltaTime * 5f); // 5f pour la vitesse de redressement

            _rigidbody.MoveRotation(newRotation);
        }

        if (_animator != null)
        {
            _animator.SetBool("isRunning", isRunning);
        }
    }
}