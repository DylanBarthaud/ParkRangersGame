using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private void Update()
    {
        if(!IsOwner) return;

        Vector3 moveDir = Vector3.zero;

        if(Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if(Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if(Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if(Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float ms = 3f;
        transform.position += moveDir * ms * Time.deltaTime; 
    }
}
