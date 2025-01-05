using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent] // Prevents you from adding the ClientNetworkTransform to the object more than once.
public class ClientNetworkTransform : NetworkTransform
{

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
    
}