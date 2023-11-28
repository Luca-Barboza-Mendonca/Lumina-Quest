using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System;

public class player : Mover
{
    public SocketManager socketManager;
    public int isAlive = 1;

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate(){
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive == 1)
            UpdateMotor(new Vector3(x, y, 0));
    }

    public override void Death(){
        // kill the player
        isAlive = 0;
        isImmune = true;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }

    public void Respawn() {
        hitpoint = maxhitpoint;
        isAlive = 1;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        isImmune = false;
        socketManager.reviving = 1;

        DamageRequest damageRequest = new DamageRequest();
        damageRequest.hitpoints = 20;
        damageRequest.request = "damage";
        damageRequest.id = socketManager.playerDataSocket.id;

        var jsonDataToSend = JsonUtility.ToJson(damageRequest);
        SendData(socketManager.socket.GetStream(), jsonDataToSend);
    }


    static void SendData(NetworkStream networkStream, string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        networkStream.Write(dataBytes, 0, dataBytes.Length);
    }
}