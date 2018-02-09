using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titlewheel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float y;
	void Update () {
        y += Time.deltaTime * -400;
        transform.rotation = Quaternion.Euler(y, 90, 0);
    }
}
