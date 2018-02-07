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
            LaceManager.GetComponent<LaceManager>().AddLaceResult(other.gameObject.GetComponent<PhotonView>().ownerId, PhotonNetwork.playerName, true);
            other.gameObject.GetComponent<SimpleCarController>().ChangeRunFlag();
        }
    }
}
