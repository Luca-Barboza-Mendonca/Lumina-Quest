using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;

public class ChatBox : MonoBehaviour
{

    TcpClient client;
    NetworkStream NetStream;
    int chatPort;
    string chatContent = "Begin Chat";
    int kChars = 700;
    string receivedContent = "";
    public TMP_InputField chatInputBox;
    int chatUpdateTimer = 0;
    public Text chat;

    Vector2 scrollPosition;


    const int margin = 20;

	Rect windowRect = new Rect(margin, margin, (Screen.width - (margin * 2) )/ 4, (Screen.height - (margin * 2))/ 4);
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
    // Start is called before the first frame update
    void Start()
    {
        chatPort = GameObject.Find("PortData").GetComponent<ReadInput>().chatPort;
        
        client = new TcpClient("localhost", chatPort);
        NetStream = client.GetStream();
    }

    // Update is called once per frame
    void Update()
    {
        // Add some stuff to chatContent
        
        if (chatUpdateTimer >= 300){
            // The lines below causes the game to lag a lot
            SendMessage(NetStream, "");
            receivedContent = ReceiveMessage(NetStream);
            Debug.Log(receivedContent);
            chatUpdateTimer = 0;

            if (chatContent.Length > kChars) { chatContent = chatContent.Substring(chatContent.Length - kChars); }
        }

        chatContent = receivedContent;

        chatUpdateTimer ++;
    }


    public void SendChatMessage(string s){
        SendMessage(NetStream, s);
        chatInputBox.text = "";
    }

    static void SendMessage(NetworkStream stream, string message)
    {
        byte[] header = BitConverter.GetBytes(message.Length);
        stream.Write(header, 0, header.Length);

        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static string ReceiveMessage(NetworkStream stream)
    {
        byte[] headerBytes = new byte[4];
        stream.Read(headerBytes, 0, 4);
        int messageLength = BitConverter.ToInt32(headerBytes, 0);

        byte[] dataBytes = new byte[messageLength];
        stream.Read(dataBytes, 0, messageLength);
        string message = Encoding.UTF8.GetString(dataBytes);

        return message;
    }
}
