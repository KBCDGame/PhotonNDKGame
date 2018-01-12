using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gameの予約を受け付け人数が揃ったらシーンを切り替えるキャラクター。
public class GameReservationPerson : MonoBehaviour {
    [SerializeField]
    private GameObject ReservationPanel;  //予約ウィンドウ。
    [SerializeField]
    private int MaxReservationNum = 4;   //最大予約人数。
    [SerializeField]
    private int NowReservationNum = 0;   //現在予約されている人数。
    [SerializeField]
    private int GameStartNum;        //ゲームを始められる人数。
    [SerializeField]
    private string PlayGameName = "";//遊ぶゲームのシーン名。

    //予約を行ったプレイヤーの情報をまとめたもの。
    struct PlayerInfo
    {
       public int PlayerID;       //プレイヤーのID。
        public string PlayerName;  //プレイヤーの名前。

        //予約したプレイヤーの情報を設定。
        public PlayerInfo(int id, string name)
        {
            this.PlayerID = id;
            this.PlayerName = name;
        }
    }
    [SerializeField]
    private　List<PlayerInfo> ReservationPlayerList;//予約したプレイヤーのリスト。
    

    // Use this for initialization
    void Start () {
        ReservationPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
      
        //人数は揃っていないが遊べる最低人数は集まった。
        if (GameStartNum== NowReservationNum)
        {
            Debug.Log("人は集まっていないがゲームを始める。");
        }

        //予約人数が最大予約人数と同じになった。
        if (MaxReservationNum== NowReservationNum)
        {
            Debug.Log("人が集まりました。");
        }
	}

    //予約を行った人をリストに追加。
    public void AddList(int id,string name)
    {
        PlayerInfo info = new PlayerInfo(id,name);
        //リストに追加。
        ReservationPlayerList.Add(info);
        //人数加算。
        NowReservationNum++;
    }

    //予約を行った人がキャンセルもしくは通信が切れたらリストから削除。
    public void SubList(int id)
    {
        //昇順にソート。
        ReservationPlayerList.Sort();

        //削除する要素を検索。
        foreach (var list in ReservationPlayerList)
        {
            //一致。   
            if (list.PlayerID == id)
            {
                ReservationPlayerList.Remove(list);
            }
        }
        //人数減算。
        NowReservationNum--;
    }

    //予約ウィンドウを閉じる。
    public void CloseReservationPanel()
    {
        ReservationPanel.SetActive(false);
    }

    //予約ウィンドウを開く。
    public void OpenReservationPanel()
    {
        ReservationPanel.SetActive(true);
    }
}
