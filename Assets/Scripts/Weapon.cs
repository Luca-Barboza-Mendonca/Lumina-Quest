using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // damage struct
    public int[] damagePoint = {1,2,3,4,5,6};
    public float[] pushForce = {2.0f, 2.2f, 2.5f, 2.8f, 3f, 3.2f};

    // Upgrade
    public int weaponLevel = 0;
    private SpriteRenderer spriteRenderer;
    public SocketManager socketManager;

    // Swing
    private Animator anim;
    private float cooldown = 0.5f;
    private float lastSwing;

    private void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();   
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space)){
            if (Time.time - lastSwing > cooldown){
                lastSwing = Time.time;
                Swing();
                socketManager.swing = 1;
            }
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter"){
            if (coll.name == "Player"){
                return;
            }
            
            Damage dmg = new Damage(){
                damageAmount = damagePoint[weaponLevel], // Probably here
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };
            // This is fucking up pvp combat
            coll.SendMessage("ReceiveDamage", dmg);
            
        }
        
    }

    private void Swing(){
        anim.SetTrigger("Swing");
        
    }

    public void UpgradeWeapon(){
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];

    }

    public void SetWeaponLevel(int level){
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
