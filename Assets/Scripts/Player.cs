using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : Mover
{
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
    }
}