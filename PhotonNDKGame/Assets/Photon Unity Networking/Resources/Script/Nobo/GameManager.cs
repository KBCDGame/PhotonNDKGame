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

        //Photonに接続していれば自プレイヤーを生成。
        //この関数で生成したオブジェクトは生成したプレイヤーがルームから消えると一緒に消される。
        PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(IfPos.x, IfPos.y, IfPos.z), Quaternion.identity, 0);
  
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
