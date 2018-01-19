using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerInfo
{
    public int PlayerID;
    public string PlayerName;
    public GameObject Player;
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
    private PhotonTransformView MyPTV;    //PhotonTransformViewコンポーネント。
    [SerializeField]
    private PhotonView MyPV;              //PhotonViewコンポーネント。     
    [SerializeField]
    private　int[] ReservationPlayerList;//予約出来るプレイヤーのリスト。
    [SerializeField]
    private List<PlayerInfo> PlayerList;//予約出来るプレイヤーのリスト。
    [SerializeField]
    private Transform StartTransform;
    [SerializeField]
    private GameObject LaceCar;            //このレースで使用する車。
    

    // Use this for initialization
    void Start () {
        ReservationPanel.SetActive(false);
        if (MaxReservationNum <= 0)
        {
            Debug.Log("最大予約人数が0以下だったので1を設定します。");
            MaxReservationNum = 1;
        }

        //配列の初期化。
        ReservationPlayerList = new int[MaxReservationNum];
        for (int i = 0; i < ReservationPlayerList.Length;i++)
        {
            ReservationPlayerList[i] = -1;
        }
    }
	
	// Update is called once per frame
	void Update () {
      
        
        //予約人数が最大予約人数と同じになった。
        if (MaxReservationNum== NowReservationNum)
        {
            Debug.Log("人が集まりました。");
        }

        Vector3 velocity = Rb.velocity;
        MyPTV.SetSynchronizedValues(velocity, 0.0f);

	}

    //予約を行った人をリストに追加。
    public void  AddList(int id,string name,GameObject player)
    {
        //foreach (int PlayerID in ReservationPlayerList)
        //{
        //    if (PlayerID == id)
        //    {
        //        return;
        //    }
        //}

        foreach (PlayerInfo list in PlayerList)
        {
            if (list.PlayerID == id)
            {
                return;
            }
        }

        PlayerInfo info = new PlayerInfo();
        info.PlayerID = id;
        info.PlayerName = name;
        info.Player = player;

        PlayerList.Add(info);

        //MyPV.RPC(" RPCAddList", PhotonTargets.All, id);

        Vector3 StartPos = new Vector3(
           GetStartTransform().position.x,
           GetStartTransform().position.y,
           GetStartTransform().position.z);

        Quaternion StartRotation = GetStartTransform().transform.rotation;

        //Photonに接続していれば自プレイヤーを生成。
        //この関数で生成したオブジェクトは生成したプレイヤーがルームから消えると一緒に消される。
        GameObject Car = PhotonNetwork.Instantiate(this.LaceCar.name,
           StartPos,
            StartRotation, 0);

        player.GetComponent<MeshRenderer>().enabled = false;
        player.transform.parent = Car.transform;
        player.transform.position = Car.transform.position;
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

    private Transform GetStartTransform()
    {
        return StartTransform;
    }

    [PunRPC]
    private void RPCAddList(int id)
    {
        for (int i = 0; i < ReservationPlayerList.Length; i++)
        {
            if (ReservationPlayerList[i] == id)
            {
                ReservationPlayerList[i] = id;
                //人数加算。
                NowReservationNum++;
            }
        }
    }
}
