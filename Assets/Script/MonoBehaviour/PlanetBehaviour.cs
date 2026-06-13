using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    // N'oublie pas de glisser l'objet enfant contenant Planet.cs ici dans l'inspecteur
    [SerializeField] private Planet planetScript;

    // Appelée par le GameManager quand les shards sont collectés
    public void ActivateTeleporter()
    {
        if (planetScript != null)
        {
            planetScript.SetTeleporterActive(true);
        }
    }

    // Appelée par le GameManager pour pointer la flèche vers le bon endroit
    public Transform GetTeleporterTransform()
    {
        if (planetScript != null)
        {
            return planetScript.GetTeleporterTransform();
        }
        return null;
    }
    public void PlayerEnteredGravity()
    {
        // Cette méthode est appelée par l'enfant quand le joueur entre dans la zone
        GameManager.Instance.SetCurrentPlanet(this);
    }

    public float GetDynamicSpawnHeight()
    {
        // On récupère le trigger enfant
        GravityZoneTrigger gravityTrigger = GetComponentInChildren<GravityZoneTrigger>();

        if (gravityTrigger != null)
        {
            SphereCollider sphereCol = gravityTrigger.GetComponent<SphereCollider>();
            if (sphereCol != null)
            {
                // On multiplie le rayon par le scale global pour avoir la vraie distance dans le monde
                return sphereCol.radius * gravityTrigger.transform.lossyScale.x;
            }
        }

        // Valeur de secours au cas où le collider serait introuvable
        return 20f;
    }
}