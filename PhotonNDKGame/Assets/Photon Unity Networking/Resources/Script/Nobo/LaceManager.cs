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
    private Text CarSpeedText;                          //速度表示用のテキスト。
    [SerializeField]
    private Text LaceTimeText;                          //レース中の時間。
    [SerializeField]
    private GameObject MiniMap;                         //ミニマップ。
    [SerializeField]
    private bool IsLaceFlag;                            //レース中のフラグ。
    [SerializeField]
    private int LacePlayStartNum;                       //開始人数。
    [SerializeField]
    private float CountDownTime;                        //カウントダウンの時間。  
    [SerializeField]
    private int[] LaceRanking;                         //順位リスト。
    [SerializeField]
    private int GoalPlyerNum = 0;                       //ゴールした人数。

    [SerializeField]
    private List<LacePlayerInfo> PlayerList;
    [SerializeField]
    private GameObject UseLaceCar;                      //レースで実際に使った車。
    private enum LacePhase                              //レースの段階。
    {
        None,               //なにもしない時。
        Ready,              //準備。                            
        Start,              //開始
        Game,               //レース中。
        Goal,               //ゴール。
        Result,             //結果発表。
        End                 //レース終わり。
    }

    [SerializeField]
    private LacePhase NowLacePhase;                     //現在のレースの段階。                       
    // Use this for initialization
    void Start()
    {
        //カウントダウンテキストと速度表示テキストとレース時間テキストを非表示。
        CountDownTimeText.gameObject.SetActive(false);
        CarSpeedText.gameObject.SetActive(false);
        LaceTimeText.gameObject.SetActive(false);

        //レース順位の初期化。
        LaceRanking = new int[LacePlayStartNum];
        IsLaceFlag = false;
        MiniMap.SetActive(false);

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
                //カウントダウンが終わったら。
                if (CountDownTimeText.GetComponent<CountDownTime>().CountDown()==true)
                {
                    //車のハンドブレーキを降ろす。
                    UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();
                    //レース開始。
                    NowLacePhase = LacePhase.Game;

                    //カウントダウンテキストを非表示。
                    CountDownTimeText.gameObject.SetActive(false);

                    //レース時間テキストを表示。
                    LaceTimeText.gameObject.SetActive(true);
                }
                break;
            case LacePhase.Game:

                //プレイヤーが一人ゴールした。
                if (GoalPlyerNum == 1)
                {
                    //レース時間テキスト非表示。
                    LaceTimeText.gameObject.SetActive(false);
                    //カウントダウンテキストを表示。
                    for (int i = 0; i < LacePlayStartNum; i++)
                    {
                        if (PlayerList[i].ID == PhotonNetwork.player.ID)
                        {
                            CountDownTimeText.gameObject.SetActive(true);

                            //カウントダウン開始。
                            CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(10.0f, "00");

                            NowLacePhase = LacePhase.Goal;
                        }
                    }
                }
                break;
            case LacePhase.Goal:
                break;
            case LacePhase.Result:
                Debug.Log(NowLacePhase);
                break;
            case LacePhase.End:
                Debug.Log(NowLacePhase);
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
            IsLaceFlag = true;
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
            //プレイヤー名を非アクティブ化。
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

                //車のハンドブレーキを引く。
                UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();       
                //プレイヤーの親に車を設定。
                PlayerList[i].Player.transform.parent = UseLaceCar.transform;

                //kmテキストに速度表示テキストを設定。
                UseLaceCar.GetComponent<Km>().SetCarSpeedText(CarSpeedText);

                //速度テキストを表示。
                CarSpeedText.gameObject.SetActive(true);

                //カウントダウンテキストを表示
                CountDownTimeText.gameObject.SetActive(true);

                //ミニマップの表示。
                MiniMap.SetActive(true);

                //ミニマップのカメラのターゲットを設定。
                MiniMap.GetComponent<MiniMap>().ChangeTarget(UseLaceCar.transform);

                //カウントダウン開始。
                CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(3.0f,"0");
            }
           
            //プレイヤーを開始位置に移動。
            PlayerList[i].Player.transform.position = StartPos[i].position;
        }

        //レース段階を開始に進める。
        NowLacePhase = LacePhase.Start;
    }

    //ゴールに着いた順番にPhotonIDをリストに加算。
    public void  AddLacePriority(int rank)
    {
        MyPV.RPC("RPCAddLacePriority", PhotonTargets.All, rank);
    }

    public  void LaceUseVariableReset()
    {
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            LaceRanking[i] = -1;
        }
    }

    [PunRPC]
    private void RPCAddLacePriority(int rank)
    {
        //レース中以外のプレイヤーには処理をさせない。
        if (IsLaceFlag == true)
        {
            LaceRanking[GoalPlyerNum] = rank;
            GoalPlyerNum++;
        }
    }

    public int GetLacePlayStartNum()
    {
        return LacePlayStartNum;
    }
}