using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Shop1Manager : MonoBehaviour {
    PlayerData playerData; 
    public Text gemText;
    public Text goldText; 
    public Text[] towerCostTexts;
    public Text[] towerLevelTexts;

    [HideInInspector]
    public int[] mapTowerUnlocks;
    [HideInInspector]
    public int[] mapLevelTowerUnlocks; 

    private void Start() {
        playerData = GetComponent<PlayerData>();
        mapTowerUnlocks = new int[] {0, 0, 0, 0, 0, 0, 1, 1, 1, 2};
        mapLevelTowerUnlocks = new int[] {0, 1, 5, 8, 4, 2, 4, 2, 6, 1};

        for (int i = 0; i < towerCostTexts.Length; i++) {
            if (playerData.levelsUnlocked[mapTowerUnlocks[i]] < mapLevelTowerUnlocks[i]) {
                Destroy(towerLevelTexts[i].transform.parent.gameObject);
            }
        }
    }
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Update() {
        goldText.text = playerData.money.ToString();
        gemText.text = playerData.gems.ToString();

        for (int i = 0; i < towerCostTexts.Length; i++) {
            if (towerCostTexts[i] == null)
                continue; 
            towerLevelTexts[i].text = "Level " + (playerData.towerLevelsUnlocked[i] + 1).ToString(); 
            if (playerData.towerLevelsUnlocked[i] < playerData.towerLevelsCosts[i].Length) {
                towerCostTexts[i].text = "Upgrade:\n" + playerData.towerLevelsCosts[i][playerData.towerLevelsUnlocked[i]].ToString() + " Coins";
                towerCostTexts[i].fontSize = (int)(22.8f * CustomFunctions.getUIScale());  
            } else {
                towerCostTexts[i].text = "Max level";
                towerCostTexts[i].fontSize = (int)(30f * CustomFunctions.getUIScale());
            }
        }
    }
    public void upgradeTower(int towerIndex) {
        //not max level (can buy)
        if (playerData.towerLevelsUnlocked[towerIndex] < playerData.towerLevelsCosts[towerIndex].Length) {
            //player has enough money 
            if (playerData.money >= playerData.towerLevelsCosts[towerIndex][playerData.towerLevelsUnlocked[towerIndex]]) {
                playerData.money -= playerData.towerLevelsCosts[towerIndex][playerData.towerLevelsUnlocked[towerIndex]];

                //stats
                playerData.coinsSpent += playerData.towerLevelsCosts[towerIndex][playerData.towerLevelsUnlocked[towerIndex]];

                playerData.towerLevelsUnlocked[towerIndex]++;
            }
        }
        playerData.saveData(); 
    }
}
