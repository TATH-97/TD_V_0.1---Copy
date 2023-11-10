using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public Text PlayerNameText;
    public Text PlayerReadyText;
    public bool Ready;
    public RawImage PlayerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded; 

    public void Start() {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        PlayerNameText.text=PlayerName;
    } 

    public void SetPlayerValues() {
        if(!AvatarReceived) {
            GetPlayerIcon();
        }
        ChangeReadyStatus();
        PlayerNameText.text=PlayerName;
    }


    void GetPlayerIcon() {
        int ImageID=SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if(ImageID==-1) {
            return;
        }
        PlayerIcon.texture=GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage) {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if(isValid) {
            byte[] image =new byte [width*height*4];

            isValid= SteamUtils.GetImageRGBA(iImage, image, (int)(width*height*4));

            if(isValid) {
                texture=new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived=true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback) {
        if(callback.m_steamID.m_SteamID == PlayerSteamID) { //us
            PlayerIcon.texture=GetSteamImageAsTexture(callback.m_iImage);
        } else { //other player
            return;
        }
    }

    public void ChangeReadyStatus() {
        if(Ready) { //ready
            PlayerReadyText.text="Ready";
            PlayerReadyText.color=Color.green;
        } else {
            PlayerReadyText.text="Unready";
            PlayerReadyText.color=Color.red;
        }
    }

}
