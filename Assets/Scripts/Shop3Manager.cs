using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop3Manager : MonoBehaviour {
    PlayerData playerData; 
    public Text gemText;
    public Text goldText;
    public Text publishingUnlockedText;
    public GameObject unlockAllLevelsButton; 
    private int[] goldTradeQuantity, gemsTradeQuantity;

    private void Start() {
        goldTradeQuantity = new int[] {
            100, 620, 1500, 3500
        };
        gemsTradeQuantity = new int[] {
            10, 50, 100, 200
        }; 
        playerData = GetComponent<PlayerData>();
        checkNonConsumable(); 
    }
    public void checkNonConsumable() {
        if (playerData.allLevelsUnlocked) {
            unlockAllLevelsButton.GetComponent<Button>().interactable = false;
            unlockAllLevelsButton.transform.GetChild(5).GetComponent<Text>().enabled = true;

            unlockAllLevelsButton.transform.GetChild(3).GetComponent<Text>().enabled = false;
            unlockAllLevelsButton.transform.GetChild(4).GetComponent<Text>().enabled = false;

            unlockAllLevelsButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            unlockAllLevelsButton.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            unlockAllLevelsButton.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            unlockAllLevelsButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
        }
        if (playerData.publishingUnlocked) {
            publishingUnlockedText.enabled = false;
            publishingUnlockedText.transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            publishingUnlockedText.transform.parent.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            publishingUnlockedText.transform.parent.GetChild(1).GetComponent<Text>().enabled = false;
            publishingUnlockedText.transform.parent.GetComponent<Button>().interactable = false;
            publishingUnlockedText.transform.parent.GetChild(3).GetComponent<Text>().enabled = true; 

        }
    }
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Update() { 
        goldText.text = playerData.money.ToString();
        gemText.text = playerData.gems.ToString();
    }
    public void tradeGold(int tierIndex) {
        if (playerData.gems >= gemsTradeQuantity[tierIndex]) {
            playerData.money += goldTradeQuantity[tierIndex];
            playerData.gems -= gemsTradeQuantity[tierIndex];

            //for stats_only
            playerData.gemsSpent += gemsTradeQuantity[tierIndex]; 
        }
        playerData.saveData(); 
    }
    public void unlockLevelUpload() {
        if (playerData.money >= 2500 && !playerData.publishingUnlocked) {
            playerData.money -= 2500;
            playerData.publishingUnlocked = true;
            playerData.saveData(); 
        }
        checkNonConsumable(); 
    }
    public void purchase80Gems() {
        //iap
        playerData.gems += 80;
        playerData.saveData();
    }
    public void purchase300Gems() {
        //iap
        playerData.gems += 300;
        playerData.saveData();
    }
    public void purchase600Gems() {
        //iap
        playerData.gems += 600; 
        playerData.saveData();
    }
    public void purchaseUnlockAllLevels() {
        //iap
        if (!playerData.allLevelsUnlocked) {
            playerData.allLevelsUnlocked = true;
            playerData.money += 2000;
            playerData.saveData();
        }
        checkNonConsumable();
    }
}


