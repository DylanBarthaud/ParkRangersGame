using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BlackboardSystem
{
    [Serializable]
    public struct BlackboardKey : IEquatable<BlackboardKey>, INetworkSerializable
    {
        public String name;
        public int hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => hashedKey == other.hashedKey;
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => hashedKey;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name); 
            serializer.SerializeValue(ref hashedKey);
        }

        public override string ToString() => name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey == rhs.hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => lhs.hashedKey != rhs.hashedKey;

    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode(); 
    }

    [Serializable]
    public class Blackboard
    {
        Dictionary<string, BlackboardKey> keys = new();
        Dictionary<BlackboardKey, object> entries = new();

        public List<Action> passedActions { get; } = new();

        public void AddAction(Action action)
        {
            if(action == null) return;
            passedActions.Add(action);
        }

        public void ClearActions() => passedActions.Clear();

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if(entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value) => entries[key] = new BlackboardEntry<T>(key, value);

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if(!keys.TryGetValue(keyName, out BlackboardKey key))
            {
                key = new BlackboardKey(keyName);
                keys[keyName] = key;
            }

            return key;
        }

        public bool ContainsKey(BlackboardKey key) => entries.ContainsKey(key);
        public void Remove(BlackboardKey key) => entries.Remove(key);
    }
}
