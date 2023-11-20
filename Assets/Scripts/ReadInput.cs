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
    private string input = "8888";
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        client = new TcpClient("localhost", 50000);
        NetStream = client.GetStream();

    }

    // Update is called once per frame
    void Update()
    {
        
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
