using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BlackboardSystem
{
    [CreateAssetMenu(fileName = "BlackboardData", menuName = "Scriptable Objects/BlackboardData")]
    public class BlackboardData : ScriptableObject
    {
        public List<BlackboardEntryData> entries = new(); 

        public void SetValuesOnBlackboard(Blackboard blackboard)
        {
            foreach (var entry in entries)
            {
                entry.SetValueOnBlackboard(blackboard);
            }
        }
    }

    [Serializable]
    public class BlackboardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.Type type;
        public AnyValue value;

        public void SetValueOnBlackboard(Blackboard blackboard)
        {
            var key = blackboard.GetOrRegisterKey(keyName);
            setValueDispatchTable[value.type](blackboard, key, value);
        }

        static Dictionary<AnyValue.Type, Action<Blackboard, BlackboardKey, AnyValue>> setValueDispatchTable = new()
        {
            { AnyValue.Type.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
            { AnyValue.Type.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue) },
            { AnyValue.Type.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
            { AnyValue.Type.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue) },
            { AnyValue.Type.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key, anyValue) },
        };

        public void OnAfterDeserialize() => value.type = this.type;
        public void OnBeforeSerialize() { }
    }

    [Serializable]
    public struct AnyValue
    {
        public enum Type { Int, Float, Bool, String, Vector3 }
        public Type type;

        public int intType;
        public float floatType;
        public bool boolType;
        public string stringType;
        public Vector3 vector3Type;

        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();

        T ConvertValue<T>()
        {
            return type switch
            {
                Type.Int => AsInt<T>(intType),
                Type.Float => AsFloat<T>(floatType),
                Type.Bool => AsBool<T>(boolType),
                Type.String => (T)(object)stringType,
                Type.Vector3 => (T)(object)vector3Type,
                _ => throw new NotSupportedException($"{typeof(T)} is not supported!")
            }; 
        }

        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
    }
}
