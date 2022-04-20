using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public PlayerData playerData; 
    public Button levelCreatorButton, onlineLevelsButton;

    public void loadScene(int index) {
        if (index == 3 && playerData.levelsUnlocked[2] == -1 || index == 6 && playerData.levelsUnlocked[1] == -1) {
            if (index == 3) {
                if (levelCreatorButton.transform.GetChild(0).GetComponent<Text>().text == "Finish Map 2") 
                    levelCreatorButton.transform.GetChild(0).GetComponent<Text>().text = "Level Creator"; 
                else
                    levelCreatorButton.transform.GetChild(0).GetComponent<Text>().text = "Finish Map 2"; 
            } else if (index == 6) {
                if (onlineLevelsButton.transform.GetChild(0).GetComponent<Text>().text == "Finish Map 1")
                    onlineLevelsButton.transform.GetChild(0).GetComponent<Text>().text = "Online Levels";
                else
                    onlineLevelsButton.transform.GetChild(0).GetComponent<Text>().text = "Finish Map 1"; 
            }
            return; 
        }
        SceneManager.LoadScene(index);
    }

    private void Start() {
        Caching.ClearCache();
        if (playerData.levelsUnlocked[1] > -1) {
            if (onlineLevelsButton.transform.childCount == 3)
                Destroy(onlineLevelsButton.transform.GetChild(2).gameObject);
            onlineLevelsButton.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            onlineLevelsButton.transform.GetChild(1).GetComponent<Image>().enabled = true; 
            onlineLevelsButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            onlineLevelsButton.GetComponent<Image>().color = Color.white; 
        }
        if (playerData.levelsUnlocked[2] > -1) {
            if (levelCreatorButton.transform.childCount == 3)
                Destroy(levelCreatorButton.transform.GetChild(2).gameObject);
            levelCreatorButton.transform.GetChild(1).GetComponent<Image>().color = Color.white;
            levelCreatorButton.transform.GetChild(1).GetComponent<Image>().enabled = true; 
            levelCreatorButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            levelCreatorButton.GetComponent<Image>().color = Color.white;
        }
    }
}
