using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RoomChat : Photon.MonoBehaviour {

    public Rect GuiRect = new Rect(0, 0, 250, 300);
    public bool isVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    public List<bool>chatkind=new List<bool>();//チャットログの種類格納
    public string inputLine = "";//入力文章格納用string
    private Vector2 scrollPos = Vector2.zero;//スクロールバー位置

    public static readonly string ChatRPC = "Chat";

    public void Start()
    {
       
    }

    public void Update()
    {
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
            //ChatUIの大きさ調整
            GuiRect.width = Screen.width / 3;
            GuiRect.height = Screen.height / 3;
        }
    }
    public void OnGUI()
    {
        if (!this.isVisible || !PhotonNetwork.inRoom)
        {
            return;
        }

        if (Event.current.type == EventType.keyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return;
            }
            else
            {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
        for(int i = messages.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine);
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [PunRPC]
    public void Chat(string newLine,PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.NickName))
            {
                senderName = mi.sender.NickName;
            }
            //ニックネームがない時はPlayerとIDが表示される
            else
            {
                senderName = "Player" + mi.sender.ID;
            }
        }
        this.messages.Add(senderName + ":" + newLine);
    }

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}
