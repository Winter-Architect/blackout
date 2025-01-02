using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void  Awake()
    {
        serverBtn.onClick.AddListener
        ( 
            () => { NetworkManager.Singleton.StartServer(); }
        );
        hostBtn.onClick.AddListener
        ( 
            () => { NetworkManager.Singleton.StartHost(); }
        );
        clientBtn.onClick.AddListener
        ( 
            () => { NetworkManager.Singleton.StartClient(); }
        );
    }
}
