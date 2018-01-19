using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Transform target;

    private const float distance = 5.2f;
    private Vector3 _offset = new Vector3(0.0f, 1.4f, distance);
    private Vector3 _lookDown = new Vector3(-2.0f, 0.0f, 0.0f);
    private const float _followRate = 0.1f;

    void Start()
    {
        transform.position = target.TransformPoint(_offset);
        transform.LookAt(target, Vector3.up);
    }

    void FixedUpdate()
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