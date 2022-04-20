using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Tower {
    public Transform sparkyTop;
    public Renderer laserBeam;
    public Renderer lightningIcon; 

    [HideInInspector]
    public float shootSpeed;
    public float[] shootSpeedByLevels;
    public int[] damageByLevels; 

    private float shootDelay;
    private float recoil; 

    public override void Start() {
        base.Start(); 
        laserBeam.enabled = false;
    }
    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shootSpeed = shootSpeedByLevels[level - 1];
        if (level == 1) {
            //do nothing here
        } else if (level == 2) {
            lightningIcon.enabled = true; 
        } else if (level == 3) {
            bottom.material = builder.blackMaterial;
        }
    }
    void Update() {
        towerUpdate();

        if (recoil > 0)
            recoil -= Time.deltaTime * 3;
        else if (recoil < 0)
            recoil = 0;

        if (shootDelay > 0) {
            shootDelay -= Time.deltaTime;
            if (recoil <= 0f) {

                stopAiming = false; 
                laserBeam.enabled = false; 
            } else {
                laserBeam.material.color = new Color(255f, 255f, 255f, recoil * 1f);
            }
        } else {
            if (isShooting && builder.gameSpeed > 0f && currentTarget != null && (!currentTarget.GetComponent<Enemy>().isCamo || canShootCamo)) {
                shootDelay = shootSpeed;
                findTarget(); 
                builder.GetComponent<AudioManager>().Play("LaserGun", 7);
                //shoot laser
                laserBeam.enabled = true;
                recoil = 1;
                stopAiming = true; 

                foreach (Collider i in Physics.OverlapBox(laserBeam.transform.position, new Vector3(laserBeam.transform.lossyScale.x / 2f, laserBeam.transform.lossyScale.y / 2f, laserBeam.transform.lossyScale.z / 2f), laserBeam.transform.rotation)) { 
                    if (i.GetComponent<Enemy>() != null) {
                        i.GetComponent<Enemy>().takeDamage(damageByLevels[level - 1]);
                        int extraDamage = 0;
                        if (i.GetComponent<Enemy>().health < 0)
                            extraDamage = 0 - i.GetComponent<Enemy>().health;
                        i.GetComponent<Enemy>().updateHealth();
                        builder.playerCash += damageByLevels[level - 1] - extraDamage;
                    }
                }
            }
        }
        sparkyTop.localPosition = Vector3.zero;
        sparkyTop.transform.Translate(Vector3.forward * recoil * 0.1f);
    }
}
