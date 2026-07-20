using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[Flags]
public enum BatteryType
{
    None = 0,
    Normal = 1 << 0,
    Large = 1 << 1
}

[Serializable]
public struct BatteryStruct
{
    [Header("UI")]
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;

    [Header("Settings")]
    [SerializeField] private BatteryType batteryType;
    [SerializeField] private int maxPower;
    [SerializeField] public int startingPower; 

    [HideInInspector] public int Power;
    [HideInInspector] public int Id; 

    public string Name => name;
    public Sprite Icon => icon;
    public BatteryType BatteryType => batteryType;
    public int MaxPower => maxPower;
    public int StartingPower => startingPower;
}
