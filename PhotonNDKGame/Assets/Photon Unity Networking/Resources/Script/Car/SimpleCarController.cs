using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class AxleInfo
{

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool brake;
    public bool steering;
}
public class SimpleCarController : Photon.MonoBehaviour
{
    [SerializeField]
    private List<AxleInfo> axleInfos;
    [SerializeField]
    private float maxMotorTorque;
    [SerializeField]
    private float maxbrakeTorque;
    [SerializeField]
    private float maxSteeringAngle;
    
    [SerializeField]
    private Rigidbody RigidBody;   //RigidBodyのコンポーネント。
    [SerializeField]
    private Vector3 Velocity;      //RigidBodyのVelocity保存用。
    [SerializeField]
    private bool IsRunFlag;        //走っていいかどうかのフラグ。
    [SerializeField]
    private Vector3 Acceleration;//加速度

    //オンライン化に必要なコンポーネントを設定。
    [SerializeField]
    private PhotonView MyPV;
    [SerializeField]
    private PhotonTransformView MyPTV;
    // finds the corresponding visual wheel
    // correctly applies the transform
    void Start()
    {
        IsRunFlag = false;
    }
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
}
    public void FixedUpdate()
    {
        //自キャラかどうかの判定。
        //自キャラであれば実行。
        if (!MyPV.isMine)
        {
            return;
        }

        float motor = maxMotorTorque * Input.GetAxis("Accel");
        float brake = maxbrakeTorque * Input.GetAxis("Jump");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        if (IsRunFlag == false)
        {
            brake = 1.0f;
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        //スムーズな同期のためにPhotonTransformViewに速度値を渡す。
        //RigidBody.transform.position -= Acceleration;
        Velocity = RigidBody.velocity;
        MyPTV.SetSynchronizedValues(Velocity, 0);
    }

    public void Update()
    {
        //自キャラかどうかの判定。
        if (MyPV)
        {
            //自キャラであれば実行。
            if (!MyPV.isMine)
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            CarRest();
        }
    }

    //車が倒れた時に使うリセット処理。
    private void CarRest()
    {
        //回転用に座標を調整。
        transform.position = new Vector3(transform.position.x, transform.position.y + 5.0f, transform.position.z);
    
        //速度を0にする。
        RigidBody.velocity = Vector3.zero;
        
        //リジットボディで使われている回転を初期化。
        RigidBody.angularVelocity = Vector3.zero;
    }

    public Vector3 GetVelocity()
    {
        return Velocity;
    }

    public void RunFlagChangeToTrue()
    {
        IsRunFlag = true;
    }

    public void RunFlagChangeToFalse()
    {
        IsRunFlag = false;
    }
}