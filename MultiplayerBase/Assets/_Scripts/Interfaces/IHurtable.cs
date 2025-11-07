using UnityEngine;

public interface IHurtable
{
    public void IsHurt(string caller, int amount);
    public void IsKilled(); 
}
