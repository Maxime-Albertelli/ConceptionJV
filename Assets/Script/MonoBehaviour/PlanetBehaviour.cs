using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject myTeleporter;
    [SerializeField] private PlanetData planetData;

    private void Awake()
    {
        // On s'assure que le TP est désactivé au spawn de la planète
        if (myTeleporter != null) myTeleporter.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int GetShardQte()
    {
        return planetData.GetShardQuantity();
    }

    public Transform GetTeleporterTransform()
    {
        if (myTeleporter != null)
        {
            return myTeleporter.transform;
        }
        return null;
    }

    public void ActivateTeleporter()
    {
        if (myTeleporter != null) myTeleporter.SetActive(true);
    }

    public void PlayerEnteredGravity()
    {
        // Cette méthode est appelée par l'enfant quand le joueur entre dans la zone
        GameManager.Instance.SetCurrentPlanet(this);
    }

}
