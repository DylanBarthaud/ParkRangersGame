using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CodeKey : NetworkBehaviour
{
    [SerializeField] Image[] symbols;
    [SerializeField] KeypadMiniGame game;

    public override void OnNetworkSpawn() 
    {
        if (IsServer) SetSymbolsClientRpc();
        DeactivateGameObj();
    }

    [ClientRpc]
    public void SetSymbolsClientRpc()
    {
        for (int i = 0; i < symbols.Length; i++)
            symbols[i].sprite = game.GetSymbol(i);
    }

    private void DeactivateGameObj() => game.gameObject.SetActive(false); 
}
