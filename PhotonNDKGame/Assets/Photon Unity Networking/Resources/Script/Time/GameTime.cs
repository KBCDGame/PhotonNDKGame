using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    public static float time = 0.0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        GetComponent<Text>().text = time.ToString("F2");
    }
}
