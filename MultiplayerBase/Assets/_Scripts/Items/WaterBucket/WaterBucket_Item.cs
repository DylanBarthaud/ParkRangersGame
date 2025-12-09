using UnityEngine;

public class WaterBucket_Item : Item
{
    [SerializeField] private GameObject waterEffect; 

    public override void UseItem(GameObject user)
    {
        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 spawnPos = cam.position + forward * 3f;

        Instantiate(waterEffect, spawnPos, Quaternion.identity);

        uses--;
    }
}
