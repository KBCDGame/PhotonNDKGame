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

    private Vector3 TargetDirection;        //移動する方向のベクトル
    private Vector3 MoveDirection = Vector3.zero;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        MoveControl();  //移動用関数。
        RotationControl(); //旋回用関数。

        //最終的な移動処理。
        //(これが無いとCharacterControllerに情報が送られないため、動けない)。
        CharaCon.Move(MoveDirection * Time.deltaTime);

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
            }
        }
        else        //空中操作の処理（重力加速度等）。
        {
            float tempy = MoveDirection.y;
            //(↓の２文の処理があると空中でも入力方向に動けるようになる)。
            //moveDirection = Vector3.Scale(targetDirection, new Vector3(1, 0, 1)).normalized;
            //moveDirection *= speed;
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

}
