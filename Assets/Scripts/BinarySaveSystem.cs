using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySaveSystem {
    public static void SaveData(PlayerData playerData) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/playerdata.sav", FileMode.Create);

        BinaryData data = new BinaryData(playerData);

        bf.Serialize(stream, data);
        stream.Close();
    }
    //add integer parameter for different maps
    public static BinaryData LoadData() {
        if (File.Exists(Application.persistentDataPath + "/playerdata.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/playerdata.sav", FileMode.Open);
            BinaryData data = bf.Deserialize(stream) as BinaryData;
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}
[System.Serializable]
public class BinaryData {
    public int money;
    public int[] levelsUnlocked;
    public int gems; 
    public int[] towerLevelsUnlocked;
    public int[] powerUps;
    public bool publishingUnlocked;
    public bool allLevelsUnlocked;
    public int endlessRecord;
    public int towersPlaced;
    public int towersSold;
    public int damageDealt;
    public int gamesPlayed;
    public int coinsSpent;
    public int gemsSpent;

    public BinaryData(PlayerData playerData) {
        money = playerData.money;
        levelsUnlocked = playerData.levelsUnlocked;
        towerLevelsUnlocked = playerData.towerLevelsUnlocked;
        gems = playerData.gems;
        powerUps = playerData.powerUps;
        publishingUnlocked = playerData.publishingUnlocked;
        allLevelsUnlocked = playerData.allLevelsUnlocked;
        endlessRecord = playerData.endlessRecord;
        towersPlaced = playerData.towersPlaced;
        towersSold = playerData.towersSold;
        damageDealt = playerData.damageDealt;
        gamesPlayed = playerData.gamesPlayed;
        coinsSpent = playerData.coinsSpent;
        gemsSpent = playerData.gemsSpent; 
    }
}