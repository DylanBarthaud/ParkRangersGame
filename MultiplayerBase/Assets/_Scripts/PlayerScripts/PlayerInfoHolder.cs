using BlackboardSystem;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfoHolder : NetworkBehaviour, IAiViewable
{
    private BlackboardKey playerInfo_Key;
    private PlayerInfo playerInfo;

    public void UpdateInfo(bool playerSeen)
    {
        playerInfo = new PlayerInfo() 
        { 
            position = gameObject.transform.position,
            canSeePlayer = playerSeen
        };
    }

    public int GetImportance()
    {
        return 100; 
    }

    public void OnSeen(Blackboard blackboard)
    {
        switch (OwnerClientId)
        {
            case 0:
                playerInfo_Key = blackboard.GetOrRegisterKey("PlayerOneInfoKey");
                break;
            case 1:
                playerInfo_Key = blackboard.GetOrRegisterKey("PlayerTwoInfoKey");
                break;
            case 2:
                playerInfo_Key = blackboard.GetOrRegisterKey("PlayerThreeInfoKey");
                break;
            case 3:
                playerInfo_Key = blackboard.GetOrRegisterKey("PlayerFourInfoKey");
                break;
            default:
                throw new NotImplementedException();
        }

        UpdateInfo(true);

        blackboard.AddAction(() =>
        {
            blackboard.SetValue(playerInfo_Key, playerInfo);
        });
    }
}