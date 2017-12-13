using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NpcEvent : MonoBehaviour {
  

    private void OnTriggerEnter(Collider other)
    {
        //Playertagを持っているものが当たると
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("おんどぅる");
            //Destroy(gameObject);
        }
    }

}
