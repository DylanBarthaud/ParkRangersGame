using BlackboardSystem;
using UnityEngine;

public class FlareLight : MonoBehaviour, IAiSensible
{
    public float GetAudioDataSquared() => 100;

    public int GetImportance(Ai_Senses caller)
    {
        if (caller == null || this == null || gameObject == null) return 0;

        Vector3 aiPos = caller.transform.position;
        return GetImportance(aiPos);
    }

    public void OnSeen(Blackboard blackboard, Ai_Senses caller) => EventManager.instance.OnDistractionUpdate(transform.position);
    public void OnUnSeen(Blackboard blackboard, Ai_Senses caller) => EventManager.instance.OnDistractionUpdate(Vector3.zero);

    private int GetImportance(Vector3 aiPos)
    {
        float distanceToAi = Vector3.Distance(transform.position, aiPos);

        int importance = 200 - (int)distanceToAi;

        return importance;
    }
}
