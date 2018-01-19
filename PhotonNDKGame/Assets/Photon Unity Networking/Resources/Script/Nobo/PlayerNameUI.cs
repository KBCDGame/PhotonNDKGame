using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    [SerializeField]
    //キャラの頭上に乗るように調整するためのOffset。
    private Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

    [SerializeField]
    //プレイヤー名前設定用Text。
    private Text PlayerNameText;

    //追従するキャラのPlayerManager情報。
    private PlayerManager Target;
    //キャラクターの高さ。
    private float CharConHeight;
    [SerializeField]
    //車用の高さ。
    private float CarHeight;
    //TargetのTransform。
    private Transform TargetTransform;
    //Targetの座標。
    private Vector3 TargetPosition;

    void Awake()
    {
        //このオブジェクトはCanvasオブジェクトの子オブジェクトとして生成。
        this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
    }


    // Update is called once per frame
    void Update()
    {
        //もしPlayerがいなくなったらこのオブジェクトも削除。
        if (Target == null)
        {
            Destroy(this.gameObject);
            return;
        }

    }

    void LateUpdate()
    {
        //Targetのオブジェクトを追跡する。
        if (TargetTransform != null)
        {
            //三次元空間上のTargetの座標を得る。
            TargetPosition = TargetTransform.position;

            //キャラクターの背の高さを考慮する。
            TargetPosition.y += CharConHeight;

            //targetの座標から頭上UIの画面上の二次元座標を計算して移動させる。
            this.transform.position = Camera.main.WorldToScreenPoint(TargetPosition) + ScreenOffset;
        }
    }
    public void SetTarget(PlayerManager target)
    {
        //targetがいなければエラーをConsoleに表示。
        if (target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        //Targetの情報をこのスクリプト内で使うのでコピー。
        Target = target;
        TargetTransform = Target.GetComponent<Transform>();
        CharacterController _characterController = Target.GetComponent<CharacterController>();

        //PlayerManagerの頭上UIに表示したいデータをコピー。
        if (_characterController != null)
        {
            CharConHeight = _characterController.height;
        }
        else
        {
            CharConHeight = CarHeight;
        }

        if (PlayerNameText != null)
        {
            PlayerNameText.text = Target.photonView.owner.NickName;
        }
    }
}
