using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootMode {First, Last, Strong}

public class Tower : MonoBehaviour {
    //nearby radar towers will search for other towers 
    [HideInInspector]
    public bool canShootCamo;
    private float shootCamoTimer; 
    public bool isTree; 
    public string towerName; 
    [HideInInspector]
    public Builder builder; 
    [HideInInspector]
    public GameObject currentTarget; 
    [HideInInspector]
    public EnemyNode enemyNode;
    [HideInInspector]
    public int sellProfit;

    //this is the transform object that rotates (cannon turret, crossbow body, etc)
    public Transform rotationAnchor;
    public float rotationOffset;
    public float[] rangeByLevels; 
    [HideInInspector]
    public float range;
    [HideInInspector]
    public ShootMode shootMode; 
    public Renderer bottom; 

    //first element is the purchase cost
    public int[] upgradeCosts;
    [HideInInspector]
    public int level;
    protected int deltaLevel;

    //child class can change this if needed to stop rotationAnchor from rotation to face target
    protected bool stopAiming; 

    //this determines whether or not a target is in range and trigger animations 
    public bool isShooting;

    public void changeCamo () {
        canShootCamo = true;
        shootCamoTimer = 1f; 
    }
    public virtual void updateTowerLevels () {
        //override in child classes
        if (!isTree)
            range = rangeByLevels[level - 1];
    }
    public virtual void Start () {
        //override in child classes
        level = 1; 
        updateTowerLevels();
        if (isTree)
            towerUpdate(); 
    }
    public void findTarget () {
        //recalculate target every interval
        isShooting = false;
        float furthestDistance = 0;
        if (shootMode == ShootMode.First) {
            //aim at first target
            for (int i = 0; i < enemyNode.enemies.Length; i++) {
                if (enemyNode.enemies[i] != null && (!enemyNode.enemies[i].GetComponent<Enemy>().isCamo || canShootCamo) && Vector3.Distance(transform.position, enemyNode.enemies[i].transform.position) < range && enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled > furthestDistance) {
                    furthestDistance = enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled; 
                    currentTarget = enemyNode.enemies[i];
                    isShooting = true; 
                }
            }
        } else if (shootMode == ShootMode.Last){
            //aim at last target 
            furthestDistance = Mathf.Infinity;
            for (int i = 0; i < enemyNode.enemies.Length; i++) {
                if (enemyNode.enemies[i] != null && Vector3.Distance(transform.position, enemyNode.enemies[i].transform.position) < range && enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled < furthestDistance) {
                    furthestDistance = enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled;
                    currentTarget = enemyNode.enemies[i];
                    isShooting = true; 
                }
            }
        } else {
            //aim at strongest target
            int largestHealth = 0;
            furthestDistance = 0; 
            for (int i = 0; i < enemyNode.enemies.Length; i++) {
                if (enemyNode.enemies[i] != null && Vector3.Distance(transform.position, enemyNode.enemies[i].transform.position) < range && (enemyNode.enemies[i].GetComponent<Enemy>().health > largestHealth || (enemyNode.enemies[i].GetComponent<Enemy>().health >= largestHealth && enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled > furthestDistance))) {
                    largestHealth = enemyNode.enemies[i].GetComponent<Enemy>().health;
                    furthestDistance = enemyNode.enemies[i].GetComponent<Enemy>().distanceTravelled;
                    currentTarget = enemyNode.enemies[i]; 
                    isShooting = true;
                }
            }
        }

        if (currentTarget != null) {
            if (currentTarget.GetComponent<Enemy>().isCamo && !canShootCamo) {
                currentTarget = null;
                return; 
            }
            aimAtTarget();
        }
    }
    void aimAtTarget () {
        if (!stopAiming) {
            rotationAnchor.LookAt(new Vector3(currentTarget.transform.position.x, rotationAnchor.position.y, currentTarget.transform.position.z));
            rotationAnchor.transform.Rotate(0, rotationOffset, 0);
        }
    }
    //called by children
    public void towerUpdate() {
        if (canShootCamo) {
            shootCamoTimer -= Time.deltaTime;
            if (shootCamoTimer <= 0) {
                canShootCamo = false;
                if (currentTarget != null && currentTarget.GetComponent<Enemy>().isCamo) {
                    currentTarget = null; 
                }
            }
        }
        int cost = 0; 
        for (int i = level - 1; i >= 0; i--) {
            cost += upgradeCosts[i]; 
        }
        if (!isTree)
            sellProfit = (int)(cost * 0.8f);
        else
            sellProfit =/* upgradeCosts[0]*/ -60; 

        if (deltaLevel != level) {
            updateTowerLevels(); 
            deltaLevel = level;
        }
        if (!isTree) {
            if (currentTarget != null) {
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentTarget.transform.position.x, currentTarget.transform.position.z)) > range + 0f) {
                    currentTarget = null;
                } else {
                    isShooting = true;
                    aimAtTarget();
                }
            }
            findTarget();
        }
    }
}
