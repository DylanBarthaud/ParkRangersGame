using UnityEngine;

public class Test_Item : Item
{
    public override void UseItem()
    {
        Debug.Log("USED ITEM:" + name); 
    }
}
