using UnityEngine;

public class ShardCollection : MonoBehaviour
{
    private int shardValue = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Shard")
        {
            shardValue++;
            Debug.Log("Shard collected");
            Destroy(other.gameObject);
        }
        
    }
}
