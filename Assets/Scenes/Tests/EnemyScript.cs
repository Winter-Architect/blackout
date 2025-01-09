using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public float speed;



    void Update()
    {

        if(player)
        {
            Vector3 direction = player.transform.position - transform.position;
            transform.position += direction * speed * Time.deltaTime;
        } 
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
