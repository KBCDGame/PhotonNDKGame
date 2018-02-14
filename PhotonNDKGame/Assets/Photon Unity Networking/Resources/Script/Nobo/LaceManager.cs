using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LacePlayerInfo
{
    public int ID;
    public int NetID;
    public bool isGoal;
}

//レースの管理。
public class LaceManager : Photon.MonoBehaviour
{
    [SerializeField]
    private GameObject GameManager;
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
    private TextMesh ReservationPlayerNameText = new TextMesh();    //予約しているプレイヤーの名前テキスト。
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
    private int GoalPlyerNum = 0;                           //ゴールした人数。
    [SerializeField]
    private List<LacePlayerInfo> LacePlayerList;            //レースに参加するプレイヤーリスト。
    [SerializeField]
    private List<int> Goal;
    [SerializeField]
    private GameObject UseLaceCar;                          //レースで実際に使った車。
    [SerializeField]
    private Button BackLobbyButton;                         //リザルトからlobbyに戻るボタン。
    [SerializeField]
    private List<int> PhotonIDList = new List<int>();
    //[SerializeField]
    //private GameObject MirrorCamera;
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
        SetActiveCollection(false);
        MainCamera.GetComponent<ExampleClass>().enabled = false;
        BackLobbyButton.gameObject.SetActive(false);
        IsLaceFlag = false;

        LaceUseVariableReset();

        if (PhotonNetwork.connected)
        {                         //Photonに接続できていなければ。
            LacePlayStartNum = PhotonNetwork.room.MaxPlayers;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReservationPlayerNameText();
        
        switch (NowLacePhase)
        {
            case LacePhase.None:
                if (GoalPlyerNum <= 0&&IsLaceFlag==true)
                {
                    //LaceUseVariableReset();
                    MyPV.RPC("LaceUseVariableReset", PhotonTargets.AllBuffered);
                }
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

                    //レースBGMを鳴らす
                    SoundManager.Instance.PlayBGM(3);

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
                    CountDownTimeText.gameObject.SetActive(true);

                    //カウントダウン開始。
                    CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(CountDownTime, "00");

                    NowLacePhase = LacePhase.Goal;
                }
                break;
            case LacePhase.Goal:
                //全員ゴールが出来た。
                if (GoalPlyerNum == LacePlayStartNum)
                {
                    for (int i = 0; i < LacePlayerList.Count; i++)
                    {
                        if (LacePlayerList[i].NetID == UseLaceCar.GetComponent<PhotonView>().ownerId)
                        {
                            UseLaceCar.transform.position = PodiumPos[i].position;
                            //カウントダウン開始。
                            CountDownTimeText.gameObject.SetActive(false);
                            NowLacePhase = LacePhase.Result;
                            SoundManager.Instance.PlaySE(2);
                        }
                    }
                    NowLacePhase = LacePhase.Result;
                    return;
                }

                //カウントダウンが終了した時点で全員がゴールしていなくてもレースを終わらせる。
                if (CountDownTimeText.GetComponent<CountDownTime>().GetCountDownEnd() == true)
                {
                    //カウントダウンを非表示。
                    CountDownTimeText.gameObject.SetActive(false);

                    //レースの順位分ループを回す。
                    for (int j = 0; j < LacePlayerList.Count;)
                    {
                        for (int i = 0; i < LacePlayerList.Count; i++)
                        {
                            if (LacePlayerList[i].NetID == PhotonNetwork.player.ID)
                            {
                                ////リザルトで表示する赤い帯の位置を設定。
                                //LaceResultTransparentPanel.GetComponent<RectTransform>().transform.position =
                                //    new Vector3(400.0f,
                                //    LaceResultTransparentPanel.GetComponent<RectTransform>().transform.position.y - 50.0f * j,
                                //    0.0f);

                                //ゴールしている時の処理。
                                if (LacePlayerList[i].isGoal == true)
                                {
                                    UseLaceCar.transform.position = PodiumPos[j].position;
                                }
                                //ゴールしていない
                                else
                                {
                                    //ゴールしていないプレイヤーのリザルト情報を設定。
                                    MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, UseLaceCar.GetComponent<PhotonView>().ownerId,
                                        "--:--:--", UseLaceCar.GetComponent<PhotonView>().owner.NickName, false);
                                    UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();
                                    NowLacePhase = LacePhase.Result;
                                    return;
                                }
                            }
                        }
                        j++;
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
                //lobbyに戻るボタンを表示。
                BackLobbyButton.gameObject.SetActive(true);
                break;
            case LacePhase.End:

                //レースに使うテキスト群を非表示。
                SetActiveCollection(false);

                //車を削除。
                if (UseLaceCar != null)
                {
                    PhotonNetwork.Destroy(UseLaceCar);
                    UseLaceCar = null;
                }
    
                //レース用のカメラから普通のカメラに戻す。
                MainCamera.GetComponent<NoboCamera>().enabled = true;
                MainCamera.GetComponent<ExampleClass>().enabled = false;
                MainCamera.GetComponent<ExampleClass>().SetTarget(null);

                //lobbyに戻るボタンを非表示。
                BackLobbyButton.gameObject.SetActive(false);

                //レースフェーズを初期化。
                NowLacePhase = LacePhase.None;
                
                //PhotonNetwork上にプレイヤーを生成し、メインカメラのターゲットを切り替え。
                MainCamera.GetComponent<NoboCamera>().ChangeTarget(GameManager.GetComponent<GameManager>().PhotonInstantiatePlayer(ReturnLobbyPos.position, Quaternion.identity, 0).transform);
                break;
            default:
                break;
        }

    }
   
    [PunRPC]
    public void RPCAddLacePlyerIdList(int id,int NetID)
    {
        LacePlayerInfo info = new LacePlayerInfo();
        info.ID = id;
        info.NetID = NetID;
        info.isGoal = false;
       
        LacePlayerList.Add(info);

        CheckLacePhaseShiftToReady();
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
            if (LacePlayerList[i].NetID ==PhotonNetwork.player.ID)
            {
                //自キャラでのみ車を作成。
                UseLaceCar = PhotonNetwork.Instantiate(this.LaceCarPrefab.name,
                LaceStartPos[i].position,
                LaceStartPos[i].rotation,
                0);

                //車のハンドブレーキを引く。
                UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();

                //kmテキストに速度表示テキストを設定。
                UseLaceCar.GetComponent<Km>().SetCarSpeedText(CarSpeedText);

                //速度テキストを表示。
                CarSpeedText.gameObject.SetActive(true);

                //ミニマップの表示。
                LaceMiniMap.SetActive(true);

                //ミニマップのカメラのターゲットを設定。
                LaceMiniMap.GetComponent<MiniMap>().ChangeTarget(UseLaceCar.transform);

                ////カウントダウン開始。
                //CountDownTimeText.gameObject.SetActive(true);
                CountDownTimeText.GetComponent<CountDownTime>().CountDownStart(3.0f, "00");
                Anim.SetActive(true);
                Anim.GetComponent<Animator>().Play("countDown");
                //直前のBGMを停止
                SoundManager.Instance.StopBGM();
                //カウントダウンのSE
                SoundManager.Instance.PlaySE(1);

                MainCamera.GetComponent<NoboCamera>().enabled = false;
                MainCamera.GetComponent<ExampleClass>().enabled = true;
                MainCamera.GetComponent<ExampleClass>().SetTarget(UseLaceCar.GetComponent<SimpleCarController>().GetLaceCameraTrans());

                PhotonView player = PhotonView.Find(LacePlayerList[i].ID);
                player.GetComponent<NoboCharacterController>().PhotonDestroy();
            }
        }
        //レース段階を開始に進める。
        NowLacePhase = LacePhase.Start;
       
    }

    //ゴールに着いた順番にPhotonIDと名前とTimeをリストに追加。
    public void AddLaceResult(int id, string name, bool isGoal)
    {
        MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, id, LaceTimeText.text, name, isGoal);
        //リザルト用BGM
        SoundManager.Instance.PlayBGM(4);
    }

    [PunRPC]
    //各数値の初期化。
    public void LaceUseVariableReset()
    {

        if (LacePlayerList.Count > 0)
        {
            LacePlayerList.Clear();
        }

        if (Goal.Count > 0)
        {
            Goal.Clear();
        }

        IsLaceFlag = false;

        NowLacePhase = LacePhase.None;

        LaceTimeText.GetComponent<GameTime>().ResetGameTime();
    }

    //ゴールをした車のID、レースの時間、プレイヤーの名前、trueなら正常にゴールfalseなら時間切れのゴール。
    [PunRPC]
    private void RPCAddLaceResult(int id, string time, string name, bool isGoal)
    {
        if (Goal.Count > 0)
        {
            for (int i = 0; i < Goal.Count; i++)
            {
                if (Goal[i] == id)
                {
                    return;
                }
            }
        }

        Goal.Add(id);
        for (int i = 0; i < LacePlayerList.Count; i++)
        {
            if (LacePlayerList[i].NetID == id)
            {
                if (isGoal == true)
                {
                    LacePlayerList[i].isGoal = isGoal;
                }
            }
        }

        //レース中以外のプレイヤーには処理をさせない。
        if (IsLaceFlag == true)
        {
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

            //人数更新。
            GoalPlyerNum++;
        }
    }

    public int GetLacePlayStartNum()
    {
        return LacePlayStartNum;
    }

    public void BackLobby()
    {
        MyPV.RPC("RPCBackLobby", PhotonTargets.AllBuffered);
        NowLacePhase = LacePhase.End;
        //ロビーBGM
        SoundManager.Instance.PlayBGM(2);
    }

    [PunRPC]
    private void RPCBackLobby()
    {
        if (GoalPlyerNum < 0)
        {
            return;
        }
        GoalPlyerNum--;
    }

    //レース関係で使うオブジェクトのSetActiveをまとめたもの。
    private void SetActiveCollection(bool flag)
    {
        CountDownTimeText.gameObject.SetActive(flag);
        CarSpeedText.gameObject.SetActive(flag);
        LaceTimeText.gameObject.SetActive(flag);
        LaceResultPanel.SetActive(flag);
        LaceMiniMap.SetActive(flag);
        Anim.SetActive(flag);
        //MirrorImage.gameObject.SetActive(flag);
    }

    //リスト取得。
    public List<LacePlayerInfo> GetLacePlayerList()
    {
        return LacePlayerList;
    }

    //追加されるIDがすでに登録されていないかチェック。
    public bool AddCheckLacePlayerList(int id,int NetID)
    {
        //リストに1人以上追加されていたら。
        if (LacePlayerList.Count > 0)
        {
            //同じIDははじく。
            for (int i = 0; i < LacePlayerList.Count; i++)
            {
                if (LacePlayerList[i].ID == id)
                {
                    //追加出来ない。
                    return false;
                }
            }
        }
        //追加。
        MyPV.RPC("RPCAddLacePlyerIdList", PhotonTargets.AllBuffered, id, NetID);
        return true;
    }

    //レースのフェーズをReadyに出来るかチェック。
    public void CheckLacePhaseShiftToReady()
    {
      
        //開始人数になったら準備開始。
        if (LacePlayStartNum == LacePlayerList.Count)
        {
            //レースの段階を準備に進める。
            NowLacePhase = LacePhase.Ready;

            //レース中フラグを立てる。
            IsLaceFlag = true;
        }
      
    }
    private void UpdateReservationPlayerNameText()
    {
        if (IsLaceFlag==true)
        {
            ReservationPlayerNameText.text ="レース中";
        }
        else
        {
            ReservationPlayerNameText.text = "参加プレイヤー：";
            //参加プレイヤー分名前を追加。
            for (int i = 0; i < LacePlayerList.Count; i++)
            {
                ReservationPlayerNameText.text = ReservationPlayerNameText.text + PhotonView.Find(LacePlayerList[i].ID).GetComponent<PhotonView>().owner.NickName/* LacePlayerList[i].Player.GetComponent<PhotonView>().owner.NickName*/ + "　";
            }
        }   
    }
}