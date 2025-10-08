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
            { AnyValue.Type.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
        };

        public void OnAfterDeserialize() => value.type = this.type;
        public void OnBeforeSerialize() { }
    }

    [Serializable]
    public struct AnyValue
    {
        public enum Type { Int, Float, Bool, String, Vector3 }
        public Type type;

        public bool boolType;

        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();

        T ConvertValue<T>()
        {
            return type switch
            {
                Type.Int => throw new NotImplementedException(),
                Type.Float => throw new NotImplementedException(),
                Type.Bool => AsBool<T>(boolType),
                Type.String => throw new NotImplementedException(),
                Type.Vector3 => throw new NotImplementedException(),
                _ => throw new NotSupportedException($"{typeof(T)} is not supported!")
            }; 
        }

        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
    }
}
