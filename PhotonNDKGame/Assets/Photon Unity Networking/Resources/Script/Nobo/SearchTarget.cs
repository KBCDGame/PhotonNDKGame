using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー周辺にNPCがいないかチェック。
public class SearchTarget : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        //範囲内にPlayerがいた。
        if (other.gameObject.tag == "Player")
        {
            
        }
    }
}
