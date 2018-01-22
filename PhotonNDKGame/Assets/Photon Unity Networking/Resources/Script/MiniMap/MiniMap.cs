using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiniMap : MonoBehaviour
{
    public Transform Target;    //追跡するオブジェクトのtransform。
    private Vector3 LookAt;     //targetとOffsetによる注視する座標。

    void Start()
    {

    }

    void Update()
    {
    }
    void LateUpdate()
    {
        CameraTargetUpdate();
    }

    //回転処理。
    private void InputRotate()
    {
    }

    //カメラのズーム処理。
    private void CameraZoom()
    {
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
        //注視座標はtarget位置+Offsetの座標。
        transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z);
    }
}
