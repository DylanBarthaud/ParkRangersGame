using UnityEngine;

public class Flare : Item
{
    [Header("Flare Settings")]
    [SerializeField] private GameObject particles; 

    public override void UseItem(GameObject user)
    {
        particles.SetActive(!particles.activeInHierarchy);
    }
}
