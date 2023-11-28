using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Threading;

struct ChatMessage {
    public string data;
};


public class ChatBox : MonoBehaviour
{

    TcpClient client;
    static NetworkStream NetStream;

    static TcpClient logClient;

    static NetworkStream logStream;
    // UdpClient logClient;
    // IPEndPoint logServerEndPoint;
    int chatPort;
    static string chatContent = "Begin Chat";
    int kChars = 700;
    Thread logThread;
    int chatUpdateTimer = 0;
    string playerId;
    public TMP_InputField chatInputBox;
    public Text chat;
    public SocketManager socketManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        chatPort = GameObject.Find("PortData").GetComponent<ReadInput>().chatPort;
        chatUpdateTimer = 0;
        client = new TcpClient("127.0.0.1", chatPort);
        NetStream = client.GetStream();

        logThread = new Thread(new ThreadStart(ReceiverThread));
        logThread.Start();

        playerId = socketManager.playerDataSocket.id;

        chat.text = chatContent;
    }

    // Update is called once per frame
    void Update()
    {
        // Add some stuff to chatContent
        
        if (chatUpdateTimer >= 300){
            SendMessage(NetStream, "update");
            chat.text = chatContent;
            chatUpdateTimer = 0;

            if (chatContent.Length > kChars) { chatContent = chatContent.Substring(chatContent.Length - kChars); }
        }

        chatUpdateTimer ++;
    }


    public void SendChatMessage(string s){
        SendMessage(NetStream, "Player " + playerId + ": " + s);
        chatInputBox.text = "";
    }

    static void SendMessage(NetworkStream stream, string message)
    {
        byte[] header = BitConverter.GetBytes(message.Length);
        stream.Write(header, 0, header.Length);

        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static void ReceiverThread()
    {

        byte[] buffer = new byte[4096];

        while (true)
        {
            int bytesRead = 0;
            try{
                bytesRead = NetStream.Read(buffer, 0, buffer.Length); // hangs here forever
            } catch (SocketException ex)
            {
                Thread.Sleep(100);
                continue;
            }
            

            if (bytesRead == 0)
            {
                Debug.Log("No bytes");
                continue;
            }

            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log(dataReceived);
            chatContent = dataReceived;
            
            
        }
    }
}
