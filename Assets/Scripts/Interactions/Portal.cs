using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : NetworkBehaviour
{
    public bool canTraverse = false;

    public void setTraverse(bool isTraversable)
    {
        canTraverse = isTraversable;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player" && canTraverse)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("DemoTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

}
