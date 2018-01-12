using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour {

    [SerializeField]
    //部屋作成ウインドウ表示用ボタン。
    private Button OpenRoomPanelButton;

    [SerializeField]
    private GameObject CreateRoomPanel;  //部屋作成ウインドウ。
    [SerializeField]
    private Text RoomNameText;           //作成する部屋名。
    [SerializeField]
    private Slider PlayerNumSlider;   //最大入室可能人数用Slider。
    [SerializeField]
    private Text PlayerNumText;       //最大入室可能人数表示用Text。
    [SerializeField]
    private Button CreateRoomButton;     //部屋作成ボタン。

    // Update is called once per frame
    void Update()
    {
        //部屋人数Sliderの値をTextに代入。
        PlayerNumText.text = PlayerNumSlider.value.ToString();
    }

    //部屋作成ウインドウ表示用ボタンを押したときの処理。
    public void OnClick_OpenRoomPanelButton()
    {
        //部屋作成ウインドウが表示していれば。
        if (CreateRoomPanel.activeSelf)
        {
            //部屋作成ウインドウを非表示に。
            CreateRoomPanel.SetActive(false);
        }
        else //そうでなければ。
        {
            //部屋作成ウインドウを表示。
            CreateRoomPanel.SetActive(true);
        }
    }

    //部屋作成ボタンを押したときの処理。
    public void OnClick_CreateRoomButton()
    {
        //作成する部屋の設定。
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;   //ロビーで見える部屋にする。
        roomOptions.IsOpen = true;      //他のプレイヤーの入室を許可する。
        roomOptions.MaxPlayers = (byte)PlayerNumSlider.value;    //入室可能人数を設定。
        //ルームカスタムプロパティで部屋作成者を表示させるため、作成者の名前を格納。
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "RoomCreator",PhotonNetwork.playerName }
        };
        //ロビーにカスタムプロパティの情報を表示させる。
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            "RoomCreator",
        };

        //部屋作成。
        PhotonNetwork.CreateRoom(RoomNameText.text, roomOptions, null);
    }
}
