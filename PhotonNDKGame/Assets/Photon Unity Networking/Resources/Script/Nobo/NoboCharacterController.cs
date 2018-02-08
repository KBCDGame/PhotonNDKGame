using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoboCharacterController : MonoBehaviour {

    [SerializeField]
    //移動処理に必要なコンポーネントを設定。
    private CharacterController CharaCon;  //キャラクター移動を管理するためCharacterControllerを取得。
    //移動速度等のパラメータ用変数(inspectorビューで設定)。
    [SerializeField]
    private float Speed;         //キャラクターの移動速度。
    [SerializeField]
    private float JumpSpeed;     //キャラクターのジャンプ力。
    [SerializeField]
    private float RotateSpeed;   //キャラクターの方向転換速度。
    [SerializeField]
    private float Gravity;       //キャラにかかる重力の大きさ。

    //オンライン化に必要なコンポーネントを設定。
    [SerializeField]
    private PhotonView MyPV;
    [SerializeField]
    private PhotonTransformView MyPTV;

    [SerializeField]
    public Camera MainCam;
    private Vector3 TargetDirection;        //移動する方向のベクトル。
    private Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    private Animator PlayerAnimator; //アニメーション。

    // Use this for initialization
    void Start()
    {
        //自キャラであれば実行。
        if (MyPV.isMine)
        {
            //MainCameraのtargetにこのゲームオブジェクトを設定。
            MainCam = Camera.main;
            MainCam.GetComponent<NoboCamera>().ChangeTarget(this.gameObject.transform);

            PlayerAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //自キャラじゃなければ実行。
        if (!MyPV.isMine)
        {
            return;
        }

        MoveControl();  //移動用関数。
        RotationControl(); //旋回用関数。

        //最終的な移動処理。
        //(これが無いとCharacterControllerに情報が送られないため、動けない)。
        CharaCon.Move(MoveDirection * Time.deltaTime);


        //スムーズな同期のためにPhotonTransformViewに速度値を渡す。
        Vector3 velocity = CharaCon.velocity;
        MyPTV.SetSynchronizedValues(velocity, 0);

        if (PlayerAnimator != null)
        {
            Vector2 xz = new Vector2(MoveDirection.x, MoveDirection.z);
            if (xz.magnitude > 0.1f)
            {
                PlayerAnimator.SetFloat("Speed", MoveDirection.magnitude);
            }
            else
            {
                PlayerAnimator.SetFloat("Speed", 0.0f);
            }
        }
    }

    private void MoveControl()
    {
        //進行方向計算。
        //キーボード入力を取得。
        float v = Input.GetAxisRaw("Vertical");         //InputManagerの↑↓の入力。    
        float h = Input.GetAxisRaw("Horizontal");       //InputManagerの←→の入力。
        //カメラの正面方向ベクトルからY成分を除き、正規化してキャラが走る方向を取得。
        Vector3 forward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Camera.main.transform.right; //カメラの右方向を取得。
        
        
        //カメラの方向を考慮したキャラの進行方向を計算。
        TargetDirection = h * right + v * forward;

        //地上にいる場合の処理。
        if (CharaCon.isGrounded)
        {
            //移動のベクトルを計算。
            MoveDirection = TargetDirection * Speed;
            //Jumpボタンでジャンプ処理。
            if (Input.GetButton("Jump"))
            {
                MoveDirection.y = JumpSpeed;
                //PlayerAnimator.SetBool("is_jump", true);
            }
        }
        else        //空中操作の処理（重力加速度等）。
        {
            float tempy = MoveDirection.y;
            //↓の２文の処理があると空中でも入力方向に動けるようになる)。
            MoveDirection = Vector3.Scale(TargetDirection, new Vector3(1, 0, 1)).normalized;
            MoveDirection *= Speed;

            MoveDirection.y = tempy - Gravity * Time.deltaTime;
        }
    }

     private void RotationControl()  //キャラクターが移動方向を変えるときの処理。
    {
        Vector3 rotateDirection = MoveDirection;
        rotateDirection.y = 0;

        //それなりに移動方向が変化する場合のみ移動方向を変える。
        if (rotateDirection.sqrMagnitude > 0.01)
        {
            //緩やかに移動方向を変える
            float step = RotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.Slerp(transform.forward, rotateDirection, step);
            transform.rotation = Quaternion.LookRotation(newDir);
            
        }
    }
    //if (Input.GetKey(KeyCode.Joystick1Button0))
    //{
    //    Debug.Log("Button A Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button1))
    //{
    //    Debug.Log("Button B Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button2))
    //{
    //    Debug.Log("Button X Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button3))
    //{
    //    Debug.Log("Button Y Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button4))
    //{
    //    Debug.Log("Button LB Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button5))
    //{
    //    Debug.Log("Button RB Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button6))
    //{
    //    Debug.Log("Button Back Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button7))
    //{
    //    Debug.Log("Button START Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button8))
    //{
    //    Debug.Log("L Stick Push Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button9))
    //{
    //    Debug.Log("R Stick Push");
    //}
    //float TrigerInput = Input.GetAxis("Triger");
    //if (TrigerInput < 0.0f)
    //{
    //    Debug.Log("L Triger");
    //}
    //else if (TrigerInput > 0.0f)
    //{
    //    Debug.Log("R Triger");
    //}
    //float HorizontalKeyInput = Input.GetAxis("HorizontalKey");
    //if (HorizontalKeyInput < 0.0f)
    //{
    //    Debug.Log("Left Key");
    //}
    //else if (HorizontalKeyInput > 0.0f)
    //{
    //    Debug.Log("Right Key");
    //}
    //float VerticalKeyInput = Input.GetAxis("VerticalKey");
    //if (VerticalKeyInput < 0.0f)
    //{
    //    Debug.Log("Up Key");
    //}
    //else if (VerticalKeyInput > 0.0f)
    //{
    //    Debug.Log("Down Key");
    //}	
}
