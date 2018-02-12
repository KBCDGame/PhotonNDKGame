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
        PhotonInstantiatePlayer(IfPos, Quaternion.identity, 0);
        //ルーム内BGM
        SoundManager.Instance.PlayBGM(2);
  
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //Photonネットワーク上にプレイヤーを生成。
    public GameObject PhotonInstantiatePlayer(Vector3 pos,Quaternion rot, byte group)
    {
        GameObject pl = PhotonNetwork.Instantiate(this.PlayerPrefab.name, pos, rot, group);
        return pl;
    }
}
