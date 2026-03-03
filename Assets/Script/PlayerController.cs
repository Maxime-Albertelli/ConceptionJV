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
        _rigidbody = transform.GetComponent<Rigidbody>();
        _gravityBody = transform.GetComponent<GravityBody>();

        if (_cam == null) _cam = Camera.main.transform;
    }

    void Update()
    {
        // On gère uniquement la détection du sol et les animations ici
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);

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
        Debug.Log("Jump !");
        // Le saut est déclenché par l'event, plus besoin de Input.GetKeyDown
        if (_isGrounded)
        {
            Debug.Log("True jump");
            // Le saut se fait toujours à l'opposé de la gravité planétaire
            _rigidbody.AddForce(-_gravityBody.GravityDirection * _jumpForce, ForceMode.Impulse);
        }
    }

    // -------------------------------

    void FixedUpdate()
    {
        bool isRunning = _moveInput.magnitude > 0.1f;

        if (isRunning)
        {
            // 1. Projection de la caméra sur la surface de la planète (Mario Galaxy style)
            Vector3 camForwardOnSurface = Vector3.ProjectOnPlane(_cam.forward, transform.up).normalized;
            Vector3 camRightOnSurface = Vector3.ProjectOnPlane(_cam.right, transform.up).normalized;

            // 2. Calcul du vecteur de déplacement
            // Note : on utilise _moveInput.y pour l'avant/arrière et _moveInput.x pour la gauche/droite
            Vector3 moveDir = (camForwardOnSurface * _moveInput.y + camRightOnSurface * _moveInput.x).normalized;

            // 3. Déplacement (MovePosition pour le feeling "vif" de Haste)
            _rigidbody.MovePosition(_rigidbody.position + moveDir * (_speed * Time.fixedDeltaTime));

            // 4. Rotation vers la direction du mouvement en respectant l'inclinaison (transform.up)
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, transform.up);
            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);

            _rigidbody.MoveRotation(newRotation);
        }

        if (_animator != null)
        {
            _animator.SetBool("isRunning", isRunning);
        }
    }
}