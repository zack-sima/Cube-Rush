using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Tower {
    public GameObject cannonballPrefab;
    public Transform cannonTop;
    public Transform shootAnchor;
    public Renderer cannonRotater; 

    [HideInInspector] 
    public float shotDelay;
    public float[] shotDelayByLevels; 
    private float recoil;

    //whether the cannon is shooting (moving back) or recovering from shot (moving to front)
    private bool isReturning;
    private bool coroutineStarted;
    private bool shootCycleHasStarted; 

    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shotDelay = shotDelayByLevels[level - 1];
        if (level == 1) {
            //nothing here
        }else if (level == 2) {
            cannonRotater.material = builder.greyMaterial; 
        } else if (level == 3) {
            bottom.material = builder.blackMaterial; 
        }
    }
    void Update() {
        //foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(10, 10, 10))) { }

        towerUpdate();

        if (isReturning) {
            recoil -= Time.deltaTime * 1.3f;
            if (recoil < 0) {
                recoil = 0;
                if (!coroutineStarted)
                    StartCoroutine(delayBetweenShots());
            }
        }else {
            if (shootCycleHasStarted || isShooting) {
                shootCycleHasStarted = true;
                recoil += Time.deltaTime * 13f;
                if (recoil > 0.7f) {
                    recoil = 0.7f;
                    isReturning = true;

                    //shoot cannonball here
                    GameObject insItem = Instantiate(cannonballPrefab, shootAnchor.position, cannonTop.rotation);
                    insItem.transform.Rotate(0, 270, 0);
                    insItem.GetComponent<CollidingProjectile>().parent = this;
                    insItem.GetComponent<CollidingProjectile>().currentTarget = currentTarget;
                    //insItem.GetComponent<CollidingProjectile>().damage = 1;
                    shootCycleHasStarted = false;
                }
            }
        }
        //if (!isShooting) {
        //    if (recoil > 0) {
        //        recoil -= Time.deltaTime;
        //    }
        //}
        cannonTop.localPosition = Vector3.zero;
        cannonTop.Translate(Vector3.right * recoil * 0.3f);
    }
    IEnumerator delayBetweenShots() {
        coroutineStarted = true;
        for (float i = 0; i < shotDelay; i += Time.deltaTime) {
            yield return null;
        }
        isReturning = false;
        coroutineStarted = false;

        //yield return new WaitForSeconds(0);
    }
}


