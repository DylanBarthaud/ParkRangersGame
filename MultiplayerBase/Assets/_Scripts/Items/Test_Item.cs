using UnityEngine;

public class Test_Item : Item
{
    public override void UseItem(GameObject user)
    {
        Debug.Log("USED ITEM:" + name);
        GetComponent<MultiplayerAudioHandlerWrapper>().PlaySoundServerRpc("Bob", default, 20, 400); 
    }
}
