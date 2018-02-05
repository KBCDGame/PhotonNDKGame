using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOut : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
   
   public void OnClick()
    {
            PhotonNetwork.LeaveRoom();
    }
    public void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}
