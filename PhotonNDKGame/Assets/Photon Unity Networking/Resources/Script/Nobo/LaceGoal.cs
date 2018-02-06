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
            LaceManager.GetComponent<LaceManager>().AddLacePriority(other.gameObject.GetComponent<PhotonView>().ownerId);
            other.gameObject.GetComponent<SimpleCarController>().ChangeRunFlag(); ;
            other.gameObject.GetComponent<SimpleCarController>().SetVelocity(Vector3.zero);
        }
    }
}
