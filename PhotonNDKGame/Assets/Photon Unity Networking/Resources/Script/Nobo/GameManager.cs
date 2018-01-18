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
    //ゲーム上に配置するプレイヤー以外のオブジェクト。
    [SerializeField]
    private GameObject[] OneGeneratedObjectPrefabList;
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
        //この関数で生成したオブジェクトは生成したプレイヤーがルームから消えると一緒に消される。
        PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(IfPos.x, IfPos.y, IfPos.z), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate(this.CarPrefab.name, new Vector3(0f, 10f, 0f), Quaternion.identity, 0);

        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }

        foreach (GameObject obj in OneGeneratedObjectPrefabList)
        {
            float x = Random.Range(100.0f, 200.0f);
            float z = Random.Range(100.0f, 200.0f);
            PhotonNetwork.InstantiateSceneObject(obj.name, new Vector3(x, IfPos.y, z), Quaternion.identity, 0, null);
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
