using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : Mover
{
    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate(){
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (isAlive)
            UpdateMotor(new Vector3(x, y, 0));
    }

    protected override void Death(){
        // kill the player
        isAlive = false;
        isImmune = true;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }

    public void Respawn() {
        hitpoint = maxhitpoint;
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
        isImmune = false;
    }
}