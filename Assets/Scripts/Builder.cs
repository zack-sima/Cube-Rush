using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Builder : MonoBehaviour {
    public PlayerData playerData; 
    public TextFade waveText; 
    public float gameSpeed = 1;

    public int playerCash;
    public int playerHealth;
    public Text playerCashDisplay;
    public Text playerHealthDisplay;
    public Text gameSpeedDisplay;

    [HideInInspector]
    public EnemyNode enemyNode;

    [HideInInspector]
    public AudioManager audioManager;

    [HideInInspector]
    public float damageBuffTimer;

    public GameObject explosionPrefab;

    //this is in order of tower id's
    public Transform buildingUI;
    public Transform pauseUI;
    public Transform gameOverUI;
    public Transform instantsUI;  
    public Text gameOverText;
    public RectTransform towerUpgradeUI;
    public RectTransform[] uiRects;
    public RectTransform[] buttonRects;
    public Text[] instantsTexts;

    private int towersBuilt; 

    //default is 0 (if 0 then ignore and delete limiting text) 
    [HideInInspector]
    public int towerLimit;
    public Text towerLimitText; 

    //access this array with tower id's
    public GameObject[] towers;

    //track the towers built during the session
    [HideInInspector]
    public GameObject[] towersInGame; 

    //game maps
    public GameObject[] maps;

    public bool isOn; 
    private bool inUI; 

    //only one prototype instantiated at start function; its transparency is changed
    public Material transGreenMaterial, transRedMaterial;
    public GameObject prototypePrefab;
    private GameObject instantiatedPrototype;
    public Transform rangeRing;

    //store mouse positions so when building cannot be built these will be checked for buildability
    Vector2 deltaMousePosition;
    Vector3 deltaBuilderPosition;

    [HideInInspector]
    public GameObject selectedTower;
    public Text sellTowerText, upgradeTowerText, towerNameText, toggleAimText;

    public bool gameOver; 

    //only build if there is no prototype existent
    private bool isBuilding;
    private int constructionId;
    private Vector2 originalBuildingUIPosition, originalTowerUIPosition, originalPauseUIPosition;

    //materials, use in children; created here because multiple children access same materials
    public Material woodMaterial, greyMaterial, blackMaterial, darkGreenMaterial, redMaterial, veryBlackMaterial, brownMaterial;

    //don't allow builds within 0.2 seconds (to prevent the player from accidentally clicking the build prefabs)
    private float buildDelay;

    private float unloadAssetsTimer = 0; 

    public void initiatePrototype(int id) {
        //prevent mouse from being here
        deltaMousePosition = new Vector2(-1000f, -1000f);
        deltaBuilderPosition = new Vector3(-1000f, -1000f, -1000f);
        constructionId = id;
        isBuilding = true;
        buildDelay = 0.2f * gameSpeed;
        instantiatedPrototype.transform.localScale = new Vector3(towers[id].GetComponent<BoxCollider>().size.x, 0.8f, towers[id].GetComponent<BoxCollider>().size.z);
    }
    public void toggleInstants() {
        //first object is text and won't be affected
        for (int i = 1; i < instantsUI.childCount; i++) {
            if (instantsUI.GetChild(i).position.y < 6000) {
                instantsUI.GetChild(i).Translate(0, 6000, 0);
            } else {
                instantsUI.GetChild(i).Translate(0, -6000, 0);
            }
        }
    }
    public void toggleUI() {
        isOn = !isOn;
        if (isOn)
            selectedTower = null;
    }
    void Start() {
        playerData.gamesPlayed++;
        playerData.saveData(); 
        towerLimit = -1; 
        towersInGame = new GameObject[200]; 
        audioManager = GetComponent<AudioManager>(); 
        GameObject insItem = Instantiate(prototypePrefab, Vector3.zero, Quaternion.identity);
        insItem.GetComponent<Renderer>().enabled = false;
        instantiatedPrototype = insItem;
        originalBuildingUIPosition = buildingUI.position;
        originalTowerUIPosition = towerUpgradeUI.position;
        originalPauseUIPosition = pauseUI.position;
        toggleInstants(); 
        resumeGame();

        //check map from choice
        int c = 0;
        foreach (GameObject i in maps) {
            if (c == PlayerPrefs.GetInt("map")) {
                foreach (Renderer j in maps[c].transform.GetChild(0).GetComponentsInChildren<Renderer>()) {
                    if (j != null)
                        j.enabled = true;
                }
            } else {
                Destroy(maps[c].gameObject);
            }
            c++;
        }
    }
    public void sellTower() {
        if (selectedTower != null) {
            if (playerCash + selectedTower.GetComponent<Tower>().sellProfit >= 0) {
                playerCash += selectedTower.GetComponent<Tower>().sellProfit;

                //make sure the tower is not a tree
                if (selectedTower.GetComponent<Tower>().sellProfit > 0) {
                    playerData.towersSold++;
                    playerData.saveData(); 
                }


                Destroy(selectedTower.gameObject);
            }
        }
    }
    public void upgradeTower() {
        if (selectedTower != null) {
            Tower myTower = selectedTower.GetComponent<Tower>();
            if (playerData.towerLevelsUnlocked[findTowerIndexByName(myTower.towerName)] >= myTower.level && myTower.level < myTower.upgradeCosts.Length && myTower.upgradeCosts[myTower.level] <= playerCash) {
                playerCash -= myTower.upgradeCosts[myTower.level];
                myTower.level++;
            }
        }
    }
    public void toggleTowerAim() {
        if (selectedTower != null) {
            if (selectedTower.GetComponent<Tower>().shootMode == ShootMode.First)
                selectedTower.GetComponent<Tower>().shootMode = ShootMode.Last; 
            else if (selectedTower.GetComponent<Tower>().shootMode == ShootMode.Last)
                selectedTower.GetComponent<Tower>().shootMode = ShootMode.Strong;
            else
                selectedTower.GetComponent<Tower>().shootMode = ShootMode.First;
        }
    }
    public IEnumerator winGameDelay(bool gameOver) {
        for (float i = 0; i < 0.1f; i += Time.deltaTime) {
            yield return null; 
        }
        if (!this.gameOver)
            endGame(gameOver); 
    }
    public void endGame(bool gameOver) {
        //used in enemynode scripts 

        pauseUI.position = new Vector3(0, 10000, 0);
        gameOverUI.position = originalPauseUIPosition;
        if (gameOver) {
            playerHealth = 0;
            playerHealthDisplay.text = "0";
            gameOverText.text = "Game Over";
        } else {
            gameOverText.text = "Level Complete!";
            playerHealthDisplay.text = playerHealth.ToString();

            if (PlayerPrefs.GetInt("currentLevel") != -1) {
                //unlock next level (if the level played is part of campaign)
                if (playerData.levelsUnlocked[PlayerPrefs.GetInt("map")] == PlayerPrefs.GetInt("currentLevel")) {
                    playerData.levelsUnlocked[PlayerPrefs.GetInt("map")]++;
                    if (PlayerPrefs.GetInt("map" + PlayerPrefs.GetInt("map").ToString() + "LevelQuantity") <= playerData.levelsUnlocked[PlayerPrefs.GetInt("map")]) {
                        gameOverText.text = "New Map Unlocked!";
                    }
                    playerData.money += (int)(100f * ((3f + PlayerPrefs.GetInt("map")) / 3f));  
                } else {
                    //get less stuff for completing again
                    playerData.money += 10; 
                }
                playerData.saveData();
            }

        }
        if (isOn)
            toggleUI();
        isBuilding = false;
        this.gameOver = true;
    }
    public void usePowerUp (int powerUpIndex) { 
        if (playerData.powerUps[powerUpIndex] > 0) {
            playerData.powerUps[powerUpIndex]--; 
        } else
            return; 
        if (powerUpIndex == 0) {
            //more health
            playerHealth += 200; 
        } else if (powerUpIndex == 1) {
            //more power (temporary, +10 secs to buff)
            damageBuffTimer += 10f; 
        } else if (powerUpIndex == 2) {
            //instantkillall
            GameObject insItem = Instantiate(explosionPrefab, new Vector3(0, 0, 10), explosionPrefab.transform.rotation);
            insItem.transform.localScale = new Vector3(3.8f, 3.8f, 3.8f);
            insItem.GetComponent<Explosion>().impactRange = 18f; 
            insItem.GetComponent<Explosion>().damage = 169; 
            insItem.GetComponent<Explosion>().builder = this; 
        } else if (powerUpIndex == 3) {
            //instantfreezeall
            foreach (GameObject i in enemyNode.enemies) {
                if (i != null) {
                    i.GetComponent<Enemy>().freezeDelay += 7f; 
                }
            }
        }
        playerData.saveData(); 
    }
    public void pauseGame() {
        Time.timeScale = 0.000001f; 
        pauseUI.position = originalPauseUIPosition;
    }
    public void changeGameSpeed() {
        if (gameSpeed <= 0)
            gameSpeed = 0.5f;
        else if (gameSpeed <= 0.5f)
            gameSpeed = 1f;
        else if (gameSpeed <= 1f)
            gameSpeed = 1.5f;
        else if (gameSpeed <= 1.5f)
            gameSpeed = 0f;
        Time.timeScale = gameSpeed;
        if (Time.timeScale == 0f)
            Time.timeScale = 0.000001f; 
        gameSpeedDisplay.text = gameSpeed.ToString() + "x";
    }
    public void resumeGame() {
        Time.timeScale = gameSpeed;
        pauseUI.position = new Vector3(0, 7000f, 0);
    }
    public void changeScene(int index) {
        if (PlayerPrefs.GetInt("infiniteLevels") == 1) {
            int moreWavesSurvived = enemyNode.wave - playerData.endlessRecord;
            if (moreWavesSurvived > 0) {
                print("saveEndless");
                //new record
                playerData.money += moreWavesSurvived * 10;

                playerData.endlessRecord = enemyNode.wave;
            }
            playerData.money += (int)(enemyNode.wave / 10f);
        }
        playerData.saveData(); 
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }
    void Update() {
        if (towerLimit > 0) {
            int towerCount = 0;
            for (int i = 0; i < towersInGame.Length; i++) {
                if (towersInGame[i] != null && towersInGame[i].GetComponent<Tower>().towerName != "Landmine") {
                    towerCount++; 
                }
            }
            towersBuilt = towerCount; 
            towerLimitText.text = "Towers:   " + towerCount.ToString() + "/" + towerLimit.ToString(); 
        } else if (towerLimitText != null && towerLimit != -1) {
            Destroy(towerLimitText.gameObject);
        }
        if (gameSpeed == 0f || Time.timeScale < 0.1f)
            Resources.UnloadUnusedAssets(); 
        unloadAssetsTimer -= Time.deltaTime;
        if (unloadAssetsTimer <= 0) {
            unloadAssetsTimer = 0.1f;
            Resources.UnloadUnusedAssets(); 
        }
        if (playerHealth <= 0 && !gameOver) {
            endGame(true); 
        }
        if (isBuilding) {
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        } else {
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
        }
        if (gameOver)
            return;

        if (damageBuffTimer > 0) {
            damageBuffTimer -= Time.deltaTime;
        } else if (damageBuffTimer < 0)
            damageBuffTimer = 0; 
        int ind = 0;
        foreach (RectTransform i in buttonRects) {
            i.GetChild(1).GetComponent<Text>().text = "$" + calculateTowerBaseCost(ind); 
            ind++;
        }

        playerCashDisplay.text = playerCash.ToString();
        playerHealthDisplay.text = playerHealth.ToString();
        Rect myRect = new Rect(towerUpgradeUI.position.x, towerUpgradeUI.position.y, towerUpgradeUI.rect.width, towerUpgradeUI.rect.height);
        if (Input.GetMouseButtonUp(0) && !inUI) {
            if (!myRect.Contains(new Vector2(Input.mousePosition.x + myRect.width / 2f, Input.mousePosition.y + myRect.height / 2f))) {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                if (Physics.Raycast(ray, out hit, 100f)) {
                    if (hit.collider.GetComponent<Tower>() != null) {
                        selectedTower = hit.collider.gameObject;
                        isOn = false;
                    } else {
                        selectedTower = null;
                    }
                }
            }
        }

        if (selectedTower == null) {
            towerUpgradeUI.position = new Vector3(0, 6000, 0);
            rangeRing.GetComponent<Canvas>().enabled = false;
        } else {
            Tower myTower = selectedTower.GetComponent<Tower>();

            rangeRing.GetComponent<Canvas>().enabled = true;
            rangeRing.position = new Vector3(selectedTower.transform.position.x, 3f, selectedTower.transform.position.z + 1.6f);
            rangeRing.transform.localScale = new Vector3(myTower.range / 550f, myTower.range / 550f, 1f);
            towerUpgradeUI.position = originalTowerUIPosition;


            //add levels later
            sellTowerText.text = "Sell\n\n$" + myTower.sellProfit;
            towerNameText.text = "Level " + myTower.level + "\n" + myTower.towerName;
            toggleAimText.text = "Target: ";

            if (myTower.shootMode == ShootMode.First) {
                toggleAimText.text += "first";
            } else if (myTower.shootMode == ShootMode.Last)
                toggleAimText.text += "last";
            else
                toggleAimText.text += "strong";

            if (playerData.towerLevelsUnlocked[findTowerIndexByName(myTower.towerName)] < myTower.level && myTower.level < myTower.upgradeCosts.Length) {
                upgradeTowerText.text = "Upgrade\n\nLocked";
            }else if (myTower.level < myTower.upgradeCosts.Length) {
                upgradeTowerText.text = "Upgrade\n\n$" + myTower.upgradeCosts[myTower.level];
            } else {
                upgradeTowerText.text = "Max level";
            }
        }

        for (int i = 0; i < instantsTexts.Length; i++) {
            instantsTexts[i].text = "x" + playerData.powerUps[i]; 
        }

        if (!isOn) {
            buildingUI.position = new Vector3(0, 6000, 0);
        } else {
            buildingUI.position = originalBuildingUIPosition;
        }

        if (isBuilding) {
            isOn = true;
            if (buildDelay > 0)
                buildDelay -= Time.deltaTime;
            instantiatedPrototype.GetComponent<Renderer>().enabled = true;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(ray, out hit, 100f, 9)) {
                instantiatedPrototype.transform.position = new Vector3(hit.point.x, 0.6f, hit.point.z);
            }

            bool canPlace = true;
            if (towers[constructionId].GetComponent<Landmine>() == null) {
                foreach (Collider i in Physics.OverlapBox(instantiatedPrototype.transform.position, new Vector3(instantiatedPrototype.transform.localScale.x / 2f - 0.1f, 4, instantiatedPrototype.transform.localScale.z / 2f - 0.1f))) {
                    if (i.GetComponent<Tower>() != null || i.gameObject.name == "Path" || i.gameObject.name == "Tree") {
                        canPlace = false;
                        break;
                    }
                }
            } else {
                canPlace = false;
                foreach (Collider i in Physics.OverlapBox(instantiatedPrototype.transform.position, new Vector3(0, 4, 0))) {
                    if (i.gameObject.name == "Path") {
                        canPlace = true;
                    }
                    if (i.GetComponent<Tower>() != null) {
                        canPlace = false;
                        break;
                    }
                }
            }

            if (canPlace) {
                instantiatedPrototype.GetComponent<Renderer>().material = transGreenMaterial;
                deltaMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                deltaBuilderPosition = instantiatedPrototype.transform.position;
            } else {
                if (!CustomFunctions.isMobile || Vector2.Distance(deltaMousePosition, new Vector2(Input.mousePosition.x, Input.mousePosition.y)) > (38f * CustomFunctions.getUIScale())) {
                    instantiatedPrototype.GetComponent<Renderer>().material = transRedMaterial;
                    deltaMousePosition = new Vector3(-1000f, -1000f, -1000f);
                } else {
                    instantiatedPrototype.GetComponent<Renderer>().material = transGreenMaterial;
                    instantiatedPrototype.transform.position = deltaBuilderPosition;
                    canPlace = true;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (canPlace && buildDelay <= 0) {
                    //build tower
                    isBuilding = false;
                    buildTower();
                } else {
                    isBuilding = false;
                }
            }
        } else {
            instantiatedPrototype.GetComponent<Renderer>().enabled = false;
            int index = 0;
            inUI = false;
            foreach (RectTransform i in buttonRects) {
                Rect rect = new Rect(i.position.x, i.position.y, i.rect.width, i.rect.height);
                if (rect.Contains(new Vector2(Input.mousePosition.x + rect.width / 2f, Input.mousePosition.y + rect.height / 2f))) {
                    inUI = true;

                    //check that the player has enough money
                    if (Input.GetMouseButton(0)) {
                        if (playerCash >= calculateTowerBaseCost(index) && (towerLimit <= 0 || towers[index].GetComponent<Tower>().towerName == "Landmine" || towersBuilt < towerLimit)) {
                            initiatePrototype(index);
                            break;
                        }
                    }
                }
                index++;
            }
            index = 0;
            foreach (RectTransform i in uiRects) {
                Rect rect = new Rect(i.position.x, i.position.y, i.rect.width, i.rect.height);
                if (rect.Contains(new Vector2(Input.mousePosition.x + rect.width / 2f, Input.mousePosition.y + rect.height / 2f))) {
                    inUI = true;
                }
                index++;
            }

        }

    }
    public int findTowerIndexByName(string towerName) {
        int towerIndex = 0;
        for (int i = 0; i < towers.Length; i++) {
            if (towers[i].GetComponent<Tower>().towerName == towerName) {
                towerIndex = i;
                break;
            }
        }
        return towerIndex; 
    }
    public int calculateTowerBaseCost(int towerId) {
        int c = towers[towerId].GetComponent<Tower>().upgradeCosts[0];
        float multiplier = 1f;
        foreach (GameObject i in towersInGame) {
            if (i != null && i.GetComponent<Tower>().towerName == towers[towerId].GetComponent<Tower>().towerName) {
                multiplier += 0.25f; 
            }
        }
        return (int)(c * multiplier); 
    }
    void buildTower() {
        playerData.towersPlaced++;
        playerData.saveData(); 
        GameObject insItem = Instantiate(towers[constructionId], new Vector3(instantiatedPrototype.transform.position.x, 0, instantiatedPrototype.transform.position.z), Quaternion.identity);
        insItem.GetComponent<Tower>().enemyNode = enemyNode;
        insItem.GetComponent<Tower>().builder = this;

        playerCash -= calculateTowerBaseCost(constructionId); 

        for (int i = 0; i < towersInGame.Length; i++) {
            if (towersInGame[i] == null) {
                towersInGame[i] = insItem; 
                break; 
            }
        }
    }
}
