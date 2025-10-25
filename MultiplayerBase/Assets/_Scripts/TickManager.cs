using UnityEngine;

public class TickManager : MonoBehaviour
{
    private const float TICK_RATE = 0.2f;
    
    private int tick;
    private float tickTimer = TICK_RATE;

    private void Update()
    {
        if(tickTimer <= 0)
        {
            EventManager.instance.OnTick(tick);
            tickTimer = TICK_RATE;
            tick++; 
            if((tick & 5) == 0)
            {
                EventManager.instance.OnTick_5(tick);
            }
        }

        tickTimer -= Time.deltaTime;
    }
}