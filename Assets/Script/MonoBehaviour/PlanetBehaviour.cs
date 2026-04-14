using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    [SerializeField] private PlanetData planetData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int GetShardQte()
    {
        return planetData.GetShardQuantity();
    }

    public void PlayerEnteredGravity()
    {
        // Cette méthode est appelée par l'enfant quand le joueur entre dans la zone
        GameManager.Instance.SetCurrentPlanet(this);
    }

    void DisplayTeleporter()
    {

    }
}
