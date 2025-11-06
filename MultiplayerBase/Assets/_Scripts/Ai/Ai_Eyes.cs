using BlackboardSystem;
using System;
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

    private void Awake()
    {
        blackboardController.RegisterExpert(this);
        blackboard = blackboardController.GetBlackboard();

        EventManager.instance.onTick += OnTick;
    }

    private void OnTick(int tick)
    {
        if (!IsHost) return;
        FieldOfViewCheck();
    }

    private void FieldOfViewCheck()
    {
        Collider[] seenObjColliders = Physics.OverlapSphere(transform.position, radius, canViewMask);

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

        //Check to see if objects are no longer seen 
        foreach(IAiViewable seenObj in seenObjects)
        {
            if(seenObj == null) continue;
            bool isSeen = false;    

            foreach(IAiViewable currentSeenObj in currentSeenObjects)
            {
                if(currentSeenObj ==  seenObj)
                {
                    isSeen = true;
                }
            }

            if (!isSeen)
            {
                seenObj.OnUnSeen(blackboard, this);
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
            seenObj.OnSeen(blackboard, this); 
        }
    }

    public int GetInsistence(Blackboard blackboard)
    {
        if(seenObjects.Count == 0) return 0;

        int highestImportance = 0;

        foreach(IAiViewable seenObj in seenObjects)
        {
            if(seenObj == null) continue;
            int importance = seenObj.GetImportance(this); 

            if(highestImportance < importance)
            {
                highestImportance = importance;
            }
        }

        return highestImportance;
    }
    #endregion
}