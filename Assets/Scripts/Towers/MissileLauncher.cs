using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Tower {
    public GameObject missilePrefab;
    public Transform missileSpawnAnchor;
    public Transform leftDoor, rightDoor; 

    [HideInInspector] 
    public float shotDelay;
    public float[] shotDelayByLevels;
    public int[] damageByLevels;

    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shotDelay = shotDelayByLevels[level - 1];
        if (level == 1) {
            //nothing here
        } else if (level == 2) {
            leftDoor.GetChild(0).GetComponent<Renderer>().material = builder.veryBlackMaterial;
            rightDoor.GetChild(0).GetComponent<Renderer>().material = builder.veryBlackMaterial;
        } else if (level == 3) {
            bottom.material = builder.blackMaterial; 
        }
    }
    public override void Start() {
        base.Start();
        //shootMode = ShootMode.Strong; 
    }
    void Update() {
        towerUpdate();

        if (shotDelay > 0)
            shotDelay -= Time.deltaTime;
        else {
            if (isShooting) {
                spawnMissile();
                shotDelay = shotDelayByLevels[level - 1];
                StartCoroutine(openHatch());
            }
        }
    }
    void spawnMissile () {
        GameObject insItem = Instantiate(missilePrefab, missileSpawnAnchor.position, missilePrefab.transform.rotation);
        insItem.GetComponent<Missile>().target = currentTarget.GetComponent<Enemy>();
        insItem.GetComponent<Missile>().parent = this;
        insItem.GetComponent<Missile>().damage = damageByLevels[level - 1]; 
    }
    IEnumerator openHatch () { 
        for (float i = 1; i > 0; i -= Time.deltaTime * 4.8f) {
            leftDoor.transform.localScale = new Vector3(1, i, 1);
            rightDoor.transform.localScale = new Vector3(1, i, 1);
            yield return null; 
        }

        //pause for launch
        for (float i = 0; i < 0.3f; i += Time.deltaTime) {
            yield return null; 
        }

        for (float i = 0; i < 1; i += Time.deltaTime * 4.8f) {
            leftDoor.transform.localScale = new Vector3(1, i, 1);
            rightDoor.transform.localScale = new Vector3(1, i, 1);
            yield return null;
        }
        leftDoor.transform.localScale = new Vector3(1, 1, 1);
        rightDoor.transform.localScale = new Vector3(1, 1, 1);
    }
}

