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

    [Header("Handlers")]
    [SerializeField] AudioHandler audioHandler;
    [SerializeField] GFXHandler gfxHandler;

    [Header("Blackboard")]
    [SerializeField] BlackboardController blackboardController;

    [Header("Base Settings")]
    [SerializeField] private float baseSpeed, investigateHintSpeed, chaseSpeed;

    [Header("Stalk Settings")]
    [SerializeField] private float stalkSpeed;
    [SerializeField] private float stalkTime; 
    [SerializeField] private float minStalkDist;
    [SerializeField] private float maxStalkDist;

    [Header("BurrowSettings")]
    [SerializeField] private float burrowSpeed; 

    Blackboard blackboard;
    BlackboardKey AiMonsterKey;
    AiInfo aiInfo;

    bool isBurrowed;

    private void Awake()
    {
        #region Set Up
        gfxHandler.DisableGFX("BurrowedGFX");
        audioHandler.PlaySound("BadgerWalking", true, default, 15);

        EventManager.instance.onTick_5 += OnTick_5;
        EventManager.instance.onBurrow += OnBurrow;
        EventManager.instance.onUnBurrow += OnUnBurrow;

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
        bool IsBurrowed()
        {
            return isBurrowed; 
        }
        Leaf unBurrow = new Leaf("UnBurrow", new UnBurrowStrategy(agent, baseSpeed, 0, IsBurrowed, gfxHandler, audioHandler));
        Leaf IsntBurrowed = new Leaf("IsntBurrowed", new Condition(() => (!IsBurrowed())));

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
        Leaf inSameCellAsPlayer = new Leaf("InSameCellAsPlayer", new Condition(InSameCellAsPlayer));
        Leaf inDifferentCellAsPlayer = new Leaf("InDifferentCellAsPlayer", new Condition(() => (!InSameCellAsPlayer())));

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
        Leaf stalkPlayer = new Leaf("StalkPlayer", new StalkPlayerStrategy(PlayerInfo, agent, stalkSpeed, stalkTime, minStalkDist, maxStalkDist, audioHandler)); 
        Leaf chasePlayer = new Leaf("ChasePlayer", new ChasePlayerStrategy(PlayerInfo, agent, chaseSpeed), 100);

        stalkPlayerSequence.AddChild(IsntBurrowed);
        stalkPlayerSequence.AddChild(stalkPlayer);
        stalkPlayerSequence.AddChild(chasePlayer);
        #endregion

        #region Investigate Hint
        Sequence burrowAndMoveToGridSequence = new Sequence("burrowAndMoveToGridSequence", 50);

        Leaf burrow = new Leaf("burrow", new BurrowStrategy(agent, burrowSpeed, 0, IsBurrowed, gfxHandler, audioHandler)); 
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
        Leaf moveToCell = new Leaf("MoveToPos", new MoveToPositionInCellStrategy(InvestigateHint, agent, null));

        burrowAndMoveToGridSequence.AddChild(inDifferentCellAsPlayer);
        burrowAndMoveToGridSequence.AddChild(burrow);
        burrowAndMoveToGridSequence.AddChild(moveToCell);
        burrowAndMoveToGridSequence.AddChild(unBurrow); 
        #endregion

        #region Search Cell
        Sequence searchCellSequence = new Sequence("SearchCellSequence", 90);
        Leaf searchCell = new Leaf("SearchCell", new SearchCellStrategy(() => GameManager.instance.mapHandler.GetGridLocation(transform.position), agent));

        searchCellSequence.AddChild(inSameCellAsPlayer); 
        searchCellSequence.AddChild(IsntBurrowed);
        searchCellSequence.AddChild(searchCell);    
        #endregion

        prioritySelector.AddChild(stalkPlayerSequence);
        prioritySelector.AddChild(searchCellSequence);
        prioritySelector.AddChild(burrowAndMoveToGridSequence);

        root.AddChild(prioritySelector);

        #endregion
    }

    private void OnUnBurrow(Vector3 vector)
    {
        isBurrowed = false;
    }

    private void OnBurrow(Vector3 vector)
    {
        isBurrowed = true;
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