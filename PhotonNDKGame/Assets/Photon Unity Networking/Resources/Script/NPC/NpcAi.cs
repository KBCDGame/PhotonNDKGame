using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAi : MonoBehaviour {

    CharacterController controller = null;
    new Transform transform;
    Animation animation;

    bool change;
    public float range;
    //向かうところ
   // Vector3 target;
    float maxRotSpeed = 200.0f;
    float minTime = 0.1f;
    float velocity;

    [Header("移動速度。")]
    public float speed = 5f;
    [Header("重力。")]
    public float gravity = 20f;
    Vector3 moveDirection;
    //目標地点リスト
    public Transform[] waypoint = new Transform[4];
  　 int index;
    bool idle;

    void Start()
    {
        
        animation = GetComponent<Animation>();
        index = 0;
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        range = 4.0f;
    }
   

    void Update()
    {
     
        if ((transform.position - waypoint[index].position).sqrMagnitude > range)
        {
            Move(waypoint[index]);

        }
        else NextIndex();

    }

   
   
    //移動、回転関数
    void Move(Transform target)
    {
        moveDirection = transform.forward;
        moveDirection *= speed;
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        //移動のためのコード  
        var newRotation = Quaternion.LookRotation(target.position - transform.position).eulerAngles;
        var angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angles.x,
            Mathf.SmoothDampAngle(angles.y, newRotation.y, ref velocity, minTime, maxRotSpeed),
                angles.z);
    }


    void NextIndex()
    {
        if (++index == waypoint.Length) index = 0;
    }
    
   
}
