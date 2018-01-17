using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public int PlayerID;       //プレイヤーのID。
    public string PlayerName;  //プレイヤーの名前。
    public GameObject Player;  //プレイヤーオブジェクト。
}

//Gameの予約を受け付け人数が揃ったらシーンを切り替えるキャラクター。
public class GameReservationPerson : Photon.MonoBehaviour {
    [SerializeField]
    private GameObject ReservationPanel;  //予約ウィンドウ。
    [SerializeField]
    private int MaxReservationNum = 0;    //最大予約人数。
    [SerializeField]
    private int NowReservationNum = 0;    //現在予約されている人数。
    [SerializeField]
    private int GameStartNum;             //ゲームを始められる人数。
    [SerializeField]
    private string PlayGameName = "";     //このキャラが受け付けるコースを設定。
    [SerializeField]
    private bool CanPlayFlag;             //コースを遊べるかのフラグ。  
    [SerializeField]
    private Rigidbody Rb;                 //Rigidbodyコンポーネント。
    [SerializeField]
    private PhotonTransformView MyPTV;
    [SerializeField]
    private　List<PlayerInfo> ReservationPlayerList;//予約したプレイヤーのリスト。
    

    // Use this for initialization
    void Start () {
        ReservationPanel.SetActive(false);
        if (MaxReservationNum <= 0)
        {
            Debug.Log("最大予約人数が0以下だったので1を設定します。");
            MaxReservationNum = 1;
        }

        ReservationPlayerList = new List<PlayerInfo>();
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

        Vector3 velocity = Rb.velocity;
        MyPTV.SetSynchronizedValues(velocity, 0.0f);

	}

    ////変数の同期。
    void OnPhotonSerializeView(PhotonStream stream, NetworkMessageInfo info)
    {
        Debug.Log("OnPhotonSerializeView()");
        //データの送信。
        if (stream.isWriting)
        {
            stream.SendNext(NowReservationNum);
            stream.SendNext(ReservationPlayerList);
        }
        //データの受信。
        else
        {
            NowReservationNum = (int)stream.ReceiveNext();
            ReservationPlayerList = (List<PlayerInfo>)stream.ReceiveNext();
        }
    }

    //予約を行った人をリストに追加。
    public bool AddList(int id,string name,GameObject player)
    {
        foreach (PlayerInfo list in ReservationPlayerList)
        {
            //すでに同じIDがあるなら追加を行わない。
            if (list.PlayerID == id)
            {
                return false;
            }
        }

        //情報設定。
        PlayerInfo info = new PlayerInfo();
        info.PlayerID = id;
        info.PlayerName = name;
        info.Player = player;

        //リストに追加。
        ReservationPlayerList.Add(info);
        //人数加算。
        NowReservationNum++;

        return true;
    }

    //予約を行った人がキャンセルもしくは通信が切れたらリストから削除。
    public void SubList(int id)
    {
       
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
