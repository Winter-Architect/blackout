using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : NetworkBehaviour
{
    private Transform prefab;
    private float dmg;
    private float knockback;
    private float lifetime;
    private float speed;
    private Rigidbody rb;

    public void Initialize(float dmg, float knockback, float speed, float lifetime)
    {
        this.dmg = dmg;
        this.knockback = knockback;
        this.speed = speed;
        this.lifetime = lifetime;
        
        Destroy(gameObject, lifetime);
    }
    
    
    
    public void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable is not null)
        {
            damageable.TakeDamage(dmg, knockback);
        }
        Destroy(gameObject);
    }
    
}