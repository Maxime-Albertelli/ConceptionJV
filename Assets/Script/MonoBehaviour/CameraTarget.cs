using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform player;
    public GravityBody gravityBody;

    private void LateUpdate()
    {
        if (player == null || gravityBody == null) return;
        
        transform.position = player.position;
        Vector3 gravityUp = -gravityBody.GravityDirection;
        if (gravityUp != Vector3.zero) 
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        }
    }
}
