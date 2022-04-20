using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MapEditor : MonoBehaviour {
    //the two arrays should be of the same length and correlate with actual cubes in game scene
    public Color[] cubeColors;
    public int[] cubeHealths;
    public Sprite[] cubeSprites; 
    public Image cubeColorDisplay;
    public Text pathText;
    public Text quantityText;
    public Text intervalText;
    public Text finishDelayText;
    public Text waveText;
    public Toggle waveEndToggle;
    public Toggle levelEndToggle;
    public Toggle isCamoToggle; 
    public InputField startingCashInput;
    public InputField startingHealthInput;
    public SpriteToggle[] towerToggles;
    public Transform finishDelayUI;
    Vector3 originalWaveDelayUIPosition;

    public Image[] saveButtons;

    public int path;
    public int quantity;
    public int interval;
    public int finishDelay;

    //total index
    public int saveButtonsIndex;

    //smallwaveindex is the index for the number of cube types sent after the first one in a designated wave
    public int smallWaveIndex;

    //this is the index for cubehealths 
    public int cubeType;

    public string makeJson() {
        string jsonString = "{\"data\":[";
        int i = 0;
        while (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i - 1).ToString() + "LevelEnd") == 0 && PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Quantity") >= 1) {
            if (i > 0)
                jsonString += ",\n";

            jsonString += "{" +
                "\"a\":" + cubeHealths[PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "CubeType")] + "," +
                "\"b\":" + PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Quantity") + "," +
                "\"c\":" + PlayerPrefs.GetFloat("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Interval") + "," +
                "\"d\":" + PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Path") + "," +
                "\"e\":" + PlayerPrefs.GetFloat("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "FinishDelay") + ",";
            if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "WaveEnd") == 1)
                jsonString += "\"f\":true,";
            else
                jsonString += "\"f\":false,";
            if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "IsCamo") == 1)
                jsonString += "\"g\":true";
            else
                jsonString += "\"g\":false";
            jsonString += "}";
            i++;
        }
        jsonString += "],\n\"startingCash\":" + PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingCash") + ", \"startingHealth\":" + PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingHealth") +
            ", \"towersAllowed\":[";
        string str = "";
        string prefsString = PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("level") + "TowersAllowed");
        foreach (char j in prefsString) {
            str += j;
            if (j.ToString() != prefsString.Substring(prefsString.Length - 1)) {
                str += ",";
            }
        }
        jsonString += str + "]}";

        //only for testing in editors
        CustomFunctions.CopyToClipboard(jsonString);

        return jsonString;
    }
    bool stopSaving = false; 
    public void resetLevel() {
        stopSaving = true; 
        int i = 0;
        while (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Quantity") >= 1) {
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "CubeType", 0);
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Quantity", 0);
            PlayerPrefs.SetFloat("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Interval", 0);
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "Path", 0);
            PlayerPrefs.SetFloat("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "FinishDelay", 0);
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "WaveEnd", 0);
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "IsCamo", 0);
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "LevelEnd", 0);
            i++;
        }
        PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("level") + "TowersAllowed", "");
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + "StartingCash", 0);
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + "StartingHealth", 0);

        PlayerPrefs.SetInt("currentCustomLevel", PlayerPrefs.GetInt("level"));
        SceneManager.LoadScene(3);
    }
    public void uploadDataEvent() {
        changeScene(4);
    }

    public void indexAdd() {
        if (!levelEndToggle.isOn) {
            smallWaveIndex++;
            updateDisplays();
        }
    }
    public void indexSub() {
        smallWaveIndex--;
        if (smallWaveIndex < 0)
            smallWaveIndex = 0;
        updateDisplays();
    }
    public void finishDelayAdd(int index) {
        finishDelay += index;
    }
    public void finishDelaySub(int index) {
        finishDelay -= index;
        if (finishDelay < 0)
            finishDelay = 0;
    }
    public void pathAdd() {
        if (path < 2)
            path++;
    }
    public void pathSub() {
        if (path > 1)
            path--;
    }
    public void quantityAdd(int amount) {
        quantity += amount;
    }
    public void quantitySub(int amount) {
        quantity -= amount;
        if (quantity < 1)
            quantity = 1;
    }
    public void intervalAdd(int amount) {
        interval += amount;
    }
    public void intervalSub(int amount) {
        interval -= amount;
        if (interval < 3)
            interval = 3;
    }
    public void typeAdd() {
        if (cubeType < cubeHealths.Length - 1)
            cubeType++;
    }
    public void typeSub() {
        if (cubeType > 0)
            cubeType--;
    }
    public void changeLevel(int index) {
        saveInputs();
        PlayerPrefs.SetInt("level", index + saveButtonsIndex * 4);
        smallWaveIndex = 0;
        updateDisplays();
        //retrive toggle data
        foreach (SpriteToggle i in towerToggles)
            i.isOn = false;
        foreach (char i in PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("level") + "TowersAllowed")) {
            towerToggles[int.Parse(i.ToString())].isOn = true;
        }
        startingCashInput.text = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingCash").ToString();
        startingHealthInput.text = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingHealth").ToString();
        saveInputs();
    }
    public void changeSaveButtons(bool add) {
        if (add) {
            saveButtonsIndex++;
        } else {
            if (saveButtonsIndex > 0)
                saveButtonsIndex--;
        }
    }
    public void changeScene(int index) {
        saveInputs(); 
        PlayerPrefs.SetInt("currentCustomLevel", PlayerPrefs.GetInt("level"));
        PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("level") + "Json", makeJson());
        print(PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("level") + "Json"));
        SceneManager.LoadScene(index);
    }
    void updateWaveCount() {
        int wave = 1, smallWave = 1;
        int i = 0;
        while (true) {
            if (i >= smallWaveIndex)
                break;
            if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + (i).ToString() + "WaveEnd") == 1) {
                wave++;
                smallWave = 1;
            } else
                smallWave++;
            i++;
        }
        waveText.text = "Wave " + wave.ToString() + "-" + smallWave.ToString();
    }
    void saveInputs() {
        int outVar;
        if (!int.TryParse(startingCashInput.text, out outVar) || int.Parse(startingCashInput.text) < 100) {
            startingCashInput.text = "1000";
        }
        if (!int.TryParse(startingHealthInput.text, out outVar) || int.Parse(startingHealthInput.text) < 1) {
            startingHealthInput.text = "100";
        }
        
        string str = "";
        for (int i = 0; i < towerToggles.Length; i++) {
            if (towerToggles[i].isOn)
                str += i.ToString();
        }
        PlayerPrefs.SetString("map" + PlayerPrefs.GetInt("level") + "TowersAllowed", str);
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + "StartingCash", int.Parse(startingCashInput.text));
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + "StartingHealth", int.Parse(startingHealthInput.text));
    }
    void updateDisplays() {
        updateWaveCount();

        //retrive data
        if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "WaveEnd") == 1) {
            waveEndToggle.isOn = true;
        } else
            waveEndToggle.isOn = false;
        if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "LevelEnd") == 1) {
            levelEndToggle.isOn = true;
        } else
            levelEndToggle.isOn = false;
        if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "IsCamo") == 1) {
            isCamoToggle.isOn = true;
        } else 
            isCamoToggle.isOn = false;
        if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Path") == 0) {
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Path", 1);
        }
        path = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Path");
        quantity = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Quantity");
        interval = (int)(PlayerPrefs.GetFloat("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Interval") * 1000);
        finishDelay = (int)(PlayerPrefs.GetFloat("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "FinishDelay") * 100);
        cubeType = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "CubeType");

        if (interval % 10 > 0) {
            interval /= 10;
            interval++;
        } else
            interval /= 10;

        if (quantity <= 0)
            quantity = 10;
        if (interval == 0)
            interval = 60;
        if (interval < 3)
            interval = 3;
        if (finishDelay % 10 > 0) {
            finishDelay /= 10;
            finishDelay *= 10;
            finishDelay += 10;
        }
        deltaWaveEnd = waveEndToggle.isOn;
        updateFinishDelay();
    }
    void updateFinishDelay() {
        if (waveEndToggle.isOn) {
            finishDelayUI.position = originalWaveDelayUIPosition;
        } else {
            finishDelayUI.position = new Vector3(0f, 8000f, 0f);
            finishDelay = 0;
        }
    }

    bool deltaWaveEnd = true;
    void Start() {
        deltaWaveEnd = waveEndToggle.isOn;
        originalWaveDelayUIPosition = finishDelayUI.position;
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("currentCustomLevel"));
        saveButtonsIndex = PlayerPrefs.GetInt("level") / 4;
        startingCashInput.text = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingCash").ToString();
        startingHealthInput.text = PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("level") + "StartingHealth").ToString();

        foreach (SpriteToggle i in towerToggles)
            i.isOn = false;
        foreach (char i in PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("level") + "TowersAllowed")) {
            towerToggles[int.Parse(i.ToString())].isOn = true;
        }
        updateDisplays();
        saveInputs();
    }

    //make sure not all towers are disabled
    public bool checkTowerToggles () {
        foreach (SpriteToggle i in towerToggles) {
            if (i.isOn) {
                return true;
            }
        }
        return false; 
    }
    void Update() {
        if (stopSaving)
            return; 
        if (!checkTowerToggles()) {
            towerToggles[0].isOn = true; 
        }
        if (deltaWaveEnd != waveEndToggle.isOn) {
            updateFinishDelay();
            if (waveEndToggle.isOn) {
                finishDelay = 600;
            }
        }
        deltaWaveEnd = waveEndToggle.isOn;

        int index = 0;
        foreach (Image i in saveButtons) {
            i.transform.GetChild(0).GetComponent<Text>().text = (saveButtonsIndex * 4 + index + 1).ToString();
            if (PlayerPrefs.GetInt("level") == index + saveButtonsIndex * 4) {
                i.color = Color.green;
            } else
                i.color = Color.white;
            index++;
        }

        if (cubeSprites[cubeType] != null) {
            cubeColorDisplay.sprite = cubeSprites[cubeType];
        } else {
            cubeColorDisplay.sprite = null; 
        }
        cubeColorDisplay.color = cubeColors[cubeType];
        pathText.text = path.ToString();
        intervalText.text = (interval / 100f) + "s";
        finishDelayText.text = (finishDelay / 100f) + "s";

        quantityText.text = quantity.ToString();
        //save data 
        if (waveEndToggle.isOn) {
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "WaveEnd", 1);
        } else
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "WaveEnd", 0);
        if (levelEndToggle.isOn) {
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "LevelEnd", 1);
        } else
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "LevelEnd", 0);
        if (isCamoToggle.isOn) {
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "IsCamo", 1);
        } else
            PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "IsCamo", 0);

        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Path", path);
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Quantity", quantity);
        PlayerPrefs.SetFloat("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "Interval", interval / 100f);
        PlayerPrefs.SetFloat("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "FinishDelay", finishDelay / 100f);
        PlayerPrefs.SetInt("map" + PlayerPrefs.GetInt("level") + smallWaveIndex.ToString() + "CubeType", cubeType);
    }
}
