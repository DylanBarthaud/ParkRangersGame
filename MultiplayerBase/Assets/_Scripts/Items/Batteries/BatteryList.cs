using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battery", menuName = "Scriptable Objects/Battery")]
public class BatteryList : ScriptableObject
{
    [SerializeField] private BatteryStruct[] batteries;

    public BatteryStruct GetBattery(string batteryName)
    {
        foreach (BatteryStruct battery in batteries)
        {
            if(battery.Name == batteryName) return battery;
        }

        return new BatteryStruct(); 
    }
}
