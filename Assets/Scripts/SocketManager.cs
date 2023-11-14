using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class SocketManager : MonoBehaviour
{
    TcpClient socket;
    public player player;
    public GameManager gameManager;
    public PlayerData playerDataSocket;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        string serverAddress = "localhost";
        int serverPort = 65439;

        

        // playerDataSocket.id = "1";
        socket = new TcpClient(serverAddress, serverPort);

        var networkStream = socket.GetStream();

        Thread receiveThread = new Thread(() =>
        {
            string receivedData = ReceiveData(networkStream);
            if (receivedData.StartsWith("id_", StringComparison.OrdinalIgnoreCase))
            {
                const string prefix = "id_";
                string clientId = receivedData.Substring(prefix.Length);
                playerDataSocket.id = clientId;
            }
            Debug.Log($"Received data from server: {receivedData}");
        });
        receiveThread.Start();


        Debug.Log("Socket connected to server");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(player.transform.position);
        if (socket == null)
        {
            return;
        }

        //If player is correctly configured, begin sending player data to server
        if (player != null && playerDataSocket.id != "")
        {
            //Grab player current position and rotation data
            playerDataSocket.xPos = player.transform.position.x;
            playerDataSocket.yPos = player.transform.position.y;
            playerDataSocket.pesos = gameManager.pesos;
            playerDataSocket.experience = gameManager.experience;
            playerDataSocket.weaponLevel = gameManager.weapon.weaponLevel;


            System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
            //Debug.Log(timestamp);
            playerDataSocket.timestamp = timestamp;

            string playerDataJSON = JsonUtility.ToJson(playerDataSocket);
            try{
                var networkStream = socket.GetStream();
                SendData(networkStream, playerDataJSON);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error: {ex.Message}");
            }
            
            // socket.Send(playerDataJSON); 
        }

        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     string messageJSON = "{\"message\": \"Some Message From Client\"}";
        //     socket.Send(messageJSON);
        // }
    }

    static void SendData(NetworkStream networkStream, string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        networkStream.Write(dataBytes, 0, dataBytes.Length);
    }

    static string ReceiveData(NetworkStream networkStream)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data: {ex.Message}");
            return null;
        }
    }

    // private void OnDestroy()
    // {
    //     //Close socket when exiting application
    //     Debug.Log("Closing Connection");
    //     socket.Close();
    // }
}
