using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is present in all scenes that require the retrieval of binary data
public class PlayerData : MonoBehaviour {
    //these variables, if edited ingame, are saved into the binary system as well (when s is pressed)
    public int money;

    public int gems; 

    public int[] towerLevelsUnlocked; 

    public int[] levelsUnlocked;

    public int[] powerUps;

    public bool publishingUnlocked;

    public bool allLevelsUnlocked; //iap purchase

    public int endlessRecord; //game stats

    public int towersPlaced; 

    public int towersSold;

    public int damageDealt;

    public int gamesPlayed; 

    public int coinsSpent;

    public int gemsSpent; 

    //
    //not part of saved data from here on
    //
      
    public int[][] towerLevelsCosts; 

    //number of maps in the game  
    private int mapQuantity = 4;

    //number of power ups
    private int powerUpsQuantity = 4; 

    //number of towers;
    private int towerQuantity = 10;

    //warning: this function should NOT be called at any time besides by awake() for actual builds 
    public void resetData () {
        PlayerPrefs.SetInt("useSound", 1);
        endlessRecord = 0; 
        money = 800; 
        gems = 30;
        towersPlaced = 0;
        towersSold = 0;
        damageDealt = 0;
        gamesPlayed = 0;
        coinsSpent = 0;
        gemsSpent = 0;

    PlayerPrefs.SetInt("useData", 0);   
        powerUps = new int[powerUpsQuantity]; 
        levelsUnlocked = new int[mapQuantity]; 
        towerLevelsUnlocked = new int[towerQuantity]; 
        publishingUnlocked = false; 
        allLevelsUnlocked = false; 



        for (int i = 0; i < powerUps.Length; i++) {
            powerUps[i] = 1; 
        }

        //i starts at 1 because the first level of the first map must be unlocked at the very beginning 
        for (int i = 1; i < levelsUnlocked.Length; i++) {
            levelsUnlocked[i] = -1;
        }
        saveData(); 
    }

    //this function MUST be removed when publishing as it gives the player an absurd amount of money and gems. Use
    //with caution 
	public void makeOP() { 
		money += 1000000; 
        gems += 9999; 
        unlockAllLevels(); 
	}
    //reserved for the IAP 
    private void unlockAllLevels() {
        for (int i = 0; i < levelsUnlocked.Length; i++)
            levelsUnlocked[i] = 10;
        saveData(); 
    }
    private void Update() {
        //turn ke[]friggered functions into comments for published versions

        if (Input.GetKey(KeyCode.R) && Input.GetKeyDown(KeyCode.E)) {
            resetData();
        }
        if (Input.GetKeyDown(KeyCode.S))
            saveData();
    }
    void Awake() {
        towerLevelsCosts = new int[][]{
            new int[] {300, 500}, //crossbow
            new int[] {350, 600}, //cannon
            new int[] {500, 750}, //gatling
            new int[] {250, 400}, //laser
            new int[] {350, 600}, //ice cannon
            new int[] {300, 500}, //landmine
            new int[] {450, 700}, //missile launcher 
            new int[] {350, 600}, //radar
            new int[] {500, 800}, //flamethrower
            new int[] {450, 600} //tesla coil
        };

        BinaryData data = BinarySaveSystem.LoadData();
        
        if (data == null || data.towerLevelsUnlocked == null || data.levelsUnlocked == null || data.powerUps == null) {
            print("must reset due to incomplete data set");
            resetData(); 
            return; 
        }
        if (data.levelsUnlocked.Length < mapQuantity) {
            //make sure data is up to date but don't reset money and previous levelsunlocked  
            int[] oldLevelsUnlocked = data.levelsUnlocked;
            levelsUnlocked = new int[mapQuantity];
            for (int i = 0; i < levelsUnlocked.Length; i++) {
                if (i < oldLevelsUnlocked.Length) {
                    levelsUnlocked[i] = oldLevelsUnlocked[i];
                } else {
                    break;
                }
            }
        } else {
            levelsUnlocked = data.levelsUnlocked;
        }
        if (data.towerLevelsUnlocked.Length < towerQuantity) {
            int[] oldTowerLevelsUnlocked = data.towerLevelsUnlocked;
            towerLevelsUnlocked = new int[towerQuantity];
            for (int i = 0; i < towerLevelsUnlocked.Length; i++) {
                if (i < oldTowerLevelsUnlocked.Length) {
                    towerLevelsUnlocked[i] = oldTowerLevelsUnlocked[i];
                } else {
                    break;
                }
            }
        } else {
            towerLevelsUnlocked = data.towerLevelsUnlocked;
        }
        if (data.powerUps.Length < mapQuantity) {
            int[] oldPowerUps = data.powerUps;
            powerUps = new int[mapQuantity];
            for (int i = 0; i < powerUps.Length; i++) {
                if (i < oldPowerUps.Length) {
                    powerUps[i] = oldPowerUps[i];
                } else {
                    break;
                }
            }
        } else {
            powerUps = data.powerUps; 
        }

        money = data.money; 
        gems = data.gems;
        endlessRecord = data.endlessRecord; 
        publishingUnlocked = data.publishingUnlocked;
        allLevelsUnlocked = data.allLevelsUnlocked;
        towersPlaced = data.towersPlaced;
        towersSold = data.towersSold;
        damageDealt = data.damageDealt;
        gamesPlayed = data.gamesPlayed;
        coinsSpent = data.coinsSpent;
        gemsSpent = data.gemsSpent;

        if (allLevelsUnlocked)
            unlockAllLevels();
    }
    public void saveData () {
        //only do this search if the playerprefs was previously saved 
        if (PlayerPrefs.GetInt("map0LevelQuantity") > 0) {
            for (int i = 0; i < levelsUnlocked.Length - 1; i++) {
                if (levelsUnlocked[i] >= PlayerPrefs.GetInt("map" + i.ToString() + "LevelQuantity")) {
                    //all levels in map[i] unlocked, so next map is unlocked 
                    if (levelsUnlocked[i + 1] == -1)
                        levelsUnlocked[i + 1] = 0;
                }
            }
        }
        BinarySaveSystem.SaveData(this); 
    }
}
