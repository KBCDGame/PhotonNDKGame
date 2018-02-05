using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//カウントダウン。
public class CountDownTime : MonoBehaviour {
    [SerializeField]
    private float TotalTime;                //トータル制限時間。

    [SerializeField]
    private Text CountDownTimeText;         //カウントダウンのタイムを表示するテキスト。
    [SerializeField]
    private bool IsCountDownEnd;            //カウントダウン中ならture。
    [SerializeField]
    private string TimeToString = "0";      //タイムをどの形式で表示するかの文字列。
    void Start()
    {
       
    }

    void Update()
    { 
        //制限時間が0秒以下なら何もしない。
        if (TotalTime <= 0f)
        {
            return;
        }
        //一旦トータルの制限時間を計測。
        TotalTime -= Time.deltaTime;

        //数字を1桁で表示。
        CountDownTimeText.text = TotalTime.ToString(TimeToString);
    }
    //指定された秒と表示形式でカウントダウンを始める。
    public void CountDownStart(float time,string type)
    {
        TotalTime = time;
        TimeToString = type;
    }

    public bool CountDown()
    {
        //制限時間以下。
        if (TotalTime <= 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}