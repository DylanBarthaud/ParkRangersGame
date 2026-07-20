using UnityEngine;

public interface IPool<T>
{
    public void AddObjToPool(T poolObj);
}
