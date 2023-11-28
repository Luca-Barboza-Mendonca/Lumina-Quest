using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System;

public class ForeignPlayer : Mover
{
    public GameManager gameManager;
    public SocketManager socketManager;
    public string playerId;
    public int isAlive;
    public ForeignPlayerWeapon foreignPlayerWeapon;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isAlive = 1;
        DontDestroyOnLoad(gameObject);
        socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {  
        
    }

    // protected override void ReceiveDamage(Damage dmg)
    // {
    //     base.ReceiveDamage(dmg);
    //     if (Time.time - lastImmune > immuneTime){
    //         var socket = socketManager.socket;
    //         var networkStream = socket.GetStream();
    //         DamageRequest newdmgRequest = new DamageRequest();
    //         newdmgRequest.hitpoints = this.hitpoint;
    //         newdmgRequest.request = "damage";
    //         newdmgRequest.id = this.playerId;

    //         var jsonDataToSend = JsonUtility.ToJson(newdmgRequest);
    //         SendData(networkStream , jsonDataToSend);
    //     }
    // }

    public override void Death()
    {
        Debug.Log("Destroying foreign player");
        gameManager.activePlayers.Remove(playerId);
        gameManager.RemoveForeignPlayer(playerId);
        Destroy(this.gameObject);
    }

    static void SendData(NetworkStream networkStream, string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        networkStream.Write(dataBytes, 0, dataBytes.Length);
    }
}
