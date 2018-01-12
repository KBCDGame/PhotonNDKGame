using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoboCamera : MonoBehaviour {

    //[SerializeField]
    //private const float YAngle_Min = -89.0f;   //カメラのY方向の最小角度。
    //[SerializeField]
    //private const float YAngle_Max = 89.0f;     //カメラのY方向の最大角度。

    public Transform Target;    //追跡するオブジェクトのtransform。
    //public Vector3 Offset;      //追跡対象の中心位置調整用オフセット。
    //private Vector3 LookAt;     //targetとOffsetによる注視する座標。

    //[SerializeField]
    //private float Distance = 10.0f;    //キャラクターとカメラ間の角度。
    //[SerializeField]
    //private float Distance_Min = 1.0f;  //キャラクターとの最小距離。
    //[SerializeField]
    //private float Distance_Max = 20.0f; //キャラクターとの最大距離。
    //[SerializeField]
    //private float CurrentX = 0.0f;  //カメラをX方向に回転させる角度。
    //[SerializeField]
    //private float CurrentY = 0.0f;  //カメラをY方向に回転させる角度。

    ////カメラ回転用係数(値が大きいほど回転速度が上がる)。
    //[SerializeField]
    //private float MouseRotatoSpeedMoveX = 4.0f;     //マウスドラッグによるカメラX方向回転係数。
    //[SerializeField]
    //private float MouseRotatoSpeedMoveY = 2.0f;     //マウスドラッグによるカメラY方向回転係数。
    //private float RotatoSpeedMoveX_QE = 2.0f;  //QEキーによるカメラX方向回転係数。

    //void Start()
    //{

    //}

    //void Update()
    //{
    //    //QとEキーでカメラ回転。
    //    if (Input.GetKey(KeyCode.Q))
    //    {
    //        CurrentX += -RotatoSpeedMoveX_QE;
    //    }
    //    if (Input.GetKey(KeyCode.E))
    //    {
    //        CurrentX += RotatoSpeedMoveX_QE;
    //    }

    //    //マウス右クリックを押しているときだけマウスの移動量に応じてカメラが回転。
    //    if (Input.GetMouseButton(1))
    //    {
    //        CurrentX += Input.GetAxis("Mouse X") * MouseRotatoSpeedMoveX;
    //        CurrentY += Input.GetAxis("Mouse Y") * MouseRotatoSpeedMoveY;
    //        CurrentY = Mathf.Clamp(CurrentY, YAngle_Min, YAngle_Max);

    //    }
    //    Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel"), Distance_Min, Distance_Max);
    //}
    //void LateUpdate()
    //{
    //    if (Target != null)  //targetが指定されるまでのエラー回避。
    //    {
    //        LookAt = Target.position + Offset;  //注視座標はtarget位置+Offsetの座標。

    //        //カメラ旋回処理
    //        Vector3 dir = new Vector3(0, 0, -Distance);
    //        Quaternion rotation = Quaternion.Euler(-CurrentY, CurrentX, 0);

    //        transform.position = LookAt + rotation * dir;   //カメラの位置を変更。
    //        transform.LookAt(LookAt);   //カメラをLookAtの方向に向けさせる。
    //    }

    //}

    //プレイヤー。
    //private GameObject Player = null;
    //プレイヤーの位置。
    private Vector3 PlayerPos = Vector3.zero;
    //入力量保持用。
    private float InputX, InputY = 0.0f;
    //ホイールの入力量保持用。
    private float Scroll = 0.0f;
    //カメラが前後に移動したかどうかのフラグ。
    private bool IsMoveBeforeOrAfterFlag = false;

    ////カメラ。
    //private Camera GemaCamera = null;
    ////ズームに使う値保持用。
    //private float Scroll, View = 0.0f;
    [Header("回転速度。")]
    public float RotateSpeed = 200.0f;
    [Header("縦回転の上限。")]
    [Space(10), Tooltip("今は設定しても効果無し。")]
    public float MaxRotateY = 0.0f;
    [Header("縦回転の下限。")]
    [Space(10), Tooltip("今は設定しても効果無し。")]
    public float MinRotateY = 0.0f;
    [Header("プレイヤーからカメラまでの最大距離。")]
    public float PlayerToCameraMaxDistance = 80.0f;
    [Header("プレイヤーからカメラまでの最小距離。")]
    public float PlayerToCameraMinDistance = 3.0f;
    [Header("ゲームスタート時にプレイヤーからどれだけカメラをずらすか。")]
    public Vector3 StartOffsetPos = Vector3.zero;
    [Header("コントローラー使用時のカメラの前後移動のスピード。")]
    public float ControllerCameraMoveBeforeAndAfterSpeed = 2.0f;
    [Header("マウス使用時のカメラの前後移動のスピード。")]
    public float MouseCameraMoveBeforeAndAfterSpeed = 1000.0f;
    ////ズームの最大と最小。
    //public float ZoomMax,ZoomMin = 0.0f;
    ////ズームの時のスピード。
    //public float ZoomSpeed = 1.0f;


    // Use this for initialization
    void Start()
    {
        //最大と最小が反転して設定されていた場合は最大と最小を入れ替え。
        //大小比較するために絶対値に直す。
        PlayerToCameraMinDistance = Mathf.Abs(PlayerToCameraMinDistance);
        PlayerToCameraMaxDistance = Mathf.Abs(PlayerToCameraMaxDistance);
        if (PlayerToCameraMinDistance > PlayerToCameraMaxDistance)
        {
            float work = PlayerToCameraMinDistance;
            PlayerToCameraMinDistance = PlayerToCameraMaxDistance;
            PlayerToCameraMaxDistance = work;
            Debug.Log("プレイヤーとカメラ間の最小距離とプレイヤーとカメラ間の最大距離が逆でした。");

        }
        //プレイヤー取得。
        //Player = GameObject.FindWithTag("Player");
        ////カメラ取得。
        //GemaCamera = GetComponent<Camera>();

        //if (Player != null)
        //{
        //    //プレイヤーの位置取得。
        //    PlayerPos = Player.transform.position;
        //    //カメラの位置をプレイヤーからずらした位置に設定。
        //    Vector3 newPos = PlayerPos + StartOffsetPos;
        //    transform.position = newPos;
        //}

        if (Target != null)
        {
            //プレイヤーの位置取得。
            PlayerPos = Target.transform.position;
            //カメラの位置をプレイヤーからずらした位置に設定。
            Vector3 newPos = PlayerPos + StartOffsetPos;
            transform.position = newPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////プレイヤーを検索。
        //if (FindPlayer() == false)
        //{
        //    return;
        //}

        //Input関係の処理。
        InputRotate();
    }

    void FixedUpdate()
    {
        ////プレイヤーを検索。
        //if (FindPlayer() == false)
        //{
        //    return;
        //}

        ////プレイヤーの移動量分、自分(カメラ)も移動する
        //transform.position += Player.transform.position - PlayerPos;
        //PlayerPos = Player.transform.position;

        //プレイヤーの移動量分、自分(カメラ)も移動する
        transform.position += Target.transform.position - PlayerPos;
        PlayerPos = Target.transform.position;

        //PlayerPosの位置のY軸を中心に、回転（公転）する
        transform.RotateAround(PlayerPos, Vector3.up, InputX);

        //カメラの前後移動。
        CameraMoveBeforeAndAfter();

        //カメラの前後移動がされていないならカメラの回転をする。
        if (IsMoveBeforeOrAfterFlag != true)
        {
            //カメラの垂直移動（角度制限なし）
            transform.RotateAround(PlayerPos, transform.right, InputY);
        }
    }

    //Inputの回転をまとまたもの。
    private void InputRotate()
    {
        float DeltaSpeed = Time.deltaTime * RotateSpeed;
        // マウスの右クリックを押している間
        if (Input.GetMouseButton(1))
        {
            //マウスの移動量
            InputX = Input.GetAxis("Mouse X") * DeltaSpeed;
            InputY = Input.GetAxis("Mouse Y") * DeltaSpeed;
        }
        //マウスの右が押されていない時は右スティックの倒しを優先する。
        else
        {
            //右スティックの移動量
            InputX = Input.GetAxis("Horizontal2") * DeltaSpeed;
            InputY = Input.GetAxis("Vertical2") * DeltaSpeed;
        }

        //ホイールの入力。
        Scroll = Input.GetAxis("Mouse ScrollWheel");
    }

    ////カメラのズーム処理。
    //private void CameraZoom()
    //{
    //    float view = GemaCamera.fieldOfView - Scroll * ZoomSpeed;
    //    //Clampを使ってマイナスや極端に大きな値にならなよう調整。
    //    GemaCamera.fieldOfView = Mathf.Clamp(value: view, min:ZoomMin, max: ZoomMax);
    //}

    //ホイールとパッドを使ったカメラの前後移動。
    private void CameraMoveBeforeAndAfter()
    {
        //初期化。
        float value = 0.0f;
        float speed = 0.0f;

        //移動フラグ初期化。
        IsMoveBeforeOrAfterFlag = false;

        //コントローラーが使われているならコントローラー用の前後移動のスピードを設定。
        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            value = InputY;
            speed = ControllerCameraMoveBeforeAndAfterSpeed;
            IsMoveBeforeOrAfterFlag = true;
        }

        //マウスが使われているならマウス用の前後移動のスピードを設定。
        if (Scroll != 0.0f)
        {
            value = Scroll;
            speed = MouseCameraMoveBeforeAndAfterSpeed;
            IsMoveBeforeOrAfterFlag = true;
        }

        //どれくらい移動するか。
        Vector3 MovePos = transform.forward * -value * Time.deltaTime * speed;

        //カメラとプレイヤーの距離計算。
        float distance = (transform.position.z + MovePos.z) - PlayerPos.z;

        //スティックが前後どちらかに倒されていて、プレイヤーとカメラの距離が指定範囲内なら移動。
        if ((value > 0.0f && Mathf.Abs(distance) < PlayerToCameraMaxDistance) || (value < 0.0f && Mathf.Abs(distance) > PlayerToCameraMinDistance))
        {
            transform.position += MovePos;
        }
    }

    ////プレイヤーがnullの間はプレイヤーを探す。
    //private bool FindPlayer()
    //{
    //    if (Player == null)
    //    {
    //        //プレイヤー取得。
    //        Player = GameObject.FindWithTag("Player");
    //        if (Player != null)
    //        {
    //            //プレイヤーの位置取得。
    //            PlayerPos = Player.transform.position;
    //            //カメラの位置をプレイヤーからずらした位置に設定。
    //            Vector3 newPos = PlayerPos + StartOffsetPos;
    //            transform.position = newPos;
    //        }
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}
}
