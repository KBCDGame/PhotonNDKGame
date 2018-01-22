using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Km : MonoBehaviour
{
    Rigidbody rigid;
    float speed;
    // Use this for initialization
    void Start()
    {
        rigid = GameObject.Find("ferrociari_red 1 1(Clone)").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = rigid.velocity.magnitude*2.0f;
        GetComponent<Text>().text = speed.ToString("F0");
    }
}
