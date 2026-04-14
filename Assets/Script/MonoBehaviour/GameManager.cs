using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject pf_Planet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(pf_Planet);
    }

    private void Update()
    {
        GetCurrentPlanetShardGoal();
    }

    void GetCurrentPlanetShardGoal()
    {

    }

    void InstantiateNewPlanet()
    {
        Instantiate(pf_Planet);
    }
}
