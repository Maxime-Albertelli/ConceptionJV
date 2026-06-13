using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "Scriptable Objects/PlanetData")]
public class PlanetData : ScriptableObject
{
    [SerializeField]
    int PlanetSize;

    public int GetShardQuantity() => 5;
}
