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
            Destroy(other.gameObject);
            shardText.text = "Shards : " + shardValue.ToString();
            GameManager.Instance.CheckPlanetCompletion(shardValue);
        }
        
    }

    public void ResetShards()
    {
        shardValue = 0;
        shardText.text = "Shards : " + shardValue.ToString();
    }
}
