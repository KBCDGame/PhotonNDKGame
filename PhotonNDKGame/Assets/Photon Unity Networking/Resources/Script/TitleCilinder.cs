using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCilinder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float y;
    void Update () {
        y += Time.deltaTime * -10;
        transform.rotation = Quaternion.Euler(0, 0, y);
    }
}
