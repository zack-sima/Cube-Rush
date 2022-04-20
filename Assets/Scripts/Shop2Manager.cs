using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Shop2Manager : MonoBehaviour {
    PlayerData playerData; 
    public Text gemText;
    public Text goldText;

    private int[] powerUpGoldCosts, powerUpGemCosts;

    public Text[] powerUpCostTexts, powerUpQuantityTexts; 

    private void Start() {
        //instant health, damage buff, screen wipe, screen freeze; cost is either paid
        //in gold or gems but cannot be both togethe
        powerUpGemCosts = new int[] {10, 0, 15, 0};
        powerUpGoldCosts = new int[] {0, 35, 0, 45}; 

        playerData = GetComponent<PlayerData>();
    }
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Update() {
        goldText.text = playerData.money.ToString();
        gemText.text = playerData.gems.ToString();

        for (int i = 0; i < powerUpCostTexts.Length; i++) {
            if (powerUpGemCosts[i] == 0) {
                powerUpCostTexts[i].text = "Buy: " + powerUpGoldCosts[i] + " Coins"; 
            } else {
                powerUpCostTexts[i].text = "Buy: " + powerUpGemCosts[i] + " Gems"; 
            }
            powerUpQuantityTexts[i].text = "Owned: " + playerData.powerUps[i]; 
        }
    }
    public void buyPowerUp(int id) {
        if (playerData.money >= powerUpGoldCosts[id] && playerData.gems >= powerUpGemCosts[id]) {
            playerData.powerUps[id]++;
            playerData.money -= powerUpGoldCosts[id];
            playerData.gems -= powerUpGemCosts[id];

            //stats
            playerData.coinsSpent += powerUpGoldCosts[id];
            playerData.gemsSpent += powerUpGemCosts[id]; 

            playerData.saveData(); 
        }
    }
}


