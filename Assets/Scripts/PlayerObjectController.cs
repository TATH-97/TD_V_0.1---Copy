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

    public bool isDefender=false;

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

    public void ChangeUIBool() {
        if(isOwned) {
            this.isDefender=!isDefender;
        }
        Debug.Log("isDefender= "+isDefender.ToString());
    }  

    private void PlayerReadyUdate(bool oldVal, bool newValue) {
        if(isServer) {
            //Debug.Log("isServer");
            this.Ready=newValue;
            LobbyController.Instance.UpdatePlayerList();
        }
        if (isClient) { //May need && !isServer
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void cmdSetPlayerReady() {
        this.PlayerReadyUdate(this.Ready, !this.Ready);
    } 

    public void ChangeReady() {
        //Debug.Log("ChangeReady");
        if(isOwned) {//hasAuthority 
           // Debug.Log("isOwned");
            cmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority() {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        //Debug.Log(SteamFriends.GetPersonaName().ToString());
        gameObject.name="LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient() {
        Debug.Log("Client?");
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient() {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command] 
    private void CmdSetPlayerName(string input) {
        this.PlayerNameUpdate(this.PlayerName, input);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue) {
        if(isServer) { //Host
            this.PlayerName=NewValue;
        } 
        if(isClient) { //isClient && !isServer
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
