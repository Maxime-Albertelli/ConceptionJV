using TMPro;
using UnityEngine;

public class ShardCollection : MonoBehaviour
{
    private int shardValue = 0;
    [SerializeField] private TextMeshProUGUI shardText;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Shard"))
        {
            other.enabled = false;
            shardValue++;
            Debug.Log("Shard collected");
            Destroy(other.gameObject);
            shardText.text = "Shards : " + shardValue.ToString();
            GameManager.Instance.CheckPlanetCompletion(shardValue);
        }
        
    }
}
