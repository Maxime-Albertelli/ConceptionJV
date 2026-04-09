using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// J'ai retiré le [RequireComponent(typeof(Rigidbody))] pour te laisser 
// la liberté de l'utiliser sur des objets sans physique si besoin.
public class GravityBody : MonoBehaviour
{
    [Header("Comportement")]
    [SerializeField] private bool _applyGravityPull = true; // Appliquer la force de chute ?
    [SerializeField] private bool _alignWithGravity = false; // Aligner l'objet avec la planète ?

    [Header("Paramètres")]
    [SerializeField] private float GRAVITY_FORCE = 800;
    [SerializeField] private float _rotationSpeed = 10f; // Vitesse de redressement

    public Vector3 GravityDirection
    {
        get
        {
            if (_gravityAreas.Count == 0) return Vector3.zero;
            _gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            return _gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }

    private Rigidbody _rigidbody;
    private List<GravityArea> _gravityAreas;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _gravityAreas = new List<GravityArea>();
    }

    void FixedUpdate()
    {
        Vector3 gravityDir = GravityDirection;
        if (gravityDir == Vector3.zero) return;

        // 1. ATTRACTION PHYSIQUE (La chute)
        if (_applyGravityPull && _rigidbody != null && !_rigidbody.isKinematic)
        {
            _rigidbody.AddForce(gravityDir * (GRAVITY_FORCE * Time.fixedDeltaTime), ForceMode.Acceleration);
        }

        // 2. ORIENTATION VERS LE CENTRE DE GRAVITÉ (Pour les décors)
        if (_alignWithGravity)
        {
            Vector3 targetUp = -gravityDir;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;

            // S'il y a un Rigidbody physique, on utilise MoveRotation
            if (_rigidbody != null && !_rigidbody.isKinematic)
            {
                _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));
            }
            else
            {
                // Sinon (objet statique/kinematic), on tourne directement le Transform
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void AddGravityArea(GravityArea gravityArea)
    {
        if (!_gravityAreas.Contains(gravityArea)) _gravityAreas.Add(gravityArea);
    }

    public void RemoveGravityArea(GravityArea gravityArea)
    {
        _gravityAreas.Remove(gravityArea);
    }
}