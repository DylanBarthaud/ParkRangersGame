using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damageAmount = 0;

    private void OnTriggerEnter(Collider collision)
    {
        IHurtable hurtableObj = collision.gameObject.GetComponent<IHurtable>();
        hurtableObj.IsKilled();
    }
}
