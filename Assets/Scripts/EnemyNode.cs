using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemySpawnInfo {
    public EnemySpawnInfo(int health, int quantity, float interval, int path, float finishDelay, bool isWave, bool isCamo) {
        this.a = health;
        this.b = quantity;
        this.c = interval;
        this.d = path;
        this.e = finishDelay;
        this.f = isWave;
        this.g = isCamo;
    }
    public int a; //health;
    public int b; //quantity;
    public float c; //interval;
    public int d; //path;
    public float e; //finishDelay;
    public bool f; //isWave;
    public bool g; //isCamo
}
[System.Serializable]
public class EnemySpawnData {
    public EnemySpawnInfo[] data;
    public int startingCash;
    public int startingHealth;
    public int[] towersAllowed;

    //not shown in level creator
    public int towerLimit; 
}

public class EnemyNode : MonoBehaviour {
    string mapJsonString;

    [HideInInspector]
    public int wave;
    public Builder builder;
    public EnemySpawnInfo[] enemyData;
    public GameObject[] enemies;
    public Transform[] path1;
    public Transform[] path2;
    public GameObject enemyPrefab;
    float initialDelay = 3f;

    int currentSpawnIndex;

    private int[] enemyHealths = { 1, 2, 3, 4, 5, 6, 8, 12, 22, 51, 100, 200, 500, 2000 };

    //which interval player is at 
    int currentIntervalIndex;

    private float intervalCountdown;

    EnemySpawnInfo createInfiniteWave() { 
        float difficulty = Mathf.Pow(wave + 2, 1.53f) / 4.5f + 1f;
        float rawEnemyHealth = Random.Range(Mathf.Pow(difficulty, 0.7f), difficulty);
        float originalRawEnemyHealth = rawEnemyHealth; 
        int enemyHealth = 0;

        //make levels harder towards the end
        if (rawEnemyHealth >= 26 && rawEnemyHealth < 43)
            rawEnemyHealth = 50;
        else if (rawEnemyHealth >= 43 && rawEnemyHealth < 65) 
            rawEnemyHealth = 100;
        else if (rawEnemyHealth >= 65 && rawEnemyHealth < 90)
            rawEnemyHealth = 200;
        else if (rawEnemyHealth >= 90 && rawEnemyHealth < 138)
            rawEnemyHealth = 500;
        else if (rawEnemyHealth >= 138)
            rawEnemyHealth = 2000; 

        for (int i = 0; i < enemyHealths.Length; i++) {
            if (i == enemyHealths.Length - 1) {
                enemyHealth = enemyHealths[i];
                break; 
            }

            if (enemyHealths[i] > rawEnemyHealth) {
                enemyHealth = enemyHealths[i - 1]; 
                break; 
            }
        }
        float morDifficulty; 
        if (rawEnemyHealth < 28)
            morDifficulty = difficulty - enemyHealth;
        else  {
            morDifficulty = difficulty - originalRawEnemyHealth; 
        }

        int spawnCount = (int)(10f * (1f + Mathf.Sqrt(morDifficulty)));
        float spawnInterval = 1f / (Mathf.Sqrt(morDifficulty + 1f) * 3);

        //todo: make camo determined by map
        bool isCamo = false;
        if (PlayerPrefs.GetInt("map") > 0 && wave > 20) { 
            int rand = Random.Range(0, 3);
            if (rand == 0)
                isCamo = true; 
        }

        //create a single wave 
        return new EnemySpawnInfo(enemyHealth, spawnCount, spawnInterval, Random.Range(0, 2), 6 + wave / 100, true, isCamo);
    }
    
    void LateStart() {
        builder.enemyNode = this;
        currentSpawnIndex = 0;
        wave = 1;

        enemies = new GameObject[1000];

        EnemySpawnData convertedJson;
        if (PlayerPrefs.GetInt("infiniteLevels") == 1) {
            //create initial level for endless mode
            convertedJson = new EnemySpawnData();
            convertedJson.data = new EnemySpawnInfo[] {createInfiniteWave()};
            convertedJson.startingCash = 2000 + PlayerPrefs.GetInt("map") * 500; 
            convertedJson.startingHealth = 100;
            convertedJson.towerLimit = 0; 

            switch (PlayerPrefs.GetInt("map")) { 
                case 0:
                    convertedJson.towersAllowed = new int[] {0, 1, 2, 3, 4, 5};
                    break;
                case 1:
                    convertedJson.towersAllowed = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8};
                    break; 
                default:
                    convertedJson.towersAllowed = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
                    break; 
            }
        } else {
            mapJsonString = PlayerPrefs.GetString("map" + PlayerPrefs.GetInt("map").ToString() + "Data");
            if (mapJsonString == "") {
                //load standard levels
                mapJsonString = PlayerPrefs.GetString("currentLevelData");
            }

            convertedJson = JsonUtility.FromJson<EnemySpawnData>(mapJsonString);
        }
        enemyData = convertedJson.data;
        builder.playerCash = convertedJson.startingCash;
        builder.playerHealth = convertedJson.startingHealth;
        builder.towerLimit = convertedJson.towerLimit;

        if (convertedJson.towersAllowed.Length > 0) {
            Vector3 firstButtonPosition = builder.buttonRects[0].transform.position;
            for (int i = 0; i < builder.buttonRects.Length; i++) {
                bool keepTower = false;
                foreach (int j in convertedJson.towersAllowed) {
                    //tower is kept
                    if (i == j) {
                        keepTower = true;
                        break;
                    }
                }
                //remove build tower button for buttonRects[i]
                if (!keepTower) {
                    builder.buttonRects[i].transform.position = new Vector3(0, 10000, 0);
                } else {
                    builder.buttonRects[i].transform.position = firstButtonPosition;
                    firstButtonPosition = new Vector3(firstButtonPosition.x + 126f * CustomFunctions.getUIScale(), firstButtonPosition.y, firstButtonPosition.z); 
                }
            } 
        } 
    }
    private bool started; 

    void newWaveTextDisplay () {
        builder.waveText.activateText("Wave " + wave.ToString());
    }
    void Update() {
        if (!started) {
            LateStart();
            started = true; 
        }
        if (initialDelay > 0) {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0)
                newWaveTextDisplay(); 
            return; 
        }
        if (builder.enemyNode == null)
            builder.enemyNode = this;
        if (currentSpawnIndex < enemyData.Length) {
            bool nothing = true;
            foreach (GameObject i in enemies) {
                if (i != null) {
                    nothing = false;
                    break;
                }
            }
            if (intervalCountdown > 0) {
                intervalCountdown -= Time.deltaTime;
                if (!nothing && currentIntervalIndex == 0) {
                    intervalCountdown = enemyData[currentSpawnIndex - 1].e;
                }
            } else {
                if (currentIntervalIndex == 0 && currentSpawnIndex != 0 && enemyData[currentSpawnIndex - 1].f) {
                    if (nothing && !builder.gameOver) { 
                        builder.playerCash += 100 + wave * 10;
                        wave++;
                        newWaveTextDisplay();
                    } else {
                        intervalCountdown = enemyData[currentSpawnIndex - 1].e;
                        return;
                    }
                }
                if (currentIntervalIndex < enemyData[currentSpawnIndex].b) {
                    intervalCountdown = enemyData[currentSpawnIndex].c;
                    spawnEnemy(new Vector3(0, 0.48f, 0), enemyData[currentSpawnIndex].d, enemyData[currentSpawnIndex].a, enemyData[currentSpawnIndex].g);
                    currentIntervalIndex++;
                } else {
                    intervalCountdown = enemyData[currentSpawnIndex].e;
                    currentSpawnIndex++;
                    currentIntervalIndex = 0;
                }

            }
        } else if (PlayerPrefs.GetInt("infiniteLevels") == 0) {
            //game is beaten if mode is not set to endless
            if (!builder.gameOver) {
                bool noEnemies = true;
                foreach (GameObject i in enemies) {
                    if (i != null) {
                        noEnemies = false;
                        break;
                    }
                }
                if (noEnemies) {
                    StartCoroutine(builder.winGameDelay(false));
                }
            }
        } else if (!builder.gameOver) {
            //generate new levels
            EnemySpawnInfo[] newEnemyData = new EnemySpawnInfo[enemyData.Length + 1];
            newEnemyData[newEnemyData.Length - 2] = enemyData[enemyData.Length - 1];
            newEnemyData[newEnemyData.Length - 1] = createInfiniteWave();
            enemyData = newEnemyData; 
        }
    }
    public GameObject spawnEnemy(Vector3 position, Transform[] path, int health, bool isCamo) {
        GameObject insItem = Instantiate(enemyPrefab, position, Quaternion.identity);
        insItem.GetComponent<Enemy>().path = path;
        insItem.GetComponent<Enemy>().enemyNode = this;
        insItem.GetComponent<Enemy>().health = health;
        insItem.GetComponent<Enemy>().isCamo = isCamo; 
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i] == null) {
                enemies[i] = insItem;
                break;
            }
        return insItem;
    }
    public GameObject spawnEnemy(Vector3 position, int path, int health, bool isCamo) {
        if (path == 1)
            return spawnEnemy(position, path1, health, isCamo);
        else 
            return spawnEnemy(position, path2, health, isCamo);
    }
}