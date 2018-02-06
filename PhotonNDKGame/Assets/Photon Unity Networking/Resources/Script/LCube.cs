using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float x = 80;
    float y = 80;
    void Update()
    {

       
        transform.rotation = Quaternion.Euler(x, y, 0);

    }
}
