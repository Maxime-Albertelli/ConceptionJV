using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pf_Planet;

    public static GameManager Instance;
    private PlanetBehaviour currentPlanet;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void SetCurrentPlanet(PlanetBehaviour planet)
    {
        currentPlanet = planet;
        Debug.Log("Nouvelle planète actuelle ! Éclats requis : " + currentPlanet.GetShardQte());
    }

    public void CheckPlanetCompletion(int playerCurrentShards)
    {
        if (currentPlanet == null) return;

        int requiredShards = currentPlanet.GetShardQte();

        if (playerCurrentShards >= requiredShards)
        {
            Debug.Log("Planète complétée ! Apparition du téléporteur...");
        }
    }

    void Start()
    {
        Instantiate(pf_Planet);
    }

    void InstantiateNewPlanet()
    {
        Instantiate(pf_Planet);
    }
}
