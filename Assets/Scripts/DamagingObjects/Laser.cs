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
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return; // Ensures only the server runs this

        IDamageable damageable = other.gameObject.GetComponent<NetworkBehaviour>() as IDamageable;
        if (damageable != null)
        {
            damageable.TakeDamage(dmg, knockback);
        }
        Destroy(this);
    }
    
}