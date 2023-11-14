using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;
using System;

public class SocketManager : MonoBehaviour
{
    TcpClient socket;
    public player player;
    public GameManager gameManager;
    public PlayerData playerDataSocket;

    // Start is called before the first frame update
    void Awake()
    {
        string serverAddress = "localhost";
        int serverPort = 65439;

        Debug.Log("Socket connected to server");

        playerDataSocket.id = "1";
        socket = new TcpClient(serverAddress, serverPort);

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

    // private void OnDestroy()
    // {
    //     //Close socket when exiting application
    //     Debug.Log("Closing Connection");
    //     socket.Close();
    // }
}
