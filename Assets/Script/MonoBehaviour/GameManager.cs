using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Paramètres de Spawn")]
    [SerializeField] private GameObject pf_Planet;
    [SerializeField] private float spawnDistance = 120f;
    [SerializeField] private float teleportHeightOffset = 20f;

    [Header("Références Joueur")]
    [SerializeField] private ShardCollection playerShardScript;
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("Références Indicateur")]
    [SerializeField] private IndicatorArrow arrowIndicator;

    [SerializeField] private TextMeshProUGUI shardText;

    public static GameManager Instance;
    private PlanetBehaviour currentPlanet;
    private PlanetBehaviour nextPlanet;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    void Start()
    {
        GameObject initialPlanet = Instantiate(pf_Planet);

        PlanetBehaviour behaviour = initialPlanet.GetComponent<PlanetBehaviour>();
        SetCurrentPlanet(behaviour);
    }

    public void SetCurrentPlanet(PlanetBehaviour planet)
    {
        currentPlanet = planet;
        Debug.Log("Nouvelle planète actuelle ! Éclats requis : 5");
    }

    public void CheckPlanetCompletion(int playerCurrentShards)
    {
        if (currentPlanet == null) return;
 
        shardText.text = "Shards : " + playerCurrentShards.ToString() + "/ 5";

        if (playerCurrentShards >= 5)
        {
            Vector3 spawnPos = currentPlanet.transform.position + Vector3.right * spawnDistance;
            GameObject nextPlanetObj = Instantiate(pf_Planet, spawnPos, Quaternion.identity);
            nextPlanet = nextPlanetObj.GetComponent<PlanetBehaviour>();

            currentPlanet.ActivateTeleporter();
            Transform tpTransform = currentPlanet.GetTeleporterTransform();
            if (tpTransform != null && arrowIndicator != null)
            {
                arrowIndicator.Show(tpTransform, currentPlanet.transform);
            }
            Debug.Log("Nouvelle planète créée au loin. Téléporteur prêt !");
        }
    }

    public void GoToNextPlanet()
    {
        if (nextPlanet == null) return;

        if (arrowIndicator != null)
        {
            arrowIndicator.Hide();
        }

        playerShardScript.ResetShards();

        // On interroge la planète cible pour connaître sa propre dimension
        float dynamicOffset = nextPlanet.GetDynamicSpawnHeight();

        // On positionne le joueur exactement sur la bordure de la zone de gravité
        playerRigidbody.transform.position = nextPlanet.transform.position + Vector3.up * dynamicOffset;
        playerRigidbody.linearVelocity = Vector3.zero;

        currentPlanet = nextPlanet;
        nextPlanet = null;
    }

    public void resetShardText(int shardValue)
    {
        shardText.text = "Shards : " + shardValue.ToString() + "/ 5";
    }

}
