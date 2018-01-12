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

public class SimpleCarController : MonoBehaviour
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
    private Camera MainCam;
    //private Camera clientCamera;

    // finds the corresponding visual wheel
    // correctly applies the transform
    void Start()
    {
        //clientCamera = this.GetComponentInChildren<Camera>();
        //MainCameraのtargetにこのゲームオブジェクトを設定。
        MainCam = Camera.main;
        MainCam.GetComponent<NoboCamera>().Target = this.gameObject.transform;
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
        //clientCamera.depth = 1;
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
    }
}