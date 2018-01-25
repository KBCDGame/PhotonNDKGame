using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LacePlayerInfo
{
    public int ID;
    public GameObject Player; 
}

//レースの管理。
public class LaceManager : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject LaceCar;                         //このレースで使用する車。
    [SerializeField]
    private Transform[] StartPos = new Transform[4];    //レース開始位置。
    [SerializeField]
    private List<int> LacePlyerIdList;                  //レースに参加するプレイヤーのIDリスト。
    [SerializeField]
    private PhotonView MyPV;
    [SerializeField]
    private Text CountDownTimeText;           //カウントダウン用のテキスト。
    [SerializeField]
    private Text LaceTimeText;               //レース中の時間。
    [SerializeField]
    private bool IsStartFlag = false;        //レースをスタートできるかどうかのフラグ。
    [SerializeField]
    private int LacePlayStartNum;            //開始人数。

    [SerializeField]
    private List<LacePlayerInfo> PlayerList;

    private List<GameObject> LacePlayer;
    private enum LacePhase                   //レースの段階。
    {
        Ready,              //準備。                            
        Start,              //開始
        Game,               //レース中。
        End                 //レース終わり。
    }

    [SerializeField]
    private LacePhase NowLacePhase;           //現在のレースの段階。                       
    // Use this for initialization
    void Start()
    {
        //初期化。
        LacePlyerIdList = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (NowLacePhase)
        {
            case LacePhase.Ready:
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    PlayerList[i].Player.transform.position = Vector3.zero;
                }

                WaitSeceond(2.0f);

                SetStartPos();
                break;
            case LacePhase.Start:
                Debug.Log(NowLacePhase);
                break;
            case LacePhase.Game:
                break;
            case LacePhase.End:
                break;
            default:
                break;
        }
        
    }

    //レースをする予定のプレイヤーを開始位置に設定。
    private void SetStartPos()
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].Player.transform.position = StartPos[i].transform.position;
        }

        //レース段階を開始に進める。
        NowLacePhase = LacePhase.Start;
    }
    //予約したプレイヤーのIDをリストに追加。
    public void AddLacePlyerIdList(int id)
    {
        MyPV.RPC("RPCAddLacePlyerIdList", PhotonTargets.AllBuffered, id);
    }

    [PunRPC]
    public void RPCAddLacePlyerIdList(int id)
    {
        //同じIDははじく。
        for (int i = 0; i < LacePlyerIdList.Count; i++)
        {
            if (LacePlyerIdList[i] == id)
            {
                return;
            }
        }

        LacePlayerInfo info = new LacePlayerInfo();
        //ゲーム上のプレイヤー取得。
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //追加されたIDのプレイヤーのオブジェクト取得。
        foreach (GameObject plobj in players)
        {
            if (id == plobj.GetComponent<PhotonView>().ownerId)
            {
                //ID設定。
                info.ID = id;
                //オブジェクト設定。
                info.Player = plobj;
                break;
            }
        }

        //リストに追加。
        PlayerList.Add(info);

        //開始人数になったら準備開始。
        if (LacePlayStartNum == LacePlyerIdList.Count)
        {
            NowLacePhase = LacePhase.Ready;
        }
    }
    //指定された秒だけ待つ用のコルーチン。  
    private IEnumerator WaitSeceond(float second)
    {

        yield return new WaitForSeconds(second);
    }
}
