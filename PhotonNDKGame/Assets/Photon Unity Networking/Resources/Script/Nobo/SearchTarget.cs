using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー周辺にNPCがいないかチェック。
public class SearchTarget : MonoBehaviour {

    [SerializeField]
    private GameReservationPerson ReservationPerson;
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.Joystick1Button3))
        {
            //範囲内にPlayerがいた。
            if (other.gameObject.tag == "Player")
            {
                if (Input.GetKey(KeyCode.Joystick1Button3))
                {
                    if (ReservationPerson != null)
                    {
                        ReservationPerson.RideCarMoveStartPos(other.gameObject);
                    }
                }
            }
        } 
    }
}
