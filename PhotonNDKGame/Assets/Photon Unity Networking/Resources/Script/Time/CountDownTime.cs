using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//カウントダウン。
public class CountDownTime : MonoBehaviour {
    private float TotalTime; //トータル制限時間。

    //[SerializeField]
    //private int Minute;       //制限時間（分）。
    
    [SerializeField]
    private float Seconds;      //制限時間（秒）。
    
    private float OldSeconds;   //前回Update時の秒数。
    [SerializeField]
    private Text CountDownTimeText; //カウントダウンのタイムを表示するテキスト。
    [SerializeField]
    private bool IsCountDown = true;       //カウントダウン中かどうかのフラグ。

    void Start()
    {
        TotalTime =/* Minute * 60 + */Seconds;
        OldSeconds = 0f;
    }

    void Update()
    {
        //制限時間が0秒以下なら何もしない。
        if (TotalTime <= 0f)
        {
            return;
        }
        //一旦トータルの制限時間を計測。
        TotalTime =/* Minute * 60 + */Seconds;
        TotalTime -= Time.deltaTime;

        //再設定。
        //Minute = (int)TotalTime / 60;
        Seconds = TotalTime/* - Minute * 60*/;

        //タイマー表示用UIテキストに時間を表示する。
        if ((int)OldSeconds != (int)Seconds)
        {
            CountDownTimeText.text = /*Minute.ToString("00") + ":" + */Seconds.ToString("0");
        }
        
        OldSeconds = Seconds;
        //制限時間以下。
        if (TotalTime <= 0f)
        {
            CountDownTimeText.text = "スタート!!";
        }
    }
}
