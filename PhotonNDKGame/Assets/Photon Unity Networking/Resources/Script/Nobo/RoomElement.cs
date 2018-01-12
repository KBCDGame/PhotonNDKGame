using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomElement : MonoBehaviour {

    //ルーム情報UI表示用。
    [SerializeField]
    private Text RoomName;   //部屋名。
    [SerializeField]
    private Text PlayerNum;   //人数。
    [SerializeField]
    private Text RoomCreator;    //部屋作成者名。

    //入室ボタンルーム名格納用。
    private string WorkRoomName;

    //GetRoomListからルーム情報をRoomElementにセットしていくための関数。
    public void SetRoomInfo(string _RoomName, int _PlayerNumber, int _MaxPlayer, string _RoomCreator)
    {
        //入室ボタン用ルーム名取得。
        WorkRoomName = _RoomName;
        RoomName.text = "部屋名：" + _RoomName;
        PlayerNum.text = "人　数：" + _PlayerNumber + "/" + _MaxPlayer;
        RoomCreator.text = "作成者：" + _RoomCreator;
    }

    //入室ボタン処理。
    public void OnJoinRoomButton()
    {
        //WorkRoomNameの部屋に入室。
        PhotonNetwork.JoinRoom(WorkRoomName);
    }
}
