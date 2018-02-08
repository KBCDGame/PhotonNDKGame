using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerManager : Photon.PunBehaviour
{
    //頭上のUIのPrefab。
    [SerializeField]
    private GameObject PlayerUIPrefab;

    [SerializeField]
    //ローカルのプレイヤーを設定。
    private static GameObject LocalPlayerInstance;

    //頭上UIオブジェクト。
    private　GameObject _UIGO;

    // Use this for initialization
    void Start () {
        if (PlayerUIPrefab != null)
        {
            //Playerの頭上UIの生成とPlayerUIScriptでのSetTarget関数呼出。
            _UIGO = Instantiate(PlayerUIPrefab) as GameObject;
            _UIGO.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

    }

    // Update is called once per frame
    void Update () {
        if (!photonView.isMine) //このオブジェクトがLocalでなければ実行しない
        {
            return;
        }
    }

    public void AnEnable()
    {
        _UIGO.gameObject.SetActive(false);
    }

    public void Enable()
    {
        _UIGO.SetActive(true);
    }
}
