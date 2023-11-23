using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;
using System.Net;

public class ChatBox : MonoBehaviour
{

    UdpClient client;
    IPEndPoint serverEndPoint;
    int chatPort;
    string chatContent = "Begin Chat";
    int kChars = 700;
    public TMP_InputField chatInputBox;
    int chatUpdateTimer = 0;
    public Text chat;
    
    // Start is called before the first frame update
    void Start()
    {
        chatPort = GameObject.Find("PortData").GetComponent<ReadInput>().chatPort;
        chatUpdateTimer = 0;
        client = new UdpClient(chatPort);
        IPAddress remoteAddress = IPAddress.Parse("127.0.0.1");
        serverEndPoint = new IPEndPoint(remoteAddress, chatPort);

        chat.text = chatContent;
    }

    // Update is called once per frame
    void Update()
    {
        // Add some stuff to chatContent
        
        if (chatUpdateTimer >= 700){
            // The lines below causes the game to lag a lot
            SendMessage(client, serverEndPoint, "");
            chatContent = ReceiveMessage(client, serverEndPoint);
            chat.text = chatContent;
            chatUpdateTimer = 0;

            if (chatContent.Length > kChars) { chatContent = chatContent.Substring(chatContent.Length - kChars); }
        }

        chatUpdateTimer ++;
    }


    public void SendChatMessage(string s){
        SendMessage(client, serverEndPoint, s);
        chatInputBox.text = "";
    }

    static void SendMessage(UdpClient clientsock, IPEndPoint server, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        byte[] header = BitConverter.GetBytes(message.Length);
        byte[] dataToSend = new byte[header.Length + data.Length];

        Buffer.BlockCopy(header, 0, dataToSend, 0, header.Length);
        Buffer.BlockCopy(data, 0, dataToSend, header.Length, data.Length);

        Debug.Log(Encoding.UTF8.GetString(data));
        clientsock.Send(dataToSend, dataToSend.Length, server);
    }

    static string ReceiveMessage(UdpClient clientsock, IPEndPoint server)
    {
        try {
            clientsock.Client.ReceiveTimeout = 50;

            byte[] receivedBytes = clientsock.Receive(ref server);
            byte[] header = new byte[4];
            byte[] message = new byte[receivedBytes.Length - 4];
            Array.Copy(receivedBytes, header, 4);
            Array.Copy(receivedBytes, 4, message, 0, receivedBytes.Length - 4);

            // Extract the message length from the header
            int messageLength = BitConverter.ToInt32(header);
                
            // Extract the actual message
            string receivedMessage = Encoding.UTF8.GetString(message);

            return receivedMessage;
        } catch (SocketException ex){
            Debug.Log(ex.SocketErrorCode);
            return "";
        }
        
    }
}
