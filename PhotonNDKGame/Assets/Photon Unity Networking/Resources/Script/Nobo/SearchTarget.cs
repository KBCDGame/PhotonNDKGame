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
            //キーボードのEnterかパッドYで車に乗る。         
            if (Input.GetKeyDown(KeyCode.Joystick1Button3)/* || Input.GetKey(KeyCode.KeypadEnter)*/)
            {
                //車のスクリプト取得。
                SimpleCarController SCar = transform.root.gameObject.GetComponent<SimpleCarController>();

                //乗る。
                SCar.OnCar(other.gameObject, PhotonNetwork.player.ID);
                Debug.Log("バグ");
                return;

            }
        }
    }

    IEnumerable A()
    {

        yield return new WaitForSeconds(1); // num秒待機
    }
}
