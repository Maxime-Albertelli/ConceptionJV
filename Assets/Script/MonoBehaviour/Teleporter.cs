using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // On vérifie que c'est le joueur et que le TP est bien activé
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GoToNextPlanet();
        }
    }
}
