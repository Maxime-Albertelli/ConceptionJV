using TMPro;
using UnityEngine;

public class ShardCollection : MonoBehaviour
{
    private int shardValue = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Shard"))
        {
            other.enabled = false;
            shardValue++;
            Destroy(other.gameObject);
            GameManager.Instance.CheckPlanetCompletion(shardValue);
        }
        
    }

    public void ResetShards()
    {
        shardValue = 0;
        GameManager.Instance.resetShardText(shardValue);
    }
}
