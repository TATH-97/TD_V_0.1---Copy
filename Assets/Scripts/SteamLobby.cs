using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

  //CallBacks
  protected Callback<LobbyCreated_t> LobbyCreated;
  protected Callback<GameLobbyJoinRequested_t> JoinRequest;
  protected Callback<LobbyEnter_t> LobbyEntered;

  //Vars
  public ulong CurrentLobbyID;
  private const string HostAddressKey = "HostAddress";
  private CustomNetworkManager manager;

  //Game Obj
//   public GameObject HostButton;
//   public Text LobbyNameText;  

  //Functions

   public void Start() {
    if(!SteamManager.Initialized) {
      Debug.Log("Turn on Steam!");
        return;
    }
    if(Instance==null) {
        Instance=this;
    }
    manager=GetComponent<CustomNetworkManager>();
    LobbyCreated = Callback<LobbyCreated_t>.Create(onLobbyCreated);
    JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(onJoinRequest);
    LobbyEntered = Callback<LobbyEnter_t> .Create(onLobbyEntered);
  } 

  public void HostLobby() {
    SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    Debug.Log("Host Lobby");
  }
  
  private void onLobbyCreated(LobbyCreated_t callback) {
    if(callback.m_eResult != EResult.k_EResultOK) {
        return;
    }
    Debug.Log("Lobby created");
    manager.StartHost();
    SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString()+"'s lobby");
  }
  
  private void onJoinRequest(GameLobbyJoinRequested_t callback) {
    Debug.Log("Request to join");
    SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
  }

  private void onLobbyEntered(LobbyEnter_t callback) {
    //Everyone
    CurrentLobbyID=callback.m_ulSteamIDLobby;
    

    //Client only
    if(NetworkServer.active) {
        return;
    }
    manager.networkAddress=SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
    manager.StartClient();
  }

}
