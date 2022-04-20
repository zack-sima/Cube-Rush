using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class LevelSelect : MonoBehaviour {PlayerData playerData; 
    public GameObject lockPrefab; 
    public GameObject[] buttons; 
    TextAsset[][] dataByLevels; 
    public TextAsset[] map1Levels, map2Levels, map3Levels, map4Levels; 

    public void changeScene(int level) {
        //level starts at 0
        if (level == -1)
            SceneManager.LoadScene(1);
        else if (level < dataByLevels[PlayerPrefs.GetInt("map")].Length) {
            PlayerPrefs.SetInt("currentLevel", level);
            PlayerPrefs.SetString("currentLevelData", dataByLevels[PlayerPrefs.GetInt("map")][level].text);
            PlayerPrefs.SetInt("infiniteLevels", 0);
            SceneManager.LoadScene(2);
        } else {
            //infinite levels

            PlayerPrefs.SetInt("infiniteLevels", 1);
            SceneManager.LoadScene(2);
        }
    }
    void Start() {
        playerData = GetComponent<PlayerData>(); 
        dataByLevels = new TextAsset[][] {
            map1Levels, map2Levels, map3Levels, map4Levels
        };

        for (int i = buttons.Length - 1; i > dataByLevels[PlayerPrefs.GetInt("map")].Length; i--) {
            Destroy(buttons[i].gameObject);
        }
        for (int i = 0; i <= dataByLevels[PlayerPrefs.GetInt("map")].Length; i++) {
            if (i > playerData.levelsUnlocked[PlayerPrefs.GetInt("map")]) {
                //lock level
                buttons[i].GetComponent<Button>().enabled = false;
                Image im = buttons[i].GetComponent<Image>();
                im.color = new Color(im.color.r, im.color.g, im.color.b, 0.3f);
                Text t = buttons[i].transform.GetChild(0).GetComponent<Text>();
                t.enabled = false;
                Instantiate(lockPrefab, im.transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
            } else if (i == dataByLevels[PlayerPrefs.GetInt("map")].Length) {
                Text t = buttons[i].transform.GetChild(0).GetComponent<Text>();
                t.text = "∞";
                t.fontSize = (int)(t.fontSize * 1.5f); 
            }
        }

        for (int i = 0; i < dataByLevels.Length; i++) { 
            PlayerPrefs.SetInt("map" + i.ToString() + "LevelQuantity", dataByLevels[i].Length);
        }
    }
}
