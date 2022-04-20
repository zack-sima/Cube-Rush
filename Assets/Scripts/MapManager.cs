using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MapManager : MonoBehaviour {
    public Toggle useDataToggle;
    public GameObject useCustomText, useCustomToggle;
    PlayerData playerData; 
    public int mapIndex;
    public Transform mapParent; 

    public void changeScene (int index) {
        PlayerPrefs.SetInt("map", mapIndex);
        if (index == 0) {
            SceneManager.LoadScene(0);
            return; 
        }
        if (useDataToggle.isOn) {
            //set to -1 so it is different from actual levels
            PlayerPrefs.SetInt("currentLevel", -1);
            //set to 0 so enemynode does not use infinite levels algorithm 
            PlayerPrefs.SetInt("infiniteLevels", 0);
            SceneManager.LoadScene(index);
        } else {
            SceneManager.LoadScene(5);
        }
    }
    void updateMap () {
        for (int i = 0; i < mapParent.childCount; i++) {
            if (i == mapIndex) {
                foreach (Renderer j in mapParent.GetChild(i).GetChild(0).GetComponentsInChildren<Renderer>()) {
                    if (j != null)
                        j.enabled = true;
                }
            } else {
                foreach (Renderer j in mapParent.GetChild(i).GetChild(0).GetComponentsInChildren<Renderer>()) {
                    if (j != null)
                        j.enabled = false;
                }
            }
        }
    }
    public void changeMap(bool addIndex) {
        if (addIndex) {
            if (mapIndex < mapParent.childCount - 1)
                mapIndex++;
        } else {
            if (mapIndex > 0)
                mapIndex--;
        }
        updateMap(); 
    }

    void Start() {
        playerData = GetComponent<PlayerData>();
        if (playerData.levelsUnlocked[2] == -1) {
            Destroy(useCustomText);
            Destroy(useCustomToggle); 
        }
        mapIndex = PlayerPrefs.GetInt("map");
        if (PlayerPrefs.GetInt("useData") == 1)
            useDataToggle.isOn = true;
        else
            useDataToggle.isOn = false; 
        updateMap(); 
    }

    void Update() {
        if (useDataToggle.isOn) {
            PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("map").ToString() + "Data", PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("currentCustomLevel") + "Json"));
            PlayerPrefs.SetInt("useData", 1);
        } else {
            PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("map").ToString() + "Data", "");
            PlayerPrefs.SetInt("useData", 0); 
        }
    }
}
