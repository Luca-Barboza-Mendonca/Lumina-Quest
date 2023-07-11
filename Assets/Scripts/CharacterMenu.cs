using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    public Text levelText, hitpointText, pesosText, upgradeCostText;

    // Not Doing Character selection because time, but it would be here

    public Image weaponSprite;
    public RectTransform xpBar;

    // Weapon Upgrade
    public void OnUpgradeClick(){
        // Game manager shenanigans to change weapon
    }

    // Update char info
    public void UpdateMenu(){
        // game mang
        weaponSprite.sprite = GameManager.instance.weaponSprites[0];
        upgradeCostText.text = "Not Implemented";

        // Meta
        hitpointText.text = GameManager.instance.player.hitpoint.ToString();
        pesosText.text = GameManager.instance.pesos.ToString();
        levelText.text = "Not Implemented";

        xpBar.localScale = new Vector3(0.5f, 0, 0);
    }
}
