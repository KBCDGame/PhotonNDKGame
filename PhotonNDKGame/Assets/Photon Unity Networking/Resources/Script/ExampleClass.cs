using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    //車に乗っている時のカメラの中心点となるターゲット。
    public Transform target;

    //ターゲットからカメラをどれだけ離すか。
    //[SerializeField]
    private const float distance = 5.2f;
   //[SerializeField]
    private Vector3 _offset = new Vector3(0.0f, 1.5f, distance);
    //[SerializeField]
    private Vector3 _lookDown = new Vector3(-15.0f, 0.0f, 0.0f);
    //[SerializeField]
    private const float _followRate = 0.01f;

    void Start()
    {
        transform.position = target.TransformPoint(_offset);
        transform.LookAt(target, Vector3.up);
    }

    void FixedUpdate()
    {
        if (target!=null)
        {
            Vector3 desiredPosition = target.TransformPoint(_offset);
            Vector3 lerp = Vector3.Lerp(transform.position, desiredPosition, _followRate);
            Vector3 toTarget = target.position - lerp;
            toTarget.Normalize();
            toTarget *= distance;
            transform.position = target.position - toTarget;
            transform.LookAt(target, Vector3.up);
            transform.Rotate(_lookDown);
        }
    }

    public void SetTarget(Transform trans)
    {
        target = trans;
        transform.position = target.TransformPoint(_offset);
        transform.LookAt(target, Vector3.up);
    }
}