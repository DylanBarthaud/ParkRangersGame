using System;
using UnityEngine;

[Flags]
public enum BatteryType 
{
    None = 0,
    Normal = 1 << 0, 
    Large = 1 << 1
}

[CreateAssetMenu(fileName = "Battery", menuName = "Scriptable Objects/Battery")]
public class BatterySO : ScriptableObject
{
    [SerializeField] private int id; 
    [Header("UI")]
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;
    [Header("Settings")]
    [SerializeField] BatteryType batteryType;
    [SerializeField] private int power;

    public int Id => id;
    public string Name => name;
    public Sprite Icon => icon;
    public BatteryType BatteryType => batteryType;
    public int Power => power;
}
