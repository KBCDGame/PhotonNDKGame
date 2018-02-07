using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float x;
    float y;
    void Update()
    {

        x += Time.deltaTime * 80;
        y += Time.deltaTime * 80;
        transform.rotation = Quaternion.Euler(x, y, 0);

    }
}
