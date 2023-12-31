using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // exp
    public int xpValue = 1;

    public float triggerLenght = 1;
    public float chaseLenght = 5;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;
    private Animator anim;
    // HitBox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    protected void FixedUpdate(){
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLenght){
            if(Vector3.Distance(playerTransform.position, startingPosition) < triggerLenght){
                chasing = true;
            }
            if (chasing){
                if (!collidingWithPlayer){
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                } 
            }else {
                    UpdateMotor(startingPosition - transform.position);
                }
        } else {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Figther" && hits[i].name == "Player"){
                collidingWithPlayer = true;
            }
            
            // The array is not cleaned up, so we do it ourself
            hits[i] = null;
        }
    }

    public override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.experience += xpValue;
        GameManager.instance.ShowText("+" + xpValue + "xp", 3000, Color.green, transform.position, Vector3.up* 40, 1.0f);
    }
}