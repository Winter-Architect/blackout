using UnityEngine;
using UnityEngine.UI;
using Discord.Sdk;
using System.Linq;

public class DiscordManager : MonoBehaviour
{
    [SerializeField] 
    private ulong clientId; // Set this in the Unity Inspector from the dev portal
    
    [SerializeField]
    private Button loginButton; 
    
    [SerializeField] 
    private Text statusText;

    public Client client;
    private string codeVerifier;

    private static DiscordManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        client = new Client();

        // Modifying LoggingSeverity will show you more or less logging information
        client.AddLogCallback(OnLog, LoggingSeverity.Error);
        client.SetStatusChangedCallback(OnStatusChanged);

        // Make sure the button has a listener
        // if (loginButton != null)
        // {
        //     loginButton.onClick.AddListener(StartOAuthFlow);
        // }
        // else
        // {
        //     Debug.LogError("Login button reference is missing, connect it in the inspector!");
        // }

        // // Set initial status text
        // if (statusText != null)
        // {
        //     statusText.text = "Ready to login";
        // }
        // else
        // {
        //     Debug.LogError("Status text reference is missing, connect it in the inspector!");
        // }
    }

    private void OnLog(string message, LoggingSeverity severity)
    {
        Debug.Log($"Log: {severity} - {message}");
    }

    private void OnStatusChanged(Client.Status status, Client.Error error, int errorCode)
    {
        Debug.Log($"Status changed: {status}");
        // statusText.text = status.ToString();
        if(error != Client.Error.None)
        {
            Debug.LogError($"Error: {error}, code: {errorCode}");
        }

        if (status == Client.Status.Ready)
        {
            ClientReady();
        }
    }

    public void StartOAuthFlow() // Assurez-vous que cette méthode est publique
    {
        var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
        codeVerifier = authorizationVerifier.Verifier();
        
        var args = new AuthorizationArgs();
        args.SetClientId(clientId);
        args.SetScopes(Client.GetDefaultPresenceScopes());
        args.SetCodeChallenge(authorizationVerifier.Challenge());
        client.Authorize(args, OnAuthorizeResult);
    }

    private void OnAuthorizeResult(ClientResult result, string code, string redirectUri) {
        Debug.Log($"Authorization result: [{result.Error()}] [{code}] [{redirectUri}]");
        if (!result.Successful()) {
            return;
        }
        GetTokenFromCode(code, redirectUri);
    }

    private void GetTokenFromCode(string code, string redirectUri) {
        client.GetToken(clientId,
                        code,
                        codeVerifier,
                        redirectUri,
                        (result, token, refreshToken, tokenType, expiresIn, scope) => {
                            if (token != "") {
                                OnReceivedToken(token);
                            } else {
                                OnRetrieveTokenFailed();
                            }
                        });
    }

    private void OnReceivedToken(string token) {
        Debug.Log("Token received: " + token);
        client.UpdateToken(AuthorizationTokenType.Bearer, token, (ClientResult result) => { client.Connect(); });
    }

    private void OnRetrieveTokenFailed() { /*statusText.text = "Failed to retrieve token";*/ }

    private void ClientReady()
    {
        Activity activity = new Activity();
        activity.SetType(ActivityTypes.Playing);
        activity.SetState("In Lobby");
        // activity.SetDetails("Rank: Diamond II");
        client.UpdateRichPresence(activity, (ClientResult result) => {
            if (!result.Successful()) {
                Debug.LogError("Failed to update rich presence");
            }
        });
    }

    public void UpdateRichPresence(Activity activity)
    {
        client.UpdateRichPresence(activity, (ClientResult result) => {
            if (!result.Successful()) {
                Debug.LogError("Failed to update rich presence");
            }
        });
    }
}
