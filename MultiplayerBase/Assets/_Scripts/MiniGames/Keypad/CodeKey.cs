using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CodeKey : NetworkBehaviour
{
    [SerializeField] Image[] symbols;
    [SerializeField] KeypadMiniGame game;

    public override void OnNetworkSpawn() 
    {
        if (IsServer)
        {
            game.Reset();
            SetSymbolsClientRpc(game.SymbolSeed);
        }

        DeactivateGameObj();
    }

    [ClientRpc]
    public void SetSymbolsClientRpc(int[] symbolSeed)
    {
        game.SetSymbolSeed(symbolSeed);
        for (int i = 0; i < symbols.Length; i++)
            symbols[i].sprite = game.GetSymbol(symbolSeed[i]);
    }

    private void DeactivateGameObj() => game.gameObject.SetActive(false); 
}