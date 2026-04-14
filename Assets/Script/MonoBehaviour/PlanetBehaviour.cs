using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    [SerializeField] private PlanetData planetData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int GetShardQte()
    {
        return planetData.GetShardQuantity();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si l'objet qui entre dans la zone de gravité est le joueur
        if (other.CompareTag("Player"))
        {
            // On dit au GameManager que cette planète est la planète actuelle
            GameManager.Instance.SetCurrentPlanet(this);
        }
    }
}
