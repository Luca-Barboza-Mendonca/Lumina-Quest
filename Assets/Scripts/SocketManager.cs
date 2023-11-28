using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Linq;

public class SocketManager : MonoBehaviour
{
    public TcpClient socket;
    public int serverPort;
    public player player;
    public GameManager gameManager;
    public PlayerData playerDataSocket;
    public GameObject foreign_player;
    public int swing = 0;
    public int counter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        serverPort = Int32.Parse(GameObject.Find("PortData").GetComponent<ReadInput>().input);
        DontDestroyOnLoad(gameObject);
        string serverAddress = "localhost";
        // int serverPort = 65461;

        

        // playerDataSocket.id = "1";
        socket = new TcpClient(serverAddress, serverPort);

        var networkStream = socket.GetStream();


        string receivedData = ReceiveData(networkStream);
        if (receivedData.StartsWith("id_", StringComparison.OrdinalIgnoreCase))
        {
            const string prefix = "id_";
            string clientId = receivedData.Substring(prefix.Length);
            playerDataSocket.id = clientId;
        }
        Debug.Log($"Received data from server: {receivedData}");



        Debug.Log("Socket connected to server");

        playerDataSocket.request = "playerdata";
        playerDataSocket.xPos = player.transform.position.x;
        playerDataSocket.yPos = player.transform.position.y;
        playerDataSocket.xlocalScale = player.transform.localScale.x;
        playerDataSocket.pesos = gameManager.pesos;
        playerDataSocket.experience = gameManager.experience;
        playerDataSocket.weaponLevel = gameManager.weapon.weaponLevel;
        playerDataSocket.hitpoints = player.hitpoint;
        playerDataSocket.isAlive = player.isAlive;
        playerDataSocket.swing = swing;

        System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        //Debug.Log(timestamp);
        playerDataSocket.timestamp = timestamp;

        string playerDataJSON = JsonUtility.ToJson(playerDataSocket);
        SendData(networkStream, playerDataJSON);
    }

    // Update is called once per frame
    void Update()
    {
        if (socket == null)
        {
            return;
        }

        //If player is correctly configured, begin sending player data to server
        if (player != null && playerDataSocket.id != "")
        {
            //Grab player current position and rotation data
            playerDataSocket.request = "playerdata";
            playerDataSocket.xPos = player.transform.position.x;
            playerDataSocket.yPos = player.transform.position.y;
            playerDataSocket.xlocalScale = player.transform.localScale.x;
            playerDataSocket.pesos = gameManager.pesos;
            playerDataSocket.experience = gameManager.experience;
            playerDataSocket.weaponLevel = gameManager.weapon.weaponLevel;
            playerDataSocket.hitpoints = player.hitpoint;
            playerDataSocket.isAlive = player.isAlive;
            playerDataSocket.swing = swing;


            System.DateTime epochStart =  new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
            playerDataSocket.timestamp = timestamp;

            string playerDataJSON = JsonUtility.ToJson(playerDataSocket);
            var networkStream = socket.GetStream();
            
            if (counter%20 == 0)
            {
                swing = 0;
            }
                
            string receivedData = ReceiveData(networkStream);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(receivedData);
            foreach(var entry in jsonObject)
            {
                var dataForPlayer = entry.Value;
                string playerId = dataForPlayer["id"].ToString();
                float xPos = Convert.ToSingle(dataForPlayer["xPos"]);
                float yPos = Convert.ToSingle(dataForPlayer["yPos"]);
                float fplayerXLocalScale = 1;
                try{
                    fplayerXLocalScale = Convert.ToSingle(dataForPlayer["xlocalScale"]);
                } catch(Exception ex){
                    Debug.Log(ex);
                }
                    
                
                int fplayerIsAlive = Convert.ToInt32(dataForPlayer["isAlive"]);
                int fplayerhitpoint = Convert.ToInt32(dataForPlayer["hitpoints"]);
                int fplayerweapon = Convert.ToInt32(dataForPlayer["weaponLevel"]);
                int fplayerSwing = Convert.ToInt32(dataForPlayer["swing"]);
                // Debug.Log($"Player {playerId} is at X: {xPos} Y: {yPos}");
                int playerIndex = gameManager.FindPlayerComponentById(playerId);
                    

                if (playerId == playerDataSocket.id)
                {
                    if (counter%5 == 0)
                    {
                        player.hitpoint = fplayerhitpoint;
                        if (player.hitpoint <= 0){
                            player.Death();
                        }
                    }
                    
                    continue;
                }

                if (!gameManager.activePlayers.Contains(playerId))
                {
                    gameManager.activePlayers.Add(playerId);
                    GameObject newPlayer;

                    newPlayer = Instantiate(foreign_player, new Vector3(xPos, yPos, 0), Quaternion.identity);

                    ForeignPlayer newPlayerScript = newPlayer.GetComponent<ForeignPlayer>();
                    newPlayerScript.playerId = playerId;
                    newPlayerScript.foreignPlayerWeapon = newPlayer.GetComponentInChildren<ForeignPlayerWeapon>();

                    ForeignPlayerStruct newForeignPlayer;
                    newForeignPlayer.playerId = playerId;
                    newForeignPlayer.fPlayer = newPlayer;
                    gameManager.foreignPlayers.Add(newForeignPlayer);
                    Debug.Log($"Added player {playerId} to foreign players");
                }
                else if (gameManager.activePlayers.Contains(playerId))
                {
                    // Very wordy but should do the trick
                    ForeignPlayer fplayercomponent = gameManager.foreignPlayers[playerIndex].fPlayer.GetComponent<ForeignPlayer>();
                    if (fplayercomponent.isAlive == 1){
                        gameManager.foreignPlayers[playerIndex].fPlayer.GetComponent<Transform>().position = new Vector3(xPos, yPos, 0);
                        gameManager.foreignPlayers[playerIndex].fPlayer.GetComponent<Transform>().localScale = new Vector3(fplayerXLocalScale, 1, 1);
                        fplayercomponent.isAlive = fplayerIsAlive;
                        fplayercomponent.hitpoint = fplayerhitpoint;
                        fplayercomponent.foreignPlayerWeapon.SetWeaponLevel(fplayerweapon); 
                        if (fplayerSwing == 1)
                        {
                            fplayercomponent.foreignPlayerWeapon.Swing();
                        }
                    }
                    // else {
                           // Remove it from the game manager references and destroy it
                    //     Destroy(gameManager.foreignPlayers[playerIndex].fPlayer);
                    // }
                    
                }
            }
            SendData(networkStream, playerDataJSON);
        }
        counter++;
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
            byte[] buffer = new byte[10000];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving data: {ex.Message}");
            return null;
        }
    }

}
