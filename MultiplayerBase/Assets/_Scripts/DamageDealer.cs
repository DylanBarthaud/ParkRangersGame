using System.Collections;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damageAmount = 0;
    private bool canDamadge = true;

    private void OnTriggerEnter(Collider collision)
    {
        if (canDamadge)
        {
            canDamadge = false;
            IHurtable hurtableObj = collision.gameObject.GetComponent<IHurtable>();
            hurtableObj.IsHurt("Monster_Ai", damageAmount);
            StartCoroutine(AttackCD()); 
        }
    }

    private IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(5);
        canDamadge = true;
    }
}
