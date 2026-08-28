using System.Collections;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damageAmount = 0;
    [SerializeField] MultiplayerAudioHandlerWrapper multiplayerAudioHandler;
    private bool canDamadge = true;

    private void OnTriggerEnter(Collider collision)
    {
        IHurtable hurtableObj = collision.gameObject.GetComponent<IHurtable>();

        if (canDamadge && hurtableObj != null)
        {
            canDamadge = false;
            hurtableObj.IsHurt("Monster_Ai", damageAmount);
            multiplayerAudioHandler.PlaySoundServerRpc("BadgerBite"); 
            StartCoroutine(AttackCD()); 
        }
    }

    private IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(5);
        canDamadge = true;
    }
}
