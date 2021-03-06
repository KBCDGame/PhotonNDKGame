﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    [SerializeField]
    private int Minute; //秒。
    [SerializeField]
    private float Seconds;  //分。
    //前のUpdateの時の秒数
    private float OldSeconds;
    //　タイマー表示用テキスト
    [SerializeField]
    private Text TimerText;
    // Use this for initialization
    void Start()
    {
        ResetGameTime();
    }
    // Update is called once per frame  
    void Update()
    {

        //秒の加算。
        Seconds += Time.deltaTime;

        //60秒を超えた。
        if (Seconds >= 60f)
        {
            //分を加算。
            Minute++;

            //細かい秒数を消さないように60を引く。
            Seconds = Seconds - 60;
        }

        //分は00分、秒は0.00まで表示。
        TimerText.text = string.Format("{0:00}:{1:00.00}", Minute, Seconds);

        OldSeconds = Seconds;
    }

    //各数値の初期化。
    public void ResetGameTime()
    {
        Minute = 0;
        Seconds = 0f;
        OldSeconds = 0f;
    }
}
