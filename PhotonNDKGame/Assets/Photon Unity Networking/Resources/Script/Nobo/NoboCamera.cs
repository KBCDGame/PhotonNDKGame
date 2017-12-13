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
    private Vector3 LookAt;     //targetとoffsetによる注視する座標。

    private float Distance = 10.0f;    //キャラクターとカメラ間の角度。
    [SerializeField]
    private float Distance_Min = 1.0f;  //キャラクターとの最小距離。
    [SerializeField]
    private float Distance_Max = 20.0f; //キャラクターとの最大距離。
    private float CurrentX = 0.0f;  //カメラをX方向に回転させる角度。
    private float CurrentY = 0.0f;  //カメラをY方向に回転させる角度。

    //カメラ回転用係数(値が大きいほど回転速度が上がる)。
    [SerializeField]
    private float RotatoMoveSpeedX = 4.0f;     //マウスドラッグによるカメラX方向回転係数。
    [SerializeField]
    private float RotatoMoveSpeedY = 2.0f;     //マウスドラッグによるカメラY方向回転係数。
    [SerializeField]
    private float MoveX_QE = 2.0f;  //QEキーによるカメラX方向回転係数。

    void Start()
    {

    }

    void Update()
    {
        //QとEキーでカメラ回転。
        if (Input.GetKey(KeyCode.Q))
        {
            CurrentX += -MoveX_QE;
        }
        if (Input.GetKey(KeyCode.E))
        {
            CurrentX += MoveX_QE;
        }

       
        //マウス右クリックを押しているときだけマウスの移動量に応じてカメラが回転。
        if (Input.GetMouseButton(1))
        {
            CurrentX += Input.GetAxis("Mouse X") * RotatoMoveSpeedX;
            CurrentY += Input.GetAxis("Mouse Y") * RotatoMoveSpeedY;
            CurrentY = Mathf.Clamp(CurrentY, YAngle_Min, YAngle_Max);

        }

        //ホイールを使ったカメラの前後移動。
        Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel"), Distance_Min, Distance_Max);
    }
    void LateUpdate()
    {
        if (Target != null)  //targetが指定されるまでのエラー回避。
        {
            LookAt = Target.position + Offset;  //注視座標はtarget位置+offsetの座標。

            //カメラ旋回処理。
            Vector3 dir = new Vector3(0, 0, -Distance);
            Quaternion rotation = Quaternion.Euler(-CurrentY, CurrentX, 0);

            transform.position = LookAt + rotation * dir;   //カメラの位置を変更。
            transform.LookAt(LookAt);   //カメラをLookAtの方向に向けさせる。
        }

    }

}
