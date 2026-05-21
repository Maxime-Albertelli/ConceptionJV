using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundProbe : MonoBehaviour
{
    [System.Serializable]
    public struct GroundHit
    {
        public bool isGrounded;
        public Vector3 normal;
        public float slopeAngle;
        public float distance;
        public Vector3 contactPoint;
        public Collider collider;
    }

    [SerializeField] private CharacterController controller;
    // Exists to detect only ground items (~0 means all layers)
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float probeStartOffset = 0.08f;

    private GroundHit lastHit;
    private Vector3 lastOrigin;
    private float lastRadius;
    private float lastDistance;
    private bool didHitLastProbe;

    public GroundHit LastHit => lastHit;

    void Start()
    {
        Probe(0.1f, 0.4f, 45f);
    }

    void Update()
    {
        // For testing, probe every frame with fixed parameters
        Probe(0.1f, 0.4f, 45f);
    }

    private void Reset()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("GroundProbe requires a CharacterController component.");
        }
    }

    public GroundHit Probe(float probeRadius, float probeDistance, float maxSlopeAngle)
    {
        if (controller == null)
        {
            Reset();
        }

        Vector3 center = transform.position + controller.center;
        Vector3 bottom = center + Vector3.down * (controller.height / 2 - controller.radius);
        Vector3 origin = bottom + Vector3.up * probeStartOffset;
        float castDistance = probeStartOffset + probeDistance;

        lastOrigin = origin;
        lastRadius = probeRadius;
        lastDistance = probeDistance;

        GroundHit result = new()
        {
            isGrounded = false,
            normal = Vector3.up,
            slopeAngle = 0f,
            distance = castDistance,
            contactPoint = transform.position,
            collider = null
        };

        if (Physics.SphereCast(origin, probeRadius, Vector3.down, out RaycastHit hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            result.isGrounded = slope <= maxSlopeAngle;
            result.normal = hit.normal;
            result.slopeAngle = slope;
            result.distance = hit.distance;
            result.contactPoint = hit.point;
            result.collider = hit.collider;
            didHitLastProbe = true;
        } else
        {
            didHitLastProbe = false;
        }
        lastHit = result;
        return result;
    }

    // Visualize the last probe in the editor for debugging
    private void OnDrawGizmos()
    {
        // if (lastDistance <= 0f) return;

        Gizmos.color = didHitLastProbe ? Color.green : Color.red;
        Gizmos.DrawWireSphere(lastOrigin, lastRadius);
        Gizmos.DrawWireSphere(lastOrigin + Vector3.down * lastDistance, lastRadius);
        Gizmos.DrawLine(lastOrigin, lastOrigin + Vector3.down * lastDistance);
    }
    }