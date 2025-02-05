using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;


public class PlayerNetwork : NetworkBehaviour, IDamageable
{
    [SerializeField] private float hp;
    public Rigidbody rb; // Assign in Inspector or get via script
    public static PlayerNetwork LocalPlayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (IsOwner) // Check if this is the local player
        {
            LocalPlayer = this;
        }
        rb = GetComponent<Rigidbody>();
        this.hp = 200;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

         Vector3 moveDir = new Vector3(0, 0, 0);

         if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
         if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
         if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
         if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 8f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(float dmg, float knokback)
    {
        if (this.hp<=0)
        {
            GetDestroyed();
        }
        Debug.Log("hit");
        this.hp -= dmg;
    }

    public void GetDestroyed()
    {
        Destroy(gameObject);
    }
}
