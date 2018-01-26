using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip Room, corse;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        //切り替え用
        if (Input.GetKeyDown("1"))
        {
            audioSource.PlayOneShot(Room);
        }
        else if (Input.GetKeyDown("2"))
        {
            audioSource.PlayOneShot(corse);
        }
       else if (Input.GetKeyDown("3"))
        {
            audioSource.Stop();
        }
	}
}
