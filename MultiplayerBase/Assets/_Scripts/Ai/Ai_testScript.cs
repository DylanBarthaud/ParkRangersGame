using BehaviourTrees;
using BlackboardSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Ai_testScript : NetworkBehaviour, IExpert
{
    private NavMeshAgent agent;

    Root root;

    [SerializeField] BlackboardController blackboardController;
    [SerializeField] private float baseSpeed, investigateHintSpeed, chaseSpeed;
    [Header("Stalk Settings")]
    [SerializeField] private float stalkSpeed;
    [SerializeField] private float stalkTime; 
    [SerializeField] private float minStalkDist;
    [SerializeField] private float maxStalkDist;
    Blackboard blackboard;
    BlackboardKey AiMonsterKey;
    AiInfo aiInfo;

    bool canParolBool;

    private void Awake()
    {
        #region Set Up
        EventManager.instance.onTick_5 += OnTick_5;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;

        aiInfo = new AiInfo()
        {
            position = transform.position,
        };

        blackboard = blackboardController.GetBlackboard();
        blackboardController.RegisterExpert(this);
        AiMonsterKey = blackboard.GetOrRegisterKey("AiMonsterKey");
        blackboard.SetValue(AiMonsterKey, aiInfo);

        #endregion

        #region Behaviour Tree

        root = new Root("Root");

        PrioritySelector prioritySelector = new PrioritySelector("PrioritySelector");

        #region ChasePlayer Sequence
        Sequence stalkPlayerSequence = new Sequence("StalkPlayerSequence", 100);

        PlayerInfo PlayerInfo() 
        {
            List<PlayerInfo> seenPlayers = new List<PlayerInfo>(); 
            foreach(BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo info))
                {
                    if (info.canSeePlayer) seenPlayers.Add(info);
                }
            }

            if (seenPlayers.Count == 0)
            {
                PlayerInfo placeHolderInfo = new PlayerInfo()
                {
                    canSeePlayer = false,
                    importance = 0,
                    position = Vector3.zero 
                };

                return placeHolderInfo;
            }

            PlayerInfo playerInfo = new PlayerInfo(); 
            foreach(PlayerInfo player in seenPlayers)
            {
                if(player.importance > playerInfo.importance)
                {
                    playerInfo = player;
                }
            }

            return playerInfo;
        }
        Leaf stalkPlayer = new Leaf("StalkPlayer", new StalkPlayerStrategy(PlayerInfo, agent, stalkSpeed, stalkTime, minStalkDist, maxStalkDist)); 
        Leaf chasePlayer = new Leaf("ChasePlayer", new ChasePlayerStrategy(PlayerInfo, agent, chaseSpeed), 100);

        stalkPlayerSequence.AddChild(stalkPlayer);
        stalkPlayerSequence.AddChild(chasePlayer);
        #endregion

        #region Investigate Hint
        GridPosition InvestigateHint()
        {
            BlackboardKey overlordKey = blackboard.GetOrRegisterKey("AiOverlordKey");

            if (blackboard.TryGetValue(overlordKey, out OverlordGivenInfo overlordInfo))
            {
                return overlordInfo.playerGridPosition;
            }

            Debug.LogError("OverlordInfo NULL setting grid position to 0,0"); 
            return new GridPosition { x = 0, z = 0 };
        }
        Leaf moveToCell = new Leaf("MoveToPos", new MoveToPositionInCellStrategy(InvestigateHint, agent, investigateHintSpeed), 50);
        #endregion

        #region Search Cell
        bool InSameCellAsPlayer()
        {
            bool inSameCellAsPlayer = false;

            MapHandler mapHandler = GameManager.instance.mapHandler;

            GridPosition aiPosition = new GridPosition { x = 0, z = 0 };
            if (blackboard.TryGetValue(AiMonsterKey, out AiInfo monsterInfo))
            {
                aiPosition = mapHandler.GetGridLocation(monsterInfo.position);
            }

            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    GridPosition playerPosition = mapHandler.GetGridLocation(playerInfo.position);
                    float distance = mapHandler.GetDistanceBetweenGrids(aiPosition, playerPosition);

                    if (distance <= 0)
                    {
                        inSameCellAsPlayer = true;
                        break;
                    }
                }
            }

            return inSameCellAsPlayer;
        }
        IfGate inSameCellAsPlayer = new IfGate("InSameCellAsPlayer", new Condition(InSameCellAsPlayer), 90); 
        Leaf searchCell = new Leaf("SearchCell", new SearchCellStrategy(() => GameManager.instance.mapHandler.GetGridLocation(transform.position), agent), 50);

        inSameCellAsPlayer.AddChild(searchCell);
        #endregion

        prioritySelector.AddChild(stalkPlayerSequence);
        prioritySelector.AddChild(inSameCellAsPlayer);
        prioritySelector.AddChild(moveToCell);

        root.AddChild(prioritySelector);

        #endregion
    }

    private void OnTick_5(int obj)
    {
        aiInfo = new AiInfo()
        {
            position = transform.position,
        };

        blackboard.SetValue(AiMonsterKey, aiInfo);
    }

    private void Update()
    {
        if (!IsHost) return;
        root.Process();
    }

    #region IExpert implimentation

    public int GetInsistence(Blackboard blackboard)
    {
       int insistence = 0;
       return insistence;
    }

    public void Execute(Blackboard blackboard)
    {
    }

    #endregion
}