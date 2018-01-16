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

//座席情報。
[System.Serializable]
public class SeatInfo
{
    [SerializeField]
    public GameObject Passengers;   //乗っているオブジェクト。
    [SerializeField]
    public Transform SeatTransform; //座席。
    [SerializeField]
    public Transform OutTransform; //車から降りた時の座標。
    [SerializeField]
    public int PlayerID = -1;       //座席に座っているプレイヤーID。
    [SerializeField]
    public bool DriverSeat = false; //運転席判定。
}

public class SimpleCarController : Photon.MonoBehaviour
{
    [SerializeField]
    private List<AxleInfo> axleInfos;
    [SerializeField]
    private List<SeatInfo> SeatInfoList;
    [SerializeField]
    private float maxMotorTorque;
    [SerializeField]
    private float maxbrakeTorque;
    [SerializeField]
    private float maxSteeringAngle;
    //オンライン化に必要なコンポーネントを設定。
    [SerializeField]
    private PhotonView MyPV;
    // finds the corresponding visual wheel
    // correctly applies the transform
    void Start()
    {
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

        //運転手が乗っているかの判定。
        foreach (SeatInfo seat in SeatInfoList)
        {
            if (seat.DriverSeat == true)
            {
                //運転席に誰も座っていない。
                if (seat.PlayerID == -1)
                {
                    return;
                }
                //運転手以外は運転出来ない。
                else if (seat.PlayerID != PhotonNetwork.player.ID)
                {
                    return;
                }
            }
        }

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


        //乗り物から降りる処理。
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            OffCar();
        }
    }

    //乗る処理。
    public void OnCar(GameObject player,int id)
    {
        foreach (SeatInfo seat in SeatInfoList)
        {
            if (seat.PlayerID == id)
            {
                return;
            }

            //乗る処理。
            if (seat.PlayerID == -1)
            {
                //乗るオブジェクト設定。
                seat.Passengers = player;
                //座席とプレイヤーの親子関係構築。
                seat.Passengers.transform.parent = seat.SeatTransform;
                //位置設定と向き設定。
                seat.Passengers.transform.SetPositionAndRotation(seat.SeatTransform.position, seat.SeatTransform.localRotation);
                //PlayerID設定。
                seat.PlayerID = id;
                break;
            }
        }
    }

    //降りる処理。
    public void OffCar()
    {
        //PlayerID取得。
        int id = PhotonNetwork.player.ID;
        foreach (SeatInfo seat in SeatInfoList)
        {
            if (seat.PlayerID == id)
            {
                //親を外す。
                seat.Passengers.transform.parent = null;
                //外に出す。
                seat.Passengers.transform.position = seat.OutTransform.position;
                //乗っているオブジェクトを外す。
                seat.Passengers = null;
                //初期化。
                seat.PlayerID = -1;
                return;
            }
        }
    }

    //変数同期。
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //データの送信。
            stream.SendNext(SeatInfoList);
        }
        else
        {
            //データの受信。
            this.SeatInfoList = (List<SeatInfo>)stream.ReceiveNext();
        }
    }
}