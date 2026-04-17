using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Paramètres de Spawn")]
    [SerializeField] private GameObject pf_Planet;
    [SerializeField] private float spawnDistance = 60f;
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
        Instantiate(pf_Planet);
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
        shardText.text = "Shards : " + playerCurrentShards.ToString() + "/" + requiredShards.ToString();

        if (playerCurrentShards >= requiredShards)
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

        playerRigidbody.transform.position = nextPlanet.transform.position + Vector3.up * teleportHeightOffset;
        playerRigidbody.linearVelocity = Vector3.zero; // Stop l'inertie pour éviter les bugs

        //Destroy(currentPlanet.gameObject, 2f);

        currentPlanet = nextPlanet;
        nextPlanet = null;
    }

    public void resetShardText(int shardValue)
    {
        int requiredShards = currentPlanet.GetShardQte();
        shardText.text = "Shards : " + shardValue.ToString() + "/" + requiredShards.ToString();
    }

}
