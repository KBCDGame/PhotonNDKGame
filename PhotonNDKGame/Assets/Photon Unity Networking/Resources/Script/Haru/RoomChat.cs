﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RoomChat : Photon.MonoBehaviour {
    #region 変数宣言

    GameObject[] players;   //全てのプレイヤーキャラ取得用a
    GameObject sender;      //送信キャラ取得用
    GameObject myPlayer;    //自分のキャラ取得用
    GameObject player;

    GUIStyle ChatStyle = new GUIStyle();    //範囲チャットStyle
    GUIStyleState ChatStyleState = new GUIStyleState();
    GUIStyle AllChatStyle = new GUIStyle(); //全体チャットStyle
    GUIStyleState AllChatStyleState = new GUIStyleState();

    public Rect GuiRect = new Rect(0, 0, 300, 200);
    public bool isVisible = true;
    public bool AlignBottom = true;
    public List<string> messages = new List<string>();
    public List<bool> chatKind = new List<bool>(); //チャットログの種類格納用(範囲チャor全チャ)
    public string inputLine = "";//入力文章格納用string
    private Vector2 scrollPos = Vector2.zero;//スクロールバー位置
    #endregion

    #region Start関数　Upbeat関数
    public void Start()
    {
        //myPlayerオブジェクト取得(範囲チャット発言時にpositionとmyPM使う)
        GetmyPlayer();

        ////範囲チャットの場合は白文字にし、文字がＵＩからあふれた場合は折り返す設定
        //ChatStyleState.textColor = Color.white;
        //ChatStyle.normal = ChatStyleState;
        //ChatStyle.wordWrap = true;
        //全体チャットの場合は赤文字にし、文字がＵＩからあふれた場合は折り返す設定
        AllChatStyleState.textColor = Color.white;
        AllChatStyle.normal = AllChatStyleState;
        AllChatStyle.wordWrap = true;
    }

    public void Update()
    {
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
            //ChatUIの大きさ調整
            GuiRect.width = Screen.width / 3;
            GuiRect.height = Screen.height / 3;
        }
    }
    #endregion

    #region OnGUI関数
    public void OnGUI()
    {
        if (!this.isVisible || !PhotonNetwork.inRoom)//表示フラグがOFF
        {
            //UI非表示
            return;
        }
        //ChatUIの作成開始
        GUILayout.Window(0, GuiRect, ChatUIWindow, "");
        //Enterを押すと
        if (Event.current.type == EventType.keyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            GUI.FocusControl("ChatInput");
        }

    }

    void ChatUIWindow(int windowID)
    {
        //FocusがチャットUIに乗ってるときにEnterを押すとチャット発言が実行される
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))  //チャット入力欄がNullやEmptyでない場合
            {
                //範囲チャット送信関数実行
                SendChat(true);

                return;
            }
        }
        //垂直のコントロールグループ開始
        GUILayout.BeginVertical();
        //スクロールビュー開始位置
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        //チャットログ表示用フレキシブルスペース生成
        GUILayout.FlexibleSpace();

        //フレキシブルスペースにチャットログを表示
        for (int i = 0; i <= messages.Count - 1; i++)
        {
            if (chatKind[i] != true)    //範囲チャットであれば
            {
                GUILayout.Label(messages[i], ChatStyle);
            }
            else                        //全チャットであれば
            {
                GUILayout.Label(messages[i], AllChatStyle);
            }
        }

        //スクロールビュー終了
        GUILayout.EndScrollView();

        //水平のコントロールグループ開始
        GUILayout.BeginHorizontal();
        //入力テキストフィールド生成、Focusが乗った状態をChatInputと命名
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine, 200);

        //Sendボタンを生成かつ押したときに送信
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            //全体チャット送信関数実行
            SendChat(true);
        }

        //水平のコントロールグループ終了
        GUILayout.EndHorizontal();
        //垂直のコントロールグループ終了
        GUILayout.EndVertical();
    }
    #endregion

    #region Getmyplayer 自キャラのオブジェクトをmyPlayerに登録
    void GetmyPlayer()
    {
        //自キャラのID取得
        int myPlayerID = PhotonNetwork.player.ID;

        //全てのプレイヤーオブジェクトを取得
        players = GameObject.FindGameObjectsWithTag("Player");
        //全てのプレイヤーオブジェクトから自キャラをIDで検索し、取り出す
        foreach (GameObject player in players)
        {
            int playerLoopId = player.GetComponent<PhotonView>().owner.ID;
            if (playerLoopId == myPlayerID)
            {
                //自プレイヤーオブジェクトを取得
                myPlayer = player;
            }
        }
        return;
    }
    #endregion

    #region チャット送信関数
    void SendChat(bool isAll)
    {
        //chatRPC
        this.photonView.RPC("Chat", PhotonTargets.All, player, this.inputLine, isAll);

        //送信後、入力欄を空にし、スクロール最下位置に移動
        this.inputLine = "";
        scrollPos.y = Mathf.Infinity;
    }
    #endregion
    [PunRPC]
    public void Chat(Vector3 senderposition, string newLine, bool isAll, PhotonMessageInfo mi)
    {
        if (messages.Count >= 100)          //チャットログが多くなって来たらログを削除してから受信
        {
            messages.Clear();               //全てのチャットログを削除
            chatKind.Clear();               //全てのチャットの種類情報削除
        }


        if (isAll) //全チャとして受信
        {
            //chat受信
            ReceiveChat(newLine, isAll, mi);
        }
        //受信したときはスクロール最下位置
        scrollPos.y = Mathf.Infinity;
    }

    void ReceiveChat(string _newLine, bool isAll, PhotonMessageInfo _mi)
    {
        //送信者の名前用変数
        string senderName = "anonymous";
        if (_mi.sender != null)
        {
            //送信者の名前があれば
            if (!string.IsNullOrEmpty(_mi.sender.NickName))
            {
                senderName = _mi.sender.NickName;
            }
            else
            {
                senderName = "Player " + _mi.sender.ID;
            }
        }
        //受信したチャットをログに追加
        this.messages.Add(senderName + ": " + _newLine);
        this.chatKind.Add(isAll);

        return;
    }

}
