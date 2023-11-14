using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    private PlayerData playerData;
    private string saveFilePath;
    public static GameManager instance;
    private void Awake()
    {   
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //PlayerPrefs.DeleteAll();
        saveFilePath = Application.persistentDataPath + "/playerSave.json";
        instance = this;
        SceneManager.sceneLoaded += LoadState;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        PlayerData playerData = new PlayerData();
        playerData.pesos = pesos;
        playerData.experience = experience;
        playerData.weaponLevel = weapon.weaponLevel;
    }

    // Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // References
    public player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public Animator deathMenuAnim;

    // Logic
    public int pesos;
    public int experience;

    // Floating Text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    // Upgrade Weapon
    public bool TryUpgradeWeapon(){
        if (weaponPrices.Count <= weapon.weaponLevel){
            return false;
        }
        if (pesos >= weaponPrices[weapon.weaponLevel]){
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }

    // Death Menu and Respawn
    public void Respawn() {
        deathMenuAnim.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1");
        player.Respawn();

    }

    // Save State
    /* What we need to do the Save State:
    - INT preferedSkin
    - INT pesos
    - INT experience
    - INT WeaponLevel
    */
    public void SaveState()
    {
        PlayerData playerData = new PlayerData();
        playerData.pesos = pesos;
        playerData.experience = experience;
        playerData.weaponLevel = weapon.weaponLevel;
        string savePlayerData = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath , savePlayerData);

        Debug.Log("Player Data saved at" + saveFilePath);

        // string s = "";

        // s += "0" + "|";
        // s += pesos.ToString() + "|";
        // s += experience.ToString() + "|";
        // s += weapon.weaponLevel.ToString();

        // PlayerPrefs.SetString("SaveState", s);
    }

    // private IEnumerator sendPlayerDataToServer(){
    //     string uriSend = "";

    //     using UnityWebRequest webRequest = new UnityWebRequest(uriSend, "POST");
    //     webRequest.SetRequestHeader("Content-Type", "application/json");


    //     var jsonDataToSend = JsonConvert.SerializeObject(playerData);
    //     var _dataJson = jsonDataToSend;
    //     string text = File.ReadAllText(saveFilePath);
    //     byte[] rawPlayerData = Encoding.UTF8.GetBytes(_dataJson);
    //     webRequest.uploadHandler = new UploadHandlerRaw(rawPlayerData);
    // }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        // if(!PlayerPrefs.HasKey("SaveState"))
        //     return;

        // string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        // // Example of data (without split): 0|10|15|2

        // // Change player skin
        // pesos = int.Parse(data[1]);
        // experience = int.Parse(data[2]);
        // // Change the weapon level
        // weapon.SetWeaponLevel(int.Parse(data[3]));

        if (File.Exists(saveFilePath))
        {
            string loadPlayerData = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(loadPlayerData);

            pesos = playerData.pesos;
            experience = playerData.experience;
            weapon.SetWeaponLevel(playerData.weaponLevel);
        }
        else
        {
            pesos = 0;
            experience = 0;
            weapon.SetWeaponLevel(0);
        }

        Debug.Log("LoadState");


        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }
}
