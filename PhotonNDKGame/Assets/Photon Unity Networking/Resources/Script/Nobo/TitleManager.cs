﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : Photon.PunBehaviour
{
    string _gameVersion = "test";   //ゲームのバージョン。仕様が異なるバージョンとなったときはバージョンを変更しないとエラーが発生する。
   
    public void Start()
    {
        Application.targetFrameRate = 60;
        SoundManager.Instance.PlayBGM(0);
    }

    //ログインボタンを押したときに実行される
    public void Connect()
    {
      
        if (!PhotonNetwork.connected)
        {                         //Photonに接続できていなければ。
            PhotonNetwork.ConnectUsingSettings(_gameVersion);   //Photonに接続する。
            Debug.Log("Photonに接続しました。");
            PhotonNetwork.LoadLevel("Lobby");//ロビーシーンをロード。
        }
        //ロビーBGM
        SoundManager.Instance.PlayBGM(1);
    }

    ////Auto-JoinLobbyにチェックを入れているとPhotonに接続後OnJoinLobby()が呼ばれる。
    //public override void OnJoinedLobby()
    //{
    //    Debug.Log("ロビーに入りました。");
    //    //Randomで部屋を選び、部屋に入る（部屋が無ければOnPhotonRandomJoinFailedが呼ばれる）。
    //    PhotonNetwork.JoinRandomRoom();
    //}

    ////JoinRandomRoomが失敗したときに呼ばれる。
    //public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    //{
    //    Debug.Log("ルームの入室に失敗しました。");
    //    //TestRoomという名前の部屋を作成して、部屋に入る。
    //    PhotonNetwork.CreateRoom("TestRoom");
    //}

    ////部屋に入った時に呼ばれる。
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("ルームに入りました。");
    //    //Gameシーンをロード。
    //    PhotonNetwork.LoadLevel("Game");
    //}
}
