using BlackboardSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Ai_Senses : NetworkBehaviour, IExpert
{
    [Header("General")]
    [SerializeField] private float minDistToSenseTarget = 5f;
    [SerializeField] public float maxDistToSenseTarget = 50f;

    [Header("Vision")]
    [SerializeField] public float radius;
    [SerializeField][Range(0f, 360f)] public float angle;
    [SerializeField] protected LayerMask canViewMask, obstructionMask;

    public HashSet<IAiSensible> sensedObjects = new HashSet<IAiSensible>();

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
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, maxDistToSenseTarget, canViewMask);

        HashSet<IAiSensible> currentSensedObjects = FieldOfViewCheck(objectsInRange);
        currentSensedObjects.UnionWith(NoiseDetectionCheck(objectsInRange));

        //Check to see if objects are no longer seen 
        foreach (IAiSensible seenObj in sensedObjects)
        {
            if (seenObj == null) continue;

            if (!currentSensedObjects.Contains(seenObj))
            {
                seenObj.OnUnSeen(blackboard, this);
            }
        }

        sensedObjects = currentSensedObjects; 
    }

    private HashSet<IAiSensible> FieldOfViewCheck(Collider[] sensedObjColliders)
    {
        HashSet<IAiSensible> currentSeenObjects = new HashSet<IAiSensible>();
        foreach (Collider seenCollider in sensedObjColliders)
        {
            Transform target = seenCollider.transform;
            Vector3 dircToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, dircToTarget) < angle / 2
                || Vector3.Distance(transform.position, target.position) > minDistToSenseTarget)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dircToTarget, distToTarget, obstructionMask))
                {
                    IAiSensible seenObj = seenCollider.gameObject.GetComponent<IAiSensible>();
                    currentSeenObjects.Add(seenObj);
                }
            }
        }

        return currentSeenObjects;
    }

    private HashSet<IAiSensible> NoiseDetectionCheck(Collider[] sensedObjColliders)
    {
        HashSet<IAiSensible> currentHeardObjects = new HashSet<IAiSensible>();
        
        foreach(Collider sensedCollider in sensedObjColliders)
        {
            IAiSensible sensedObj = sensedCollider.gameObject.GetComponent<IAiSensible>();
            if(sensedObj != null)
            {
                float audioHeardDataSquared = sensedObj.GetAudioDataSquared();
                float distanceToObj = Vector3.Distance(transform.position, sensedCollider.gameObject.transform.position);

                if (distanceToObj * distanceToObj <= audioHeardDataSquared)
                {
                    Debug.Log("NoiseDetected");
                    Debug.Log(sensedCollider.name); 
                    currentHeardObjects.Add(sensedObj);
                }
            }
        }

        return currentHeardObjects;
    }

    #region IExpert implimentation
    public void Execute(Blackboard blackboard)
    {
        foreach(IAiSensible seenObj in sensedObjects)
        {
            if(seenObj != null)
            {
                seenObj.OnSeen(blackboard, this);
            }
        }
    }

    public int GetInsistence(Blackboard blackboard)
    {
        if(sensedObjects.Count == 0) return 0;

        int highestImportance = 0;

        foreach(IAiSensible seenObj in sensedObjects)
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