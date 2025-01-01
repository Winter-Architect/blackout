using Unity.Netcode;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<string> PlayerName = new NetworkVariable<string>("Unknown");
    public NetworkVariable<ulong> PlayerID = new NetworkVariable<ulong>(ulong.MaxValue);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerName.Value = $"Player {OwnerClientId}";
            PlayerID.Value = OwnerClientId;
        }
    }
}
