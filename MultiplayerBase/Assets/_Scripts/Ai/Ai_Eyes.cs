using BlackboardSystem;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//[RequireComponent(typeof(Ai_Brain))]
public class Ai_Eyes : NetworkBehaviour, IExpert
{
    [SerializeField] public float radius;
    [SerializeField][Range(0f, 360f)] public float angle;
    [SerializeField] protected LayerMask canViewMask, obstructionMask;

    public List<IAiViewable> seenObjects = new List<IAiViewable>();

    [SerializeField] BlackboardController blackboardController;
    private Blackboard blackboard;

    private float fovCheckTimer = 0.2f;
    private float fovCurrentTime; 

    private void Awake()
    {
        blackboardController.RegisterExpert(this);
        blackboard = blackboardController.GetBlackboard();

        fovCurrentTime = fovCheckTimer;
    }

    private void Update()
    {
        //if(!IsHost) return;

        if (fovCurrentTime < 0)
        { 
            FieldOfViewCheck();
            fovCurrentTime = fovCheckTimer; 
        }
        else fovCurrentTime -= Time.deltaTime;
    }

    private void FieldOfViewCheck()
    {
        Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, radius, canViewMask);

        if (seenObjColliders.Length == 0)
        {
            seenObjects = new List<IAiViewable>();
            return;
        }

        List<IAiViewable> currentSeenObjects = new List<IAiViewable>();
        foreach (Collider seenCollider in seenObjColliders)
        {
            Transform target = seenCollider.transform;
            Vector3 dircToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, dircToTarget) < angle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dircToTarget, distToTarget, obstructionMask))
                {
                    IAiViewable seenObj = seenCollider.gameObject.GetComponent<IAiViewable>();
                    currentSeenObjects.Add(seenObj);
                }
            }
        }

        seenObjects = currentSeenObjects;
    }

    #region IExpert implimentation
    public void Execute(Blackboard blackboard)
    {
        foreach(IAiViewable seenObj in seenObjects)
        {
            if(seenObj == null) continue;
            seenObj.OnSeen(blackboard); 
        }
    }

    public int GetInsistence(Blackboard blackboard)
    {
        if(seenObjects.Count == 0) return 0;

        int highestImportance = 0;

        foreach(IAiViewable seenObj in seenObjects)
        {
            int importance = seenObj.GetImportance(transform.position); 

            if(highestImportance < importance)
            {
                highestImportance = importance;
            }
        }

        return highestImportance;
    }
    #endregion
}