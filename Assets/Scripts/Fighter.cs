using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System;

public class Fighter : MonoBehaviour
{
    public int hitpoint = 10;
    public int maxhitpoint = 10;
    public float pushRecoverySpeed = 0.2f;

    protected float immuneTime = 1.0f;
    protected float lastImmune;
    public bool isImmune = false;

    protected Vector3 pushDirection;

    protected virtual void ReceiveDamage(Damage dmg){
        if (!isImmune){
            if (Time.time - lastImmune > immuneTime){
                lastImmune = Time.time;
                hitpoint -= dmg.damageAmount;
                pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

                
    
                GameManager.instance.ShowText("-" + dmg.damageAmount.ToString(), 2500, Color.red, transform.position, Vector3.zero, 0.5f);

                if (hitpoint <= 0){
                    hitpoint = 0;
                    Death();
                }
                if (this.GetType() == typeof(ForeignPlayer))
                {
                    
                    ForeignPlayer foreignInstance = (ForeignPlayer)this;
                    var socket = foreignInstance.socketManager.socket;
                    var networkStream = socket.GetStream();
                    DamageRequest newdmgRequest = new DamageRequest();
                    newdmgRequest.hitpoints = this.hitpoint;
                    Debug.Log($"Sending Damage Request: {this.hitpoint}");
                    newdmgRequest.request = "damage";
                    newdmgRequest.id = foreignInstance.playerId;

                    var jsonDataToSend = JsonUtility.ToJson(newdmgRequest);
                    SendData(networkStream , jsonDataToSend);
                }
            }
        }
    }

    protected virtual void Death(){

    }

    static void SendData(NetworkStream networkStream, string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        networkStream.Write(dataBytes, 0, dataBytes.Length);
    }
}
