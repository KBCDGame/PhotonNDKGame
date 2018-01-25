using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoboCamera : MonoBehaviour {

    [SerializeField]
    private const float YAngle_Min = -89.0f;   //カメラのY方向の最小角度。
    [SerializeField]
    private const float YAngle_Max = 89.0f;     //カメラのY方向の最大角度。

    public Transform Target;    //追跡するオブジェクトのtransform。
    public Vector3 Offset;      //追跡対象の中心位置調整用オフセット。
    [SerializeField]
    private Vector3 LookAt;     //targetとOffsetによる注視する座標。

    [SerializeField]
    private float Distance = 10.0f;    //キャラクターとカメラ間の角度。
    [SerializeField]
    private float Distance_Min = 1.0f;  //キャラクターとの最小距離。
    [SerializeField]
    private float Distance_Max = 20.0f; //キャラクターとの最大距離。
    [SerializeField]
    private float CurrentX = 0.0f;  //カメラをX方向に回転させる角度。
    [SerializeField]
    private float CurrentY = 0.0f;  //カメラをY方向に回転させる角度。
    [SerializeField]
    private float PadZoomSpeed = 2.0f;
    [SerializeField]
    private float MouseZoomSpeed = 2.0f;

    //カメラ回転用係数(値が大きいほど回転速度が上がる)。
    [SerializeField]
    private float MouseRotatoSpeedMoveX = 4.0f;     //マウスドラッグによるカメラX方向回転係数。
    [SerializeField]
    private float MouseRotatoSpeedMoveY = 2.0f;     //マウスドラッグによるカメラY方向回転係数。
    [SerializeField]
    private float PadRotatoSpeedMoveX = 4.0f;     //パッドによるカメラX方向回転係数。
    [SerializeField]
    private float PadRotatoSpeedMoveY = 2.0f;     //パッドによるカメラY方向回転係数。
    private float RotatoSpeedMoveX_QE = 2.0f;  //QEキーによるカメラX方向回転係数。
    [SerializeField]
    private bool IsZoomFlag;                    //ズームフラグ。

    void Start()
    {
       
    }

    void Update()
    {
        //QとEキーでカメラ回転。
        if (Input.GetKey(KeyCode.Q))
        {
            CurrentX += -RotatoSpeedMoveX_QE;
        }
        if (Input.GetKey(KeyCode.E))
        {
            CurrentX += RotatoSpeedMoveX_QE;
        }
        IsZoomFlag = false;
        //ズーム。
        CameraZoom();

        if (IsZoomFlag == false)
        {
            //回転。
            InputRotate();
        }
    }
    void LateUpdate()
    {
        CameraTargetUpdate();
    }

    //回転処理。
    private void InputRotate()
    {

        //マウス右クリックを押しているときだけマウスの移動量に応じてカメラが回転。
        if (Input.GetMouseButton(1))
        {
            CurrentX += Input.GetAxis("Mouse X") * MouseRotatoSpeedMoveX;
            CurrentY += Input.GetAxis("Mouse Y") * MouseRotatoSpeedMoveY;
        }
        //右スティックを使ったカメラの回転。
        else
        {

            CurrentX += Input.GetAxis("Horizontal2") * PadRotatoSpeedMoveX;
            CurrentY -= Input.GetAxis("Vertical2") * PadRotatoSpeedMoveY;
        }

        //補正。
        CurrentY = Mathf.Clamp(CurrentY, YAngle_Min, YAngle_Max);
    }

    //カメラのズーム処理。
    private void CameraZoom()
    {
        //初期化。
        float value = 0.0f;
        float speed = 0.0f;
        //パッド用のズームスピードを設定。
        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            value = Input.GetAxis("Vertical2");
            speed = PadZoomSpeed;
            IsZoomFlag = true;
        }
        //ホイールを使ったカメラのズームスピード設定。
        else
        {
            value = Input.GetAxis("Mouse ScrollWheel");
            speed = MouseZoomSpeed;
        }


        Distance = Mathf.Clamp(Distance - value * speed, Distance_Min, Distance_Max);
    }

    //カメラのターゲット切り替え。
    public void ChangeTarget(Transform trans)
    {
        Target = trans;

        CameraTargetUpdate();
    }

    private void CameraTargetUpdate()
    {
        //nullエラー回避。
        if (Target == null)
        {
            Debug.Log("MainCameraのTargetがnullでした。");
            return;
        }
        LookAt = Target.position + Offset;  //注視座標はtarget位置+Offsetの座標。

        //カメラ旋回処理
        Vector3 dir = new Vector3(0, 0, -Distance);
        Quaternion rotation = Quaternion.Euler(-CurrentY, CurrentX, 0);

        transform.position = LookAt + rotation * dir;   //カメラの位置を変更。
        transform.LookAt(LookAt);   //カメラをLookAtの方向に向けさせる。
    }
}
