using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatling : Tower {
    public GameObject dartPrefab;
    public Transform tubesAnchor; 
    public Transform gatlingTop; 
    public Transform shootAnchor; 

    [HideInInspector]
    public float shootSpeed;
    public float[] shootSpeedByLevels;
    public int[] damageByLevels; 

    //gatling spinning animation speed
    private float spinSpeed;

    public Renderer gatlingTopRender; 

    private float shootDelay;
    private float recoil;

    public override void Start() {
        base.Start();
        shootDelay = shootSpeedByLevels[level - 1]; 
    }
    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shootSpeed = shootSpeedByLevels[level - 1];
        if (level == 1) {
            spinSpeed = 430;
        } else if (level == 2) {
            spinSpeed = 530;
            gatlingTopRender.material = builder.darkGreenMaterial;
        } else if (level == 3) {
            spinSpeed = 530;
            bottom.material = builder.blackMaterial; 
        }
    }
    void Update() {
        tubesAnchor.Rotate(0, 0, Time.deltaTime * spinSpeed);
        towerUpdate();

        if (recoil > 0)
            recoil -= Time.deltaTime * 2.8f; 
        else if (recoil < 0)
            recoil = 0; 

        if (shootDelay > 0)
            shootDelay -= Time.deltaTime;
        else {
            shootDelay = shootSpeed + shootDelay; 
            if (isShooting) {
                builder.audioManager.Play("Gatling", 3);
                recoil = 1; 
                GameObject insItem = Instantiate(dartPrefab, shootAnchor.position, gatlingTop.rotation);
                insItem.transform.Rotate(0, 180, 0);
                insItem.GetComponent<CollidingProjectile>().damage = damageByLevels[level - 1];
                insItem.GetComponent<CollidingProjectile>().parent = this;
                insItem.GetComponent<CollidingProjectile>().currentTarget = currentTarget;
                insItem.GetComponent<CollidingProjectile>().isActive = true;
                if (level >= 3) {
                    insItem.GetComponent<CollidingProjectile>().maxHealth = 2;
                    insItem.GetComponent<CollidingProjectile>().health = 2;
                } else {
                    insItem.GetComponent<CollidingProjectile>().maxHealth = 2;
                    insItem.GetComponent<CollidingProjectile>().health = 2;
                }
                insItem.transform.localScale = new Vector3(insItem.transform.localScale.x / 1.2f, insItem.transform.localScale.y / 1.2f, insItem.transform.localScale.z / 1.2f);
            }
        }
        gatlingTop.localPosition = Vector3.zero;
        gatlingTop.transform.Translate(Vector3.forward * recoil * 0.1f);
    }
}
