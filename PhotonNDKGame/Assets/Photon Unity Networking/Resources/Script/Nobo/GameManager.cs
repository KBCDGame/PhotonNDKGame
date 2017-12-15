using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    //誰かがログインする度に生成するプレイヤーPrefab。
    [SerializeField]
    private GameObject PlayerPrefab;
    void Start()
    {
        if (!PhotonNetwork.connected)   //Phootnに接続されていなければ。
        {
            SceneManager.LoadScene("Title"); //ログイン画面に戻る。
            return;
        }
        //Photonに接続していれば自プレイヤーを生成。
        PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 10f, 0f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
