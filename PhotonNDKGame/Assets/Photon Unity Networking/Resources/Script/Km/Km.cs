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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!MyPV.isMine)
        {
            return;
        }

        if (CarSpeedText!=null)
        {
            speed = Car.GetComponent<SimpleCarController>().GetVelocity().magnitude * 2.0f;
            CarSpeedText.text = speed.ToString("F0");
        }
    }

    public void SetCarSpeedText(Text text)
    {
        CarSpeedText = text;
    }
}
