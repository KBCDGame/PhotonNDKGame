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
    //オンライン化に必要なコンポーネントを設定。
    [SerializeField]
    private PhotonView MyPV;
    [SerializeField]
    private PhotonTransformView MyPTV;
    [SerializeField]
    private Rigidbody RigidBody;
    [SerializeField]
    private Camera MainCam;
    // finds the corresponding visual wheel
    // correctly applies the transform
    void Start()
    {
        if (MyPV)
        {
            //自キャラであれば実行。
            if (MyPV.isMine)
            {
                //MainCameraのtargetにこのゲームオブジェクトを設定。
                MainCam = Camera.main;
                MainCam.GetComponent<NoboCamera>().Target = this.gameObject.transform;
            }
        }
        else
        {
            Debug.Log(this.gameObject.name + "にPhotonViewがアタッチされていません。");
        }    
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
        if (MyPV)
        {
            //自キャラであれば実行。
            if (!MyPV.isMine)
            {
                return;
            }
        }

        ////運転手が乗っているかの判定。
        //foreach (SeatInfo seat in SeatInfoList)
        //{
        //    if (seat.DriverSeat == true)
        //    {
        //        //運転席に誰も座っていない。
        //        if (seat.PlayerID == -1)
        //        {
        //            return;
        //        }
        //        //運転手以外は運転出来ない。
        //        else if (seat.PlayerID != PhotonNetwork.player.ID)
        //        {
        //            return;
        //        }
        //    }
        //}

        float motor = maxMotorTorque * Input.GetAxis("Accel");
        float brake = maxbrakeTorque * Input.GetAxis("Jump");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

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

        if (MyPTV)
        {
            //スムーズな同期のためにPhotonTransformViewに速度値を渡す。
            Vector3 velocity = RigidBody.velocity;
            MyPTV.SetSynchronizedValues(velocity, 0);
        }
        else
        {
            Debug.Log(this.gameObject.name + "にPhotonTransformViewがアタッチされていません。");
        }
    }
}