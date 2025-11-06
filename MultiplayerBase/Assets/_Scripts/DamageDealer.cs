using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damageAmount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        IHurtable hurtableObj = collision.gameObject.GetComponent<IHurtable>();
        hurtableObj.IsKilled(); 
    }
}
