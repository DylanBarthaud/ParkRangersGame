using BehaviourTrees;
using BlackboardSystem;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BagerAi : NetworkBehaviour
{
    private NavMeshAgent agent;

    Root root;

    [Header("Handlers")]
    [SerializeField] MultiplayerAudioHandlerWrapper audioHandler;
    [SerializeField] GFXHandler gfxHandler;

    [Header("Blackboard")]
    [SerializeField] BlackboardController blackboardController;

    [Header("Base Settings")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float investigateHintSpeed;

    [Header("Hunt Settings")]
    [SerializeField] private float chaseSpeed;
    [SerializeField] private int minCrowsNeededToHunt = 3;

    [Header("Area Search Settings")]
    [SerializeField] private int crowsNeededForAreaSearch = 5;
    [SerializeField] private float distanceFromPlayerToUnburrow = 25;
    [SerializeField] private float distanceFromPlayerToburrow = 35;
    [SerializeField] private Vector2 searchAreaBoundries; 

    [Header("Stalk Settings")]
    [SerializeField] private float stalkSpeed;
    [SerializeField] private float stalkTime; 
    [SerializeField] private float minStalkDist;
    [SerializeField] private float maxStalkDist;
    [SerializeField] private float ObjectPermanenceTimer;
    private float CurrentObjectPermamenceTime; 

    [Header("BurrowSettings")]
    [SerializeField] private float burrowSpeed; 

    Blackboard blackboard;
    BlackboardKey AiMonsterKey;
    AiInfo aiInfo;

    private bool isBurrowedBool;
    private bool canHunt = false; 
    private bool aiIsActive = true;

    private void Awake()
    {
        #region Set Up

        EventManager.instance.onTick_5 += OnTick_5;
        EventManager.instance.onBurrow += OnBurrow;
        EventManager.instance.onUnBurrow += OnUnBurrow;
        EventManager.instance.onPlayerSpawned += OnPlayerSpawned;
        EventManager.instance.onPlayerKilled += OnPlayerKilled;
        EventManager.instance.onPlayerHurt += OnPlayerHurt; 

        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;

        aiInfo = new AiInfo()
        {
            position = transform.position,
        };

        blackboard = blackboardController.GetBlackboard();
        AiMonsterKey = blackboard.GetOrRegisterKey("AiMonsterKey");
        blackboard.SetValue(AiMonsterKey, aiInfo);

        #endregion

        #region Behaviour Tree

        int GetHighestCrowAmount()
        {
            int HighestAmount = 0;
            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    if (playerInfo.ravenCount > HighestAmount) HighestAmount = playerInfo.ravenCount;
                }

                else Debug.LogError("Cannot find blackboard value with key:" + playerKey);
            }

            // Debug.Log(playerHasSufficientCrows ? canHunt : false); 
            return HighestAmount;
        }
        float GetDistToNearestPlayer()
        {
            float nearest = 1000;

            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    float distance = Vector3.Distance(transform.position, playerInfo.position);
                    if(distance < nearest) nearest = distance;     
                }
            }

            return nearest; 
        }
        bool CanHunt()
        {
            bool playerHasSufficientCrows = false;
            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    if(playerInfo.ravenCount >= minCrowsNeededToHunt)
                    {
                        playerHasSufficientCrows = true;
                        break;
                    }
                }

                else Debug.LogError("Cannot find blackboard value with key:" +  playerKey);
            }

           // Debug.Log(playerHasSufficientCrows ? canHunt : false); 
            return playerHasSufficientCrows ? canHunt : false;
        }
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
        GridPosition GetHomeCellPos()
        {
            GridPosition aiHomePosition = new GridPosition { x = 0, z = 0 };
            BlackboardKey overlordKey = blackboard.GetOrRegisterKey("AiOverlordKey");
            if (blackboard.TryGetValue(overlordKey, out OverlordGivenInfo overlordInfo))
            {
                aiHomePosition = overlordInfo.homeGrid;
            }

            //Debug.Log($"Ai Home Pos: {aiHomePosition.x} {aiHomePosition.z}");
            return aiHomePosition;
        }
        bool InHomeCell()
        {
            bool inHomeCell = false;

            MapHandler mapHandler = GameManager.instance.mapHandler;

            GridPosition aiPosition = new GridPosition { x = 0, z = 0 };
            if (blackboard.TryGetValue(AiMonsterKey, out AiInfo monsterInfo))
            {
                aiPosition = mapHandler.GetGridLocation(monsterInfo.position);
            }

            GridPosition aiHomePosition = GetHomeCellPos();

            float distance = mapHandler.GetDistanceBetweenGrids(aiPosition, aiHomePosition);
            if (distance <= 0) inHomeCell = true;

            return inHomeCell;
        }
        bool IsBurrowed()
        {
            return isBurrowedBool;
        }
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
        Vector2 GetSearchArea()
        {
            float nearest = 1000;
            PlayerInfo player = new(); 
            foreach (BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo playerInfo))
                {
                    float distance = Vector3.Distance(transform.position, playerInfo.position);
                    if (distance < nearest)
                    {
                        nearest = distance;
                        player = playerInfo;
                    }
                }
            }

            Vector2 bounries = new Vector2 (searchAreaBoundries.x, searchAreaBoundries.y);

            float x = Random.Range(-bounries.x, bounries.x); 
            float y = Random.Range(-bounries.y, bounries.y);

            return new Vector2 (player.position.x + x, player.position.z + y);
        }

        root = new Root("Root");

        Selector canHuntSelector = new Selector("CanHuntSelector");
        IfGate canHuntLeaf = new IfGate("CanHunt", new Condition(() => CanHunt()));

        Selector inHomeCellSelector = new Selector("InHomeCellSelector");

        IfGate inHomeCellLeaf = new IfGate("InHomeCell", new Condition(() => InHomeCell()));
        Leaf setCanHuntTrue = new Leaf("setCanHunt", new ActionStrategy(() => { canHunt = true; }));

        Sequence goToHomeCellSequence = new Sequence("GoToHomeCellSequence");
        Leaf moveToHomeCell = new Leaf("MoveToHomeCell", new SearchCellStrategy(GetHomeCellPos, agent));

        PrioritySelector huntSelector = new PrioritySelector("HuntSelector");

        Leaf unBurrow = new Leaf("UnBurrow", new UnBurrowStrategy(agent, baseSpeed, 0, IsBurrowed, gfxHandler, audioHandler));
        Leaf isntBurrowed = new Leaf("IsntBurrowed", new Condition(() => (!IsBurrowed())));
        Leaf isBurrowed = new Leaf("IsntBurrowed", new Condition(() => (IsBurrowed())));

        Leaf inSameCellAsPlayer = new Leaf("InSameCellAsPlayer", new Condition(() => (InSameCellAsPlayer())));
        Leaf inDifferentCellAsPlayer = new Leaf("InDifferentCellAsPlayer", new Condition(() => (!InSameCellAsPlayer())));

        Sequence stalkPlayerSequence = new Sequence("StalkPlayerSequence", 100);

        PlayerInfo PlayerInfo() 
        {
            List<PlayerInfo> seenPlayers = new List<PlayerInfo>(); 
            foreach(BlackboardKey playerKey in GameManager.instance.playerBlackboardKeys)
            {
                if (blackboard.TryGetValue(playerKey, out PlayerInfo info))
                {
                    float timeSinceSeen = Time.time - info.lastTimePlayerSeen; 

                    if (info.canSeePlayer
                        || timeSinceSeen <= ObjectPermanenceTimer) seenPlayers.Add(info);
                }
            }

            if (seenPlayers.Count == 0)
            {
                PlayerInfo placeHolderInfo = new PlayerInfo()
                {
                    canSeePlayer = false,
                    importance = 0,
                    position = Vector3.zero,
                    lastTimePlayerSeen = -1
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

        Sequence burrowAndMoveToGridSequence = new Sequence("burrowAndMoveToGridSequence", 50);

        Leaf burrow = new Leaf("burrow", new BurrowStrategy(agent, burrowSpeed, 0, IsBurrowed, gfxHandler, audioHandler)); 
        Leaf moveToCell = new Leaf("MoveToPos", new SearchCellStrategy(InvestigateHint, agent));

        Sequence searchCellSequence = new Sequence("SearchCellSequence", 90);
        Leaf searchCell = new Leaf("SearchCell", new SearchCellStrategy(() => GameManager.instance.mapHandler.GetGridLocation(transform.position), agent));

        Sequence searchAreaSeq = new Sequence("SearchAreaSeq", 95);
        Leaf canSearchArea = new Leaf("CanSearchArea", new Condition(() => { return GetHighestCrowAmount() >= crowsNeededForAreaSearch ? true : false; }));
        PrioritySelector searchAreaSelector = new PrioritySelector("SearchAreaSelector", 95);
        Sequence closeToPlayerSeq = new Sequence("CloseToPlayerSeq", 100); 
        Leaf closeToPlayer = new Leaf("CloseToPlayer", new Condition(() => { return GetDistToNearestPlayer() <= distanceFromPlayerToUnburrow ? true : false; }));
        Sequence farFromPlayerSeq = new Sequence("FarFromPlayerSeq", 95);
        Leaf farFromPlayer = new Leaf("FarFromPlayer", new Condition(() => { return GetDistToNearestPlayer() > distanceFromPlayerToburrow ? true : false;  }));
        Leaf searchArea = new Leaf("SearchArea", new SearchAreaStrategy(GetSearchArea, agent), 90);
        Leaf closeToPlayerSearchArea = new Leaf("SearchArea", new SearchAreaStrategy(GetSearchArea, agent), 90);
        Leaf farFromPlayerSearchArea = new Leaf("SearchArea", new SearchAreaStrategy(GetSearchArea, agent), 90);

        root.AddChild(canHuntSelector);
          
        canHuntSelector.AddChild(canHuntLeaf);
        canHuntLeaf.AddChild(huntSelector);

        huntSelector.AddChild(stalkPlayerSequence);
        stalkPlayerSequence.AddChild(isntBurrowed);
        stalkPlayerSequence.AddChild(stalkPlayer);
        stalkPlayerSequence.AddChild(chasePlayer);

        huntSelector.AddChild(searchAreaSeq);
        searchAreaSeq.AddChild(canSearchArea);
        searchAreaSeq.AddChild(searchAreaSelector);
        searchAreaSelector.AddChild(closeToPlayerSeq);
        closeToPlayerSeq.AddChild(isBurrowed); 
        closeToPlayerSeq.AddChild(closeToPlayer);
        closeToPlayerSeq.AddChild(unBurrow);
        closeToPlayerSeq.AddChild(closeToPlayerSearchArea);
        searchAreaSelector.AddChild(farFromPlayerSeq);
        farFromPlayerSeq.AddChild(isntBurrowed); 
        farFromPlayerSeq.AddChild(farFromPlayer);
        farFromPlayerSeq.AddChild(burrow);
        farFromPlayerSeq.AddChild(farFromPlayerSearchArea);
        searchAreaSelector.AddChild(searchArea);

        huntSelector.AddChild(searchCellSequence);
        searchCellSequence.AddChild(inSameCellAsPlayer);
        searchCellSequence.AddChild(isntBurrowed);
        searchCellSequence.AddChild(searchCell);

        huntSelector.AddChild(burrowAndMoveToGridSequence);
        burrowAndMoveToGridSequence.AddChild(inDifferentCellAsPlayer);
        burrowAndMoveToGridSequence.AddChild(burrow);
        burrowAndMoveToGridSequence.AddChild(moveToCell);
        burrowAndMoveToGridSequence.AddChild(unBurrow);

        canHuntSelector.AddChild(inHomeCellSelector);
        inHomeCellSelector.AddChild(inHomeCellLeaf);
        inHomeCellLeaf.AddChild(setCanHuntTrue);

        inHomeCellSelector.AddChild(goToHomeCellSequence);
        goToHomeCellSequence.AddChild(burrow);
        goToHomeCellSequence.AddChild(moveToHomeCell);
        goToHomeCellSequence.AddChild(unBurrow); 
        #endregion
    }

    public override void OnNetworkDespawn()
    {
        EventManager.instance.onTick_5 -= OnTick_5;
        EventManager.instance.onBurrow -= OnBurrow;
        EventManager.instance.onUnBurrow -= OnUnBurrow;
        EventManager.instance.onPlayerSpawned -= OnPlayerSpawned;
        EventManager.instance.onPlayerKilled -= OnPlayerKilled;
        EventManager.instance.onPlayerHurt -= OnPlayerHurt;
    }

    private void OnPlayerSpawned(BlackboardKey key, ulong clientId) => root.Reset();
    private void OnPlayerHurt()
    {
        canHunt = false;
        root.Reset();
    }
    private void OnPlayerKilled(BlackboardKey key)
    {
        canHunt = false; 

        //Play animation

        root.Reset();
    }

    private void Start()
    {
        audioHandler.PlaySoundServerRpc("BadgerWalking", true, default, 15);
        gfxHandler.DisableGFXServerRpc("BurrowedGFX");
    }

    private void OnUnBurrow(Vector3 vector) => isBurrowedBool = false;
    private void OnBurrow(Vector3 vector) => isBurrowedBool = true;

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
        if (!IsServer || !aiIsActive) return;
        root.Process();
    }
}