using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BatteryPool : IPool<Battery>
{
    [SerializeField] private GameObject batteryPrefab; 
    public GameObject BatteryPrefab => batteryPrefab;
    private List<Battery> list = new();
    
    public Battery GetBattery(int id)
    {
        foreach (var battery in list) if(battery.Id == id) return battery;

        Debug.Log($"No Battery with id: '{id}' found in list"); 
        return null;
    }

    public void AddObjToPool(Battery battery) => list.Add(battery);
}
