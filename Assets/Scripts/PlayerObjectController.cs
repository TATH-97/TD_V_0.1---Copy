using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUdate))] public bool Ready;

    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager {
        get {
            if(manager!=null) {
                return manager;
            } else {
                return manager=CustomNetworkManager.singleton as CustomNetworkManager; 
            }
        }
    }
    private void PlayerReadyUdate(bool oldVal, bool newValue) {
        if(isServer) {
            Debug.Log("isServer");
            this.Ready=newValue;
            LobbyController.Instance.UpdatePlayerList();
        }
        if (isClient && !isServer) { //May need just isClient
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void cmdSetPlayerReady() {
        this.PlayerReadyUdate(this.Ready, !this.Ready);
    } 

    public void ChangeReady() {
        Debug.Log("ChangeReady");
        if(isOwned) {//hasAuthority 
            Debug.Log("isOwned");
            cmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority() {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        Debug.Log(SteamFriends.GetPersonaName().ToString());
        gameObject.name="LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient() {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient() {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command] 
    private void CmdSetPlayerName(string PlayerName) {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue) {
        if(isServer) { //Host
            this.PlayerName=NewValue;
        } 
        if(isClient && !isServer) {//client
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneName) {
        if(isOwned) {//hasAuthority
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName) {
        manager.StartGame(SceneName); //May need Manager
    }

}
