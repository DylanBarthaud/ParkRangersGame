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
            SetSymbolsClientRpc(game.SymbolSeed, game.Code);
        }

        DeactivateGameObj();
    }

    [ClientRpc]
    public void SetSymbolsClientRpc(int[] symbolSeed, int[] code)
    {
        game.SetSymbolSeed(symbolSeed);
        game.SetCode(code);
        game.SetSymbolCode();
        for (int i = 0; i < symbols.Length; i++)
            symbols[i].sprite = game.GetSymbol(symbolSeed[i]);

    }

    private void DeactivateGameObj() => game.gameObject.SetActive(false); 
}