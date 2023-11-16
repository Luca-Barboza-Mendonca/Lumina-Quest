using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignPlayer : Mover
{

    //public SocketManager socketManager;
    public string playerId;
    public int isAlive;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
        //socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
    }

    // Update is called once per frame
    void Update()
    {  
               
    }

    protected override void Death()
    {
        isAlive = 0;
    }

}
