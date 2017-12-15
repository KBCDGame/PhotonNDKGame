using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class door : MonoBehaviour
{
    void OnTriggerStay(Collider collider)
    {
        if (Input.GetKey(KeyCode.Joystick1Button0))
        {
            if (collider.gameObject.tag == "Player")
            {

                collider.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
                //SceneManager.LoadScene("DoiScene2");
            }
        }
    }
}