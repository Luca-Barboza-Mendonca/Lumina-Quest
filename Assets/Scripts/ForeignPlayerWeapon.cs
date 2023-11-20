using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignPlayerWeapon : Collidable
{
    // damage struct
    public int[] fdamagePoint = {1,2,3,4,5,6};
    public float[] fpushForce = {2.0f, 2.2f, 2.5f, 2.8f, 3f, 3.2f};

    // Upgrade
    public int weaponLevel = 0;
    private SpriteRenderer spriteRenderer;

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
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter"){
            if (coll.name == "ForeignPlayer(Clone)"){
                return;
            }
            
            Damage dmg = new Damage(){
                damageAmount = fdamagePoint[weaponLevel],
                origin = transform.position,
                pushForce = fpushForce[weaponLevel]
            };
            coll.SendMessage("ReceiveDamage", dmg);
            
        }
        
    }

    public void Swing(){
        anim.SetTrigger("Swing");
    }


    public void SetWeaponLevel(int level){
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
