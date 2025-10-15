using BlackboardSystem;
using UnityEngine;

public struct KeyAndValueStruct<Tvalue>
{
    public BlackboardKey key;
    public Tvalue value;

    public KeyAndValueStruct(BlackboardKey key, Tvalue value)
    {
        this.key = key;
        this.value = value;
    }
}
