using UnityEngine;

public class GravityTesting : MonoBehaviour
{
    public Rigidbody rb;
    public float Yvelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Yvelocity = rb.linearVelocity.y;
    }
}
