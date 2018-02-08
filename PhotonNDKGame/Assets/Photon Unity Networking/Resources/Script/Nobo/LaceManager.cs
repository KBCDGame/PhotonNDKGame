﻿using System.Collections;
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
    private PhotonView MyPV;
    [SerializeField]
    private GameObject LaceCarPrefab;                       //このレースで使用する車。
    [SerializeField]
    private Transform[] LaceStartPos = new Transform[4];    //レース開始位置。   
    [SerializeField]
    private Transform[] PodiumPos = new Transform[4];       //表彰体の位置。  
    [SerializeField]
    private Transform ReturnLobbyPos;                       //プレイヤーを戻す位置。
    [SerializeField]
    private Text CountDownTimeText;                         //カウントダウン用のテキスト。
    [SerializeField]
    private Text CarSpeedText;                              //速度表示用のテキスト。
    [SerializeField]
    private Text LaceTimeText;                              //レース中の時間。
    [SerializeField]
    private GameObject Anim;                                //カウントダウンアニメーション。
    [SerializeField]
    private GameObject LaceResultPanel;                     //リザルトを表示するパネル。
    [SerializeField]
    private Text[] LaceResultTextList = new Text[4];        //レースの順位、名前を出すテキスト。
    [SerializeField]
    private Text[] LaceResultTimeTextList = new Text[4];    //レースのTimeを出すテキスト(名前と一緒に表示するとTimeがずれるのでそれを防ぐためのテキスト)。
    [SerializeField]
    private GameObject LaceResultTransparentPanel;          //リザルトで表示する透明な赤いパネル。
    [SerializeField]
    private GameObject LaceMiniMap;                         //レース中に使うミニマップ。
    [SerializeField]
    private GameObject MainCamera;                          //メインカメラ。
    [SerializeField]
    private bool IsLaceFlag;                                //レース中のフラグ。
    [SerializeField]
    private int LacePlayStartNum;                           //開始人数。
    [SerializeField]
    private float CountDownTime;                            //カウントダウンの時間。  
    [SerializeField]
    private int[] LaceGoalID;                               //ゴールしたIDを格納していく。
    [SerializeField]
    private int GoalPlyerNum = 0;                           //ゴールした人数。
    [SerializeField]
    private List<LacePlayerInfo> LacePlayerList;            //レースに参加するプレイヤーリスト。
    [SerializeField]
    private GameObject UseLaceCar;                          //レースで実際に使った車。
    private enum LacePhase                                  //レースの段階。
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
        //カウントダウンテキスト、速度表示テキスト、レース時間テキスト、
        //リザルトパネル、ミニマップ、アニメーションのカウントダウンを非表示。
        CountDownTimeText.gameObject.SetActive(false);
        CarSpeedText.gameObject.SetActive(false);
        LaceTimeText.gameObject.SetActive(false);
        LaceResultPanel.SetActive(false);
        LaceMiniMap.SetActive(false);
        Anim.SetActive(false);
        MainCamera.GetComponent<ExampleClass>().enabled = false;

        //レースで使う変数の初期化。
        LaceGoalID = new int[LacePlayStartNum];
        IsLaceFlag = false;

        LaceUseVariableReset();

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
                if (Anim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    //車のハンドブレーキを降ろす。
                    UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();
                    //レース開始。
                    NowLacePhase = LacePhase.Game;

                    //カウントダウンテキストを非表示。
                    CountDownTimeText.gameObject.SetActive(false);
                    Anim.SetActive(false);

                    //レース時間テキストを表示。
                    LaceTimeText.gameObject.SetActive(true);
                }
                break;
            case LacePhase.Game:

                //プレイヤーが一人ゴールした。
                if (GoalPlyerNum == 1)
                {
                   
                    //カウントダウンテキストを表示。
                    for (int i = 0; i < LacePlayStartNum; i++)
                    {
                        if (LacePlayerList[i].ID == PhotonNetwork.player.ID)
                        {
                            CountDownTimeText.gameObject.SetActive(true);

                            //カウントダウン開始。
                            CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(CountDownTime, "00");

                            NowLacePhase = LacePhase.Goal;
                        }
                    }
                }
                break;
            case LacePhase.Goal:
                //全員ゴールが出来た。
                if (GoalPlyerNum== LacePlayStartNum)
                {
                    for (int i = 0; i < LacePlayStartNum; i++)
                    {
                        if (LaceGoalID[i] == UseLaceCar.GetComponent<PhotonView>().ownerId)
                        {
                            UseLaceCar.transform.position = PodiumPos[i].position;
                            //カウントダウン開始。
                            CountDownTimeText.gameObject.SetActive(false);
                            NowLacePhase = LacePhase.Result;
                        }
                    }

                    return;
                }

                //カウントダウンが終了した時点で全員がゴールしていなくてもレースを終わらせる。
                if (CountDownTimeText.GetComponent<CountDownTime>().GetCountDownEnd() == true)
                {
                    //カウントダウンを非表示。
                    CountDownTimeText.gameObject.SetActive(false);
                    for (int i = 0; i < LacePlayStartNum; i++)
                    {
                        //ゴールしている時の処理。
                        if (LaceGoalID[i] == UseLaceCar.GetComponent<PhotonView>().ownerId)
                        {
                            UseLaceCar.transform.position = PodiumPos[i].position;
                           
                        }
                        //ゴールしていない
                        else if (LaceGoalID[i] == -1)
                        {
                            //ゴールしていないプレイヤーのリザルトを設定。
                            MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, UseLaceCar.GetComponent<PhotonView>().ownerId,
                                "--:--:--", UseLaceCar.GetComponent<PhotonView>().owner.NickName,false);
                        }
                        NowLacePhase = LacePhase.Result;
                    }       
                }
                break;
            case LacePhase.Result:
                //レース時間テキスト非表示。
                LaceTimeText.gameObject.SetActive(false);
                //リザルトを表示。
                LaceResultPanel.SetActive(true);
                //ミニマップを非表示。
                LaceMiniMap.SetActive(false);
                break;
            case LacePhase.End:
                for (int i = 0; i < LacePlayStartNum; i++)
                {
                    //プレイヤーをロビーに移動。
                    LacePlayerList[i].Player.transform.position = ReturnLobbyPos.position;
                    //プレイヤー名をアクティブ化。
                    LacePlayerList[i].Player.GetComponent<PlayerManager>().Enable();
                    //プレイヤーをアクティブ化。
                    LacePlayerList[i].Player.SetActive(true);

                    if (LacePlayerList[i].ID == PhotonNetwork.player.ID)
                    {
                        //プレイヤーの親に車を設定。
                        LacePlayerList[i].Player.transform.parent = null;


                        //カウントダウンテキスト、速度表示テキスト、レース時間テキスト、
                        //リザルトパネル、ミニマップ、アニメーションのカウントダウンを非表示。
                        CountDownTimeText.gameObject.SetActive(false);
                        CarSpeedText.gameObject.SetActive(false);
                        LaceTimeText.gameObject.SetActive(false);
                        LaceResultPanel.SetActive(false);
                        LaceMiniMap.SetActive(false);
                        Anim.SetActive(false);

                        MainCamera.GetComponent<NoboCamera>().enabled = true;
                        MainCamera.GetComponent<ExampleClass>().enabled = false;
                        MainCamera.GetComponent<ExampleClass>().SetTarget(null);

                        PhotonNetwork.Destroy(UseLaceCar);
                        UseLaceCar = null;
                    }

                    NowLacePhase = LacePhase.None;
                }
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
        for (int i = 0; i < LacePlayerList.Count; i++)
        {
            if (LacePlayerList[i].ID == id)
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
        LacePlayerList.Add(info);

        //開始人数になったら準備開始。
        if (LacePlayStartNum == LacePlayerList.Count)
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
            LacePlayerList[i].Player.transform.position = Vector3.zero;
            //プレイヤー名を非アクティブ化。
            LacePlayerList[i].Player.GetComponent<PlayerManager>().AnEnable();
            //プレイヤーを非アクティブ化。
            LacePlayerList[i].Player.SetActive(false);

            if (LacePlayerList[i].ID == PhotonNetwork.player.ID)
            {
                //自キャラでのみ車を作成。
                UseLaceCar = PhotonNetwork.Instantiate(this.LaceCarPrefab.name,
                LaceStartPos[i].position,
                LaceStartPos[i].rotation,
                0);

                //車のハンドブレーキを引く。
                UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();
                //プレイヤーの親に車を設定。
                LacePlayerList[i].Player.transform.parent = UseLaceCar.transform;

                //kmテキストに速度表示テキストを設定。
                UseLaceCar.GetComponent<Km>().SetCarSpeedText(CarSpeedText);

                //速度テキストを表示。
                CarSpeedText.gameObject.SetActive(true);

                //ミニマップの表示。
                LaceMiniMap.SetActive(true);

                //ミニマップのカメラのターゲットを設定。
                LaceMiniMap.GetComponent<MiniMap>().ChangeTarget(UseLaceCar.transform);

                //カウントダウン開始。
                CountDownTimeText.gameObject.SetActive(true);
                CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(3.0f, "00");
                Anim.SetActive(true);
                Anim.GetComponent<Animator>().Play("countDown");

                SoundManager.Instance.PlaySE(1);
                MainCamera.GetComponent<NoboCamera>().enabled = false;
                MainCamera.GetComponent<ExampleClass>().enabled = true;
                MainCamera.GetComponent<ExampleClass>().SetTarget(UseLaceCar.GetComponent<SimpleCarController>().GetLaceCameraTrans());
                //プレイヤーを開始位置に移動。
                LacePlayerList[i].Player.transform.position = LaceStartPos[i].position;
            }
        }
        //レース段階を開始に進める。
        NowLacePhase = LacePhase.Start;
    }

    //ゴールに着いた順番にPhotonIDと名前とTimeをリストに追加。
    public void  AddLaceResult(int id,string name,bool isGoal)
    {
        MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, id, LaceTimeText.text,name, isGoal);
    }

    public  void LaceUseVariableReset()
    {
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            LaceGoalID[i]= -1;
        }

        CountDownTimeText.gameObject.SetActive(false);
        CarSpeedText.gameObject.SetActive(false);
        LaceTimeText.gameObject.SetActive(false);
        LaceResultPanel.SetActive(false);
        LaceMiniMap.SetActive(false);
        Anim.SetActive(false);
    }

    //ゴールをした車のID、レースの時間、プレイヤーの名前、trueなら正常にゴールfalseなら時間切れのゴール。
    [PunRPC]
    private void RPCAddLaceResult(int id,string time,string name,bool isGoal)
    {
        //同じIDははじく。
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            if (LaceGoalID[i] == id)
            {
                return;
            }
        }
        //レース中以外のプレイヤーには処理をさせない。
        if (IsLaceFlag == true)
        {
            //ゴールしたIDを追加。
            LaceGoalID[GoalPlyerNum] = id;

            //ゴールした時間を設定。
            LaceResultTimeTextList[GoalPlyerNum].text = time;
            //正常にゴールした。
            if (isGoal == true)
            {
                //順位、名前をテキストに設定。
                LaceResultTextList[GoalPlyerNum].text = (GoalPlyerNum + 1).ToString() + "        " + name;
            
            }
            else
            {
                //正常にゴール出来なかったので順位無し。
                LaceResultTextList[GoalPlyerNum].text = "--" + "        " + name;
            }

            //Gaolした人数を更新。
            GoalPlyerNum++;
        }
    }

    public int GetLacePlayStartNum()
    {
        return LacePlayStartNum;
    }

    public void LacePhaseEnd()
    {
        NowLacePhase = LacePhase.End;
    }
}