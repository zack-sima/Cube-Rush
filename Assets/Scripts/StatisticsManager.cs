using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class StatisticsManager : MonoBehaviour {
	public Text text;
	PlayerData playerData; 
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    private void Start() {
        playerData = GetComponent<PlayerData>();
    }
    private void Update() {
        text.text = playerData.towersPlaced + "\n" + playerData.towersSold + "\n" +
            playerData.damageDealt + "\n" + playerData.gamesPlayed + "\n" + playerData.coinsSpent + "\n" +
            playerData.gemsSpent + "\n" + playerData.endlessRecord + " waves";
    }
}
