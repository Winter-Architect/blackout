
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class HarmfulObject : NetworkBehaviour, IDamaging
{
    private int knockback;
    private int dmg;
    private Collider hurtbox;
    


    public HarmfulObject(int knockback, int dmg)
    {
        this.knockback = knockback;
        this.dmg = dmg;
        hurtbox = this.GetComponent<Collider>();
    }
    
    public void DealDamage(int dmg, int knockback, IDamageable damageable)
    {
        damageable.TakeDamage(this.dmg, this.knockback);
    }

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Has entered");
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Has detected the player");
            DealDamage(dmg, knockback, (IDamageable)collider);
        }
        
    }
    
}