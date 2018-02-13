using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRoadCollision : MonoBehaviour {

    [SerializeField]
    private Transform SkyRoadReturnTrans; //空中コースから落ちた際に車を戻す場所。
    public void Start()
    {
        if (SkyRoadReturnTrans == null)
        {
            Debug.Log(this.gameObject.name+"Transformが設定されていません");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SkyRoadReturnTrans == null)
        {
            return;
        }

        if (other.gameObject.tag == "Car")
        {
            //車を戻す処理。
            other.gameObject.GetComponent<SimpleCarController>().Stop();
            other.gameObject.GetComponent<SimpleCarController>().transform.SetPositionAndRotation(SkyRoadReturnTrans.position, SkyRoadReturnTrans.rotation);
        }
    }
}
