using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//カウントダウン。
public class CountDownTime : MonoBehaviour {
    private float TotalTime;                //トータル制限時間。
    [SerializeField]
    private float Seconds;                  //制限時間（秒）。
    
    private float OldSeconds;               //前回Update時の秒数。
    [SerializeField]
    private Text CountDownTimeText;         //カウントダウンのタイムを表示するテキスト。
    [SerializeField]
    private bool IsCountDownEnd = false;    //カウントダウンが終わったかどうかのフラグ。

    void Start()
    {
        TotalTime = Seconds;
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
        TotalTime = Seconds;
        TotalTime -= Time.deltaTime;

        //再設定。
        Seconds = TotalTime;
        //数字を1桁で表示。
        CountDownTimeText.text =Seconds.ToString("0");

        //1フレーム前の時間を設定。
        OldSeconds = Seconds;
        //制限時間以下。
        if (TotalTime <= 0f)
        {
            IsCountDownEnd = true;
        }
    }

    //カウントダウンのタイムを設定。
    public void SetCountTime(float scond)
    {
        TotalTime = scond;
        OldSeconds = 0f;
    }

    //カウントダウンが終わったかどうかのフラグを取得。
    public bool GetCountDownEnd()
    {
        return IsCountDownEnd;
    }
}
