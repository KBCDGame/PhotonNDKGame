using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gameの予約を受け付け人数が揃ったらシーンを切り替えるキャラクター。
public class GameReservationPerson : Photon.MonoBehaviour {
    [SerializeField]
    private GameObject ReservationPanel;  //予約ウィンドウ。
    [SerializeField]
    private int MaxReservationNum = 0;    //最大予約人数。
    [SerializeField]
    private bool CanPlayFlag = true;     //コースを遊べるかのフラグ。  
    [SerializeField]
    private Rigidbody Rb;                 //Rigidbodyコンポーネント。
    [SerializeField]
    private PhotonTransformView MyPTV;    //PhotonTransformViewコンポーネント。
    [SerializeField]
    private PhotonView MyPV;              //PhotonViewコンポーネント。     
    [SerializeField]
    private List<int> ReservationPlayerList;//予約したプレイヤーのIDリスト。
    //[SerializeField]
    //private Transform StartTransform;
    //[SerializeField]
    //private GameObject LaceCar;            //このレースで使用する車。  
    [SerializeField]
    private GameObject LaceManager;         //このオブジェクトが担当するレースのマネージャー。

    private enum LaceCourse
    {
        Course1,
        Course2,
        Course3,
        Course4,
        Course5
    }
    

    // Use this for initialization
    void Start () {
        ReservationPanel.SetActive(false);
        if (MaxReservationNum <= 0)
        {
            Debug.Log("最大予約人数が0以下だったので1を設定します。");
            MaxReservationNum = 1;
        }

        ReservationPlayerList = new List<int>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = Rb.velocity;
        MyPTV.SetSynchronizedValues(velocity, 0.0f);

        if (CanPlayFlag == false)
        {
            return;
        }

        //現在の予約人数が最大予約人数と同じになった。
        if (MaxReservationNum == ReservationPlayerList.Count)
        {
            Debug.Log("人が集まりました。");
            LaceManagerSetId();
            CanPlayFlag = false;
        }
	}

    //予約を行った人をリストに追加。
    public void AddList(int id)
    {

        if (CanPlayFlag == false)
        {
            return;
        }

        //予約リストに同じIDがないかチェック。
        for (int i = 0; i < ReservationPlayerList.Count; i++)
        {
            if (ReservationPlayerList[i] == id)
            {
                return;
            }
        }

        //追加処理。
        MyPV.RPC("RPCAddList", PhotonTargets.AllBuffered, id);

        //Vector3 StartPos = new Vector3(
        //   GetStartTransform().position.x,
        //   GetStartTransform().position.y,
        //   GetStartTransform().position.z);

        //Quaternion StartRotation = GetStartTransform().transform.rotation;

        ////Photonに接続していれば自プレイヤーを生成。
        ////この関数で生成したオブジェクトは生成したプレイヤーがルームから消えると一緒に消される。
        //GameObject Car = PhotonNetwork.Instantiate(this.LaceCar.name,
        //   StartPos,
        //    StartRotation, 0);

        //player.transform.parent = Car.transform;
        //player.transform.position = Car.transform.position;
        ////プレイヤーを消す。
        //player.gameObject.SetActive(false);
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

    //private Transform GetStartTransform()
    //{
    //    return StartTransform;
    //}

    //追加の処理を全プレイヤーに通知。
    [PunRPC]
    public void RPCAddList(int id)
    {
        ReservationPlayerList.Add(id);
    }

    //レースマネージャーにレースをするプレイヤーのIDを設定。
    private void LaceManagerSetId()
    {
        LaceManager LM = LaceManager.GetComponent<LaceManager>();
        for (int i = 0; i < ReservationPlayerList.Count; i++)
        {
            LM.AddLacePlyerIdList(ReservationPlayerList[i]);
        }
    }
}
