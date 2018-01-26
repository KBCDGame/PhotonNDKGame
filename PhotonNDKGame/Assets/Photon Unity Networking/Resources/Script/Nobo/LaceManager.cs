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
    private GameObject LaceCarPrefab;                   //このレースで使用する車。
    [SerializeField]
    private Transform[] StartPos =new Transform[4];     //レース開始位置。
    [SerializeField]
    private PhotonView MyPV;
    [SerializeField]
    private Text CountDownTimeText;                     //カウントダウン用のテキスト。
    [SerializeField]
    private Text LaceTimeText;                          //レース中の時間。
    [SerializeField]
    private bool IsStartFlag = false;                   //レースをスタートできるかどうかのフラグ。
    [SerializeField]
    private int LacePlayStartNum;                       //開始人数。
    [SerializeField]
    private float CountDownTime;                        //カウントダウンの時間。

    [SerializeField]
    private List<LacePlayerInfo> PlayerList;
    [SerializeField]
    private GameObject UseLaceCar;       //レースで実際に使った車。
    private enum LacePhase                   //レースの段階。
    {
        None,               //なにもしない時。
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
        //CountDownTimeText.GetComponent<CountDownTime>().SetCountTime(CountDownTime);
        //CountDownTimeText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (NowLacePhase)
        {
            case LacePhase.None:
                break;
            case LacePhase.Ready:
                LaceReady();
                break;
            case LacePhase.Start:
                //CountDownTimeText.gameObject.SetActive(true);
                //if (CountDownTimeText.GetComponent<CountDownTime>().GetCountDownEnd() == true)
                //{
                //    UseLaceCar.GetComponent<SimpleCarController>().RunFlagChangeToTrue();
                   
                //}
                Debug.Log(NowLacePhase);
                NowLacePhase = LacePhase.Game;
                break;
            case LacePhase.Game:
                break;
            case LacePhase.End:
                break;
            default:
                break;
        }
        
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
        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerList[i].ID == id)
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
        if (LacePlayStartNum == PlayerList.Count)
        {
            NowLacePhase = LacePhase.Ready;
        }
    }
    //指定された秒だけ待つ用のコルーチン。  
    private IEnumerator WaitSeceond(float second)
    {

        yield return new WaitForSeconds(second);
    }

    //レースの準備。
    private void LaceReady()
    {
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            //プレイヤーを見えない所に移動。
            PlayerList[i].Player.transform.position = Vector3.zero;
            //プレイヤーを非アクティブ化。
            PlayerList[i].Player.GetComponent<PlayerManager>().Enable();
            //プレイヤーを非アクティブ化。
            PlayerList[i].Player.SetActive(false);

            if (PlayerList[i].ID == PhotonNetwork.player.ID)
            {
                //自キャラでのみ車を作成。
                UseLaceCar = PhotonNetwork.Instantiate(this.LaceCarPrefab.name,
                StartPos[i].position,
                StartPos[i].rotation,
                0);

                UseLaceCar.GetComponent<SimpleCarController>().RunFlagChangeToFalse();       
                //プレイヤーの親に車を設定。
                PlayerList[i].Player.transform.parent = UseLaceCar.transform;
            }
           
            //プレイヤーを開始位置に移動。
            PlayerList[i].Player.transform.position = StartPos[i].position;
        }

        //レース段階を開始に進める。
        NowLacePhase = LacePhase.Start;
    }
}
