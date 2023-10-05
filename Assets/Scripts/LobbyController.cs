using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    //UI elements
    public Text LobbyNameText;

    //Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated =false;
    public List<PlayerListItem> PlayerListItems=new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    //Ready
    public Button StartGameButton;
    public Text ReadyButtonText;

    public Toggle defenderUI;

    //Manager 
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

    private void Awake() {
        if(Instance==null) {
            Instance=this;
        }
    }

    public void DefenderUI() {
        Debug.Log("UI Change, LobbyController " + LocalPlayerController.PlayerName);
        LocalPlayerController.ChangeUIBool();
    }

    public void ReadyPlayer() {
        Debug.Log("ReadyPlayer");
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton() {
        if(LocalPlayerController.Ready) {
            ReadyButtonText.text="Unready";
        } else {
            ReadyButtonText.text="Ready";
        }
    }

    public void CheckIfAllReady() {
        bool allReady=false;
        foreach(PlayerObjectController player in Manager.GamePlayers)  {
            if(player.Ready) {
                allReady=true;
            } else {
                allReady=false;
                break;
            }
        }
        if(allReady) {
            if(LocalPlayerController.PlayerIDNumber==1) { //if host
                StartGameButton.interactable=true;
            } else {
                StartGameButton.interactable=false;
            }
        } else {
            StartGameButton.interactable=false;
        }
    }

    public void UpdateLobbyName() {
        CurrentLobbyID=Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text=SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList() {
        //Debug.Log("PlayerListItems.Count: "+PlayerListItems.Count+"\n"+"Manager.GamePlayers.Count: "+Manager.GamePlayers.Count);
        if(!PlayerItemCreated) {
            CreateHostPlayerItem(); //Host 
        } 
        if(PlayerListItems.Count < Manager.GamePlayers.Count) { //May need 'manager'
            CreateClientPlayerItem();
        }
        if(PlayerListItems.Count > Manager.GamePlayers.Count) {
            Debug.Log("Remove Player");
            RemovePlayerItem();
        }
        if(PlayerListItems.Count == Manager.GamePlayers.Count) {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer() {
        LocalPlayerObject =GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>(); 
    }

    public void CreateHostPlayerItem() { //host player 
    Debug.Log("Host player");
        foreach(PlayerObjectController player in Manager.GamePlayers) {
            GameObject NewPlayerItem=Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript=NewPlayerItem.GetComponent<PlayerListItem>();
            NewPlayerItemScript.PlayerName=player.PlayerName;
            NewPlayerItemScript.ConnectionID=player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID=player.PlayerSteamID;
            NewPlayerItemScript.Ready=player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale=Vector2.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated=true;

    }

    public void CreateClientPlayerItem() { //client player
    Debug.Log("Client player");
        foreach(PlayerObjectController player in Manager.GamePlayers) {
            if(!PlayerListItems.Any(b => b.ConnectionID ==player.ConnectionID)) {
                GameObject NewPlayerItem=Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript=NewPlayerItem.GetComponent<PlayerListItem>();
                NewPlayerItemScript.PlayerName=player.PlayerName;
                NewPlayerItemScript.ConnectionID=player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID=player.PlayerSteamID;
                NewPlayerItemScript.Ready=player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale=Vector2.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem() {
        foreach(PlayerObjectController player in Manager.GamePlayers) {
            foreach(PlayerListItem PlayerListItemScript in PlayerListItems) {
                if(PlayerListItemScript.ConnectionID==player.ConnectionID) {
                    PlayerListItemScript.PlayerName=player.PlayerName;
                    PlayerListItemScript.Ready=player.Ready;
                    Debug.Log(player.Ready.ToString());
                    PlayerListItemScript.SetPlayerValues();
                    if(player==LocalPlayerController) {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem() {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
        foreach(PlayerListItem playerListItem in PlayerListItems) {
            if(!Manager.GamePlayers.Any(b=>b.ConnectionID==playerListItem.ConnectionID)) {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if(playerListItemsToRemove.Count>0) {
            foreach(PlayerListItem playerListItem in playerListItemsToRemove) {
                GameObject ObjectToRemove =playerListItem.gameObject;
                PlayerListItems.Remove(playerListItem);
                Destroy(ObjectToRemove);
                ObjectToRemove=null;
            }
        }
    }

    public void StartGame(string SceneName) {
        LocalPlayerController.CanStartGame(SceneName);
    }

}
