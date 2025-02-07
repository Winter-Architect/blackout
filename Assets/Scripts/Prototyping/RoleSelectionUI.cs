using UnityEngine;
using UnityEngine.UI;

public class RoleSelectionUI : MonoBehaviour
{
    [SerializeField] private Button agentButton;
    [SerializeField] private Button supportButton;
    [SerializeField] private Button spectatorButton;

    private void Start()
    {
        agentButton.onClick.AddListener(() => SelectRole("Agent"));
        supportButton.onClick.AddListener(() => SelectRole("Support"));
        spectatorButton.onClick.AddListener(() => SelectRole("Spectator"));
    }
        private void SelectRole(string role)
    {
        if (PrototypePlayerSpawner.Instance != null)
        {
            PrototypePlayerSpawner.Instance.RequestSpawn(role);
        }
    }

}
