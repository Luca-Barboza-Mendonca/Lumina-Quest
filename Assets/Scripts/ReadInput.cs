using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System.Text;


public class ReadInput : MonoBehaviour
{   

    TcpClient client;
    NetworkStream NetStream;
    public string input = "8888";
    public int chatPort = 8888;
    private bool connected = false;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // client = new TcpClient("localhost", 50000);
        // NetStream = client.GetStream();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadMainServerPort(string s){
        Debug.Log(s);
        int portNumber;
        if (connected == false){
        try{
            portNumber = Int32.Parse(s);
            client = new TcpClient("localhost", portNumber);
            Debug.Log("Connected to Main Server");
            NetStream = client.GetStream();
            connected = true;
            try{
                string connectionMessage = ReceiveMessage(NetStream);
                chatPort = Int32.Parse(connectionMessage);
                Debug.Log($"Chat Port Received From Server: {chatPort}");
            } catch(Exception ex){
                Debug.Log(ex);
                return;
            }
                

        } catch(Exception ex){
            Debug.Log(ex);
        }
        }
    }

    public void ReadStringInput(string s){
        int portNumber;
        try
        {
            portNumber = Int32.Parse(s);
            input = s;

        } catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void HostNewSession(){
        Debug.Log("Sending message to server");
        SendMessage(NetStream, input);
        string receivedMessage;
        receivedMessage = ReceiveMessage(NetStream);
        Debug.Log($"Return from server: {receivedMessage}");
    }

    public void ConnectToSession(){
        // Change scene and set the SocketManager port
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        // GameObject.Find("SocketManager").GetComponent<SocketManager>().serverPort = Int32.Parse(input);
        //
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
