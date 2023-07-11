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
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Weapon Upgrade
    public void OnUpgradeClick(){
        // Game manager shenanigans to change weapon
        if (GameManager.instance.TryUpgradeWeapon()){
            UpdateMenu();
        }
    }

    // Update char info
    public void UpdateMenu(){
        // game mang
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLevel];
        if (GameManager.instance.weapon.weaponLevel == GameManager.instance.weaponPrices.Count)
            upgradeCostText.text = "MAX";
        else
            upgradeCostText.text = GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLevel].ToString();

        // Meta
        hitpointText.text = GameManager.instance.player.hitpoint.ToString();
        pesosText.text = GameManager.instance.pesos.ToString();
        levelText.text = "Not Implemented";

        xpBar.localScale = new Vector3(0.5f, 0, 0);
    }
}
