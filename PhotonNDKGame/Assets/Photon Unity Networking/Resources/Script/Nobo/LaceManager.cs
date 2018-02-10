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

[System.Serializable]
public class LaceGoalPlayerInfo
{
    public int ID;
    public bool IsGoal;

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
    private LaceGoalPlayerInfo[] LaceGoalPlayerInfoList = new LaceGoalPlayerInfo[4];    //ゴールしたプレイヤーリスト。
    [SerializeField]
    private int GoalPlyerNum = 0;                           //ゴールした人数。
    [SerializeField]
    private List<LacePlayerInfo> LacePlayerList;            //レースに参加するプレイヤーリスト。
    [SerializeField]
    private GameObject UseLaceCar;                          //レースで実際に使った車。
    [SerializeField]
    private Button BackLobbyButton;                         //リザルトからlobbyに戻るボタン。
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateReservationPlayerNameText();
        
        switch (NowLacePhase)
        {
            case LacePhase.None:
                if (GoalPlyerNum < 0)
                {
                    LaceUseVariableReset();
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
                if (GoalPlyerNum == LacePlayStartNum)
                {
                    for (int i = 0; i < LacePlayStartNum; i++)
                    {
                        if (LaceGoalPlayerInfoList[i].ID == UseLaceCar.GetComponent<PhotonView>().ownerId)
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
                        if (LaceGoalPlayerInfoList[i].ID == UseLaceCar.GetComponent<PhotonView>().ownerId && LaceGoalPlayerInfoList[i].IsGoal == true)
                        {
                            UseLaceCar.transform.position = PodiumPos[i].position;
                        }
                        //ゴールしていない
                        else if (LaceGoalPlayerInfoList[i].ID == UseLaceCar.GetComponent<PhotonView>().ownerId && LaceGoalPlayerInfoList[i].IsGoal == false)
                        {
                            //ゴールしていないプレイヤーのリザルト情報を設定。
                            MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, UseLaceCar.GetComponent<PhotonView>().ownerId,
                                "--:--:--", UseLaceCar.GetComponent<PhotonView>().owner.NickName, false);
                            UseLaceCar.GetComponent<SimpleCarController>().ChangeRunFlag();
                        }
                        //他のIDの時は無視。
                        else
                        {
                            return;
                        }

                        //リザルトで表示する赤い帯の位置を設定。
                        LaceResultTransparentPanel.GetComponent<RectTransform>().transform.position =
                            new Vector3(400.0f,
                            LaceResultTransparentPanel.GetComponent<RectTransform>().transform.position.y + 50.0f * i,
                            0.0f);
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
                //lobbyに戻るボタンを表示。
                BackLobbyButton.gameObject.SetActive(true);
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
                        LaceUseVariableReset();

                        MainCamera.GetComponent<NoboCamera>().enabled = true;
                        MainCamera.GetComponent<ExampleClass>().enabled = false;
                        MainCamera.GetComponent<ExampleClass>().SetTarget(null);
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
        //ゲーム上のプレイヤー取得。
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        LacePlayerInfo info = new LacePlayerInfo();
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
    public void AddLaceResult(int id, string name, bool isGoal)
    {
        MyPV.RPC("RPCAddLaceResult", PhotonTargets.All, id, LaceTimeText.text, name, isGoal);
    }

    //各数値の初期化。
    public void LaceUseVariableReset()
    {
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            LaceGoalPlayerInfoList[i].ID = -1;
            LaceGoalPlayerInfoList[i].IsGoal = false;
        }

        if (LacePlayerList.Count > 0)
        {
            LacePlayerList.Clear();
        }

        SetActiveCollection(false);

        if (UseLaceCar != null)
        {
            PhotonNetwork.Destroy(UseLaceCar);
            UseLaceCar = null;
        }

        IsLaceFlag = false;

        NowLacePhase = LacePhase.None;

        LaceTimeText.GetComponent<GameTime>().ResetGameTime();
    }

    //ゴールをした車のID、レースの時間、プレイヤーの名前、trueなら正常にゴールfalseなら時間切れのゴール。
    [PunRPC]
    private void RPCAddLaceResult(int id, string time, string name, bool isGoal)
    {
        //同じIDははじく。
        for (int i = 0; i < LacePlayStartNum; i++)
        {
            if (LaceGoalPlayerInfoList[i].ID == id)
            {
                return;
            }
        }
        //レース中以外のプレイヤーには処理をさせない。
        if (IsLaceFlag == true)
        {
            //ゴールしたIDを追加。
            LaceGoalPlayerInfoList[GoalPlyerNum].ID = id;

            //正常にゴールしたかどうかのフラグを設定。
            LaceGoalPlayerInfoList[GoalPlyerNum].IsGoal = isGoal;

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

            //Goalした人数を更新。
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
    }

    [PunRPC]
    private void RPCBackLobby()
    {
        GoalPlyerNum--;
    }

    private void SetActiveCollection(bool flag)
    {
        CountDownTimeText.gameObject.SetActive(flag);
        CarSpeedText.gameObject.SetActive(flag);
        LaceTimeText.gameObject.SetActive(flag);
        LaceResultPanel.SetActive(flag);
        LaceMiniMap.SetActive(flag);
        Anim.SetActive(flag);
    }

    //リスト取得。
    public List<LacePlayerInfo> GetLacePlayerList()
    {
        return LacePlayerList;
    }

    //追加されるIDがすでに登録されていないかチェック。
    public bool AddCheckLacePlayerList(int id)
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
        MyPV.RPC("RPCAddLacePlyerIdList", PhotonTargets.AllBuffered, id);
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

    public void LacePhaseShiftToReady()
    {
        MyPV.RPC("RPCLacePhaseShiftToReady", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    private void RPCLacePhaseShiftToReady()
    {

        
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
                ReservationPlayerNameText.text = ReservationPlayerNameText.text + LacePlayerList[i].Player.GetComponent<PhotonView>().owner.NickName + "　";
            }
        }   
    }
}