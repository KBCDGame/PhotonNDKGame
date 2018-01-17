using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {
    [SerializeField]
    private Vector3 IfPos = Vector3.zero;
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

        //float posx = Random.Range(1200.0f, 1400.0f);
        //float posz = Random.Range(630.0f, 800.0f);
        //Photonに接続していれば自プレイヤーを生成。
        PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(IfPos.x, IfPos.y, IfPos.z), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate(this.CarPrefab.name, new Vector3(0f, 10f, 0f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
