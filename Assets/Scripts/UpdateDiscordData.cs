using UnityEngine;
using Discord.Sdk;
using System;

public class UpdateDiscordData : MonoBehaviour {
    private DiscordManager discordManager;
    private Room[] foundRooms;
    private Room currentRoom;
    private Agent player1;
    private long startTimestamp;

    void Start() {
        try {
            discordManager = FindObjectOfType<DiscordManager>();
            foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
            player1 = FindFirstObjectByType<Agent>();

            if (discordManager != null && discordManager.client != null) {
                discordManager.client.AddLogCallback(OnLog, LoggingSeverity.Error);
                discordManager.client.SetStatusChangedCallback(OnStatusChanged);
                Debug.Log("DiscordManager and client initialized successfully in Start");
                startTimestamp = GetCurrentUnixTimestamp();
            } else {
                Debug.Log("DiscordManager or client is null in Start");
            }
        } catch (System.Exception ex) {
            Debug.Log("Exception in Start: " + ex.Message);
        }
    }

    private void Update() {
        try {
            if (player1 == null) {
                Debug.Log("Player is null in Update");
                player1 = FindFirstObjectByType<Agent>();
                return;
            } else {
                Debug.Log("Player is not null");
            }

            if (discordManager == null || discordManager.client == null) {
                Debug.Log("DiscordManager or client is null in Update");
                return;
            }

            if (discordManager.client.GetStatus() != Client.Status.Ready) {
                Debug.Log("Discord client is not ready");
                return;
            }

            foreach (var room in foundRooms) {
                if (room.ContainsPlayer(player1)) {
                    currentRoom = room;
                    SetNewActivity();
                    break;
                }
            }
        } catch (System.Exception ex) {
            Debug.Log("Exception in Update: " + ex.Message);
        }
    }

    private void SetNewActivity() {
        try {
            Activity activity = new Activity();
            activity.SetType(ActivityTypes.Playing);
            activity.SetState("Current room : " + currentRoom.RoomID);
            activity.SetDetails("In Game");

            // Créez un objet ActivityTimestamps et définissez le timestamp
            ActivityTimestamps timestamps = new ActivityTimestamps();
            timestamps.SetStart((ulong)startTimestamp); // Convertissez le long en ulong
            activity.SetTimestamps(timestamps);

            discordManager.UpdateRichPresence(activity);
        } catch (System.Exception ex) {
            Debug.Log("Exception in SetNewActivity: " + ex.Message);
        }
    }

    private void OnApplicationQuit() {
            try {
                if (discordManager != null && discordManager.client != null) {
                    // Activity activity = new Activity();
                    // activity.SetType(ActivityTypes.None); // Arrête l'activité
                    // discordManager.UpdateRichPresence(activity);
                    discordManager.client.Dispose();
                    Debug.Log("Stopped Discord Rich Presence");
                }
            } catch (System.Exception ex) {
                Debug.LogError("Exception in OnApplicationQuit: " + ex.Message);
            }
        }

    private void OnLog(string message, LoggingSeverity severity) {
        Debug.Log($"Log: {severity} - {message}");
    }

    private void OnStatusChanged(Client.Status status, Client.Error error, int errorCode) {
        Debug.Log($"Status changed: {status}");
        if (error != Client.Error.None) {
            Debug.LogError($"Error: {error}, code: {errorCode}");
        }
    }

    private long GetCurrentUnixTimestamp() {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}