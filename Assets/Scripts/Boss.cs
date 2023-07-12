using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    protected override void ReceiveDamage(Damage dmg){
        // Exact same behaviour except the boss does not get pushed
        if (Time.time - lastImmune > immuneTime){
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
 
            GameManager.instance.ShowText("-" + dmg.damageAmount.ToString(), 2500, Color.red, transform.position, Vector3.zero, 0.5f);

            if (hitpoint <= 0){
                hitpoint = 0;
                Death();
            }
        }
    }
}
