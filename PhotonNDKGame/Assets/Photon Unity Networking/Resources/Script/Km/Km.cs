using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Km : MonoBehaviour
{
    Rigidbody rigid;
    float speed;
    [SerializeField]
    private GameObject Car;
    [SerializeField]
    private Text CarSpeedText;
    [SerializeField]
    private PhotonView MyPV;
    // Use this for initialization
    void Start()
    {
        CarSpeedText = GameObject.Find("CarSpeedText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!MyPV.isMine)
        {
            return;
        }
        speed = Car.GetComponent<SimpleCarController>().GetVelocity().magnitude * 2.0f;
        CarSpeedText.text = speed.ToString("F0");
    }
}
