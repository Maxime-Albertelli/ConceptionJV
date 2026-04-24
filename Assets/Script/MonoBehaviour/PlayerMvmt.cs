using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMvmt : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform cameraTransform;
    private Rigidbody rb;
    private Vector2 moveInput;
    [SerializeField] private float turnSpeed = 5;
    [SerializeField] private float moveSpeed = 6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        MovePlayer();
    }

    void OnJump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0),ForceMode.Impulse);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        // Sécurité : si la caméra n'est pas assignée, on prend la caméra principale
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // 1. On récupère les vecteurs "Avant" et "Droite" de la caméra
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // 2. On "aplatit" ces vecteurs sur l'axe Y pour éviter que le joueur 
        // n'essaie de s'enfoncer dans le sol ou de s'envoler si la caméra regarde en bas/haut
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 3. On calcule la vraie direction du mouvement
        // (Avant de la caméra * input Vertical) + (Droite de la caméra * input Horizontal)
        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // Si on détecte une entrée de la part du joueur
        if (moveDir.magnitude > 0.1f)
        {
            // --- ORIENTATION (Le joueur regarde où il va) ---
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeed * Time.fixedDeltaTime // fixedDeltaTime si appelé dans FixedUpdate()
            );

            // --- DÉPLACEMENT (Vif et net) ---
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
        }
        else
        {
            // Arrêt immédiat dès qu'on lâche les touches
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
}
