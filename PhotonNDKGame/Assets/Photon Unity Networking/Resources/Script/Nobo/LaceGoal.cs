using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//車がゴールに当たったかどうかを判定するクラス。
public class LaceGoal : MonoBehaviour {
    [SerializeField]
    private GameObject LaceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            //ゴールに衝突した車のIDとプレイヤーの名前をレースマネージャーに送信。
            LaceManager.GetComponent<LaceManager>().AddLaceResult(other.gameObject.GetComponent<PhotonView>().ownerId, other.gameObject.GetComponent<PhotonView>().owner.NickName, true);
            //ゴールした車のハンドブレーキを引く。
            other.gameObject.GetComponent<SimpleCarController>().ChangeRunFlag();
        }
    }
}
