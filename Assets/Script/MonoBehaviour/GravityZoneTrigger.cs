using UnityEngine;

public class GravityZoneTrigger : MonoBehaviour
{
    // Référence vers le script principal de la planète
    [SerializeField] private PlanetBehaviour myPlanet;

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie que c'est bien le joueur
        if (other.CompareTag("Player"))
        {
            // On prévient le parent !
            myPlanet.PlayerEnteredGravity();
        }
    }
}
