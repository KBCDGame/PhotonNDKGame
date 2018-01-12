using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー周辺にNPCがいないかチェック。
public class SearchTarget : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        //範囲内にNPCがいた。
        if (other.gameObject.tag == "NPC")
        {
          
            ////BボタンでGame予約ウィンドウを呼び出す。
            //GameReservationPerson Grp = other.gameObject.GetComponent<GameReservationPerson>();
            if (Input.GetKey(KeyCode.Joystick1Button1))
            {
                PhotonNetwork.LoadLevel("TimeAttack");
            }
        }
    }
}
