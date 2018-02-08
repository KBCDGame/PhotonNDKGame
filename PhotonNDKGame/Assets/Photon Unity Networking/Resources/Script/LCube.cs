using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float y;
    void Update()
    {

        y += Time.deltaTime * 40;
        transform.rotation = Quaternion.Euler(-90, y, 0);

    }
}
