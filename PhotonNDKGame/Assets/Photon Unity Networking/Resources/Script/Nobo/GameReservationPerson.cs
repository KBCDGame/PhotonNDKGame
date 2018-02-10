using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Gameの予約を受け付け人数が揃ったらシーンを切り替えるキャラクター。
public class GameReservationPerson : Photon.MonoBehaviour {
    [SerializeField]
    private Rigidbody Rb;                 //Rigidbodyコンポーネント。
    [SerializeField]
    private PhotonTransformView MyPTV;    //PhotonTransformViewコンポーネント。
    [SerializeField]
    private PhotonView MyPV;              //PhotonViewコンポーネント。     
    [SerializeField]
    private GameObject LaceManager;       //このオブジェクトが担当するレースのマネージャー。
    [SerializeField]
    private TextMesh LaceStartNumText;    //レース開始までの人数を表示。

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update () {
        Vector3 velocity = Rb.velocity;
        MyPTV.SetSynchronizedValues(velocity, 0.0f);


        //あと何人予約出来るのか計算。
        //予約出来る人数=レース開始人数ー現在の予約人数。
        int num;
        num = LaceManager.GetComponent<LaceManager>().GetLacePlayStartNum() - LaceManager.GetComponent<LaceManager>().GetLacePlayerList().Count;
        LaceStartNumText.text = "レースに必要な人数：あと"+num.ToString("0") + "人";
    }

    //予約を行った人をリストに追加。
    public void AddList()
    {
        //追加処理。
        int id = PhotonNetwork.player.ID;
        if (LaceManager.GetComponent<LaceManager>().AddCheckLacePlayerList(id))
        {
            Debug.Log("追加出来た。");
        }
        else
        {
            Debug.Log("すでに追加されている");
        }
    }

    //予約を行った人がキャンセルもしくは通信が切れたらリストから削除。
    public void SubList(int id)
    {

    }
}
