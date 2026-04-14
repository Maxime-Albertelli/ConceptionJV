using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Scriptable Objects/PlanetData")]
public class PlanetData : ScriptableObject
{
    [SerializeField]
    int PlanetSize;

    int GetPlanetSize()
    {
        return PlanetSize;
    }

    int GetShardQuantity() {
        return Mathf.FloorToInt(PlanetSize / 7);
    }

}
