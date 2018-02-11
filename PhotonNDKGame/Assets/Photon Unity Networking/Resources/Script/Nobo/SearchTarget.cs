using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー周辺にプレイヤーがいないかチェック。
public class SearchTarget : MonoBehaviour
{
    [SerializeField]
    private GameReservationPerson ReservationPerson;
    private void OnTriggerStay(Collider other)
    {
        //範囲内にPlayerがいた。
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                if (ReservationPerson != null)
                {
                    ReservationPerson.AddList();
                }
            }
        }

    }
}
