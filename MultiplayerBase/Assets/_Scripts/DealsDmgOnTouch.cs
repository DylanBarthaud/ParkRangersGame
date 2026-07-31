using System;
using System.Collections.Generic;
using UnityEngine;

public class DealsDmgOnTouch : MonoBehaviour
{
    [SerializeField] private string objName;
    [SerializeField] private int dmg; 
    [SerializeField] private bool repeats = false; 
    List<IHurtable> hurtables = new List<IHurtable>();

    private void Awake()
    {
        if(repeats) EventManager.instance.onTick_5 += OnTick;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (repeats) hurtables.Add(collision.gameObject.GetComponent<IHurtable>());
            else collision.gameObject.GetComponent<IHurtable>().IsHurt(objName, dmg); 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (repeats) hurtables.Remove(collision.gameObject.GetComponent<IHurtable>());
    }

    private void OnTick(int obj)
    {
        foreach(var hurtable in hurtables) hurtable.IsHurt(objName, dmg);
    }
}
