using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Scriptable Objects/PlanetData")]
public class PlanetData : ScriptableObject
{
    [SerializeField]
    int PlanetSize;

    public int GetPlanetSize()
    {
        return PlanetSize;
    }

    public int GetShardQuantity() {
        return Mathf.FloorToInt(PlanetSize / 7);
    }

}
