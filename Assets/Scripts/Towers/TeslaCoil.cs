using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : Tower {
    public Transform teslaTop; 
    public Renderer laserBeam;
    public Transform laserBeamAnchor; 

    [HideInInspector]
    public float shootSpeed;
    [HideInInspector]
    public int damage; 
    public float[] shootSpeedByLevels;
    public int[] damageByLevels; 

    private float shootDelay;
    public Material[] lightningMaterials; 
    private Transform previousTargetTransform; 

    public override void Start() {
        base.Start();
        shootDelay = shootSpeedByLevels[level - 1] - 0.2f; 
        laserBeam.enabled = false;
        //shootMode = ShootMode.Strong; 
    }
    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
        shootSpeed = shootSpeedByLevels[level - 1];
        damage = damageByLevels[level - 1];

        if (level == 1) {
            //do nothing here
        } else if (level == 2) {

        } else if (level == 3) {
            bottom.material = builder.blackMaterial;
        }
    }
    void Update() {
        towerUpdate();

        if (shootDelay > 0) {
            shootDelay -= Time.deltaTime;
            if (shootDelay > shootSpeed - 0.18f) {
                laserBeam.enabled = true;
                laserBeam.material = lightningMaterials[Random.Range(0, lightningMaterials.Length - 1)]; 
            } else {
                laserBeam.enabled = false; 
            }
        } else {
            if (isShooting && currentTarget != null) {
                previousTargetTransform = currentTarget.transform; 
                shootDelay = shootSpeed;
                builder.audioManager.Play("Zap", 1);
                findTarget(); 

                laserBeam.enabled = true;

                currentTarget.GetComponent<Enemy>().takeDamage(damage);
                int extraDamage = 0;
                if (currentTarget.GetComponent<Enemy>().health < 0)
                    extraDamage = 0 - currentTarget.GetComponent<Enemy>().health;
                currentTarget.GetComponent<Enemy>().updateHealth();
                builder.playerCash += damage - extraDamage;

                laserBeamAnchor.localScale = new Vector3(laserBeamAnchor.localScale.x, laserBeamAnchor.localScale.y, Vector3.Distance(laserBeamAnchor.position, previousTargetTransform.position));
                laserBeamAnchor.LookAt(previousTargetTransform);
            }
        }
        if (previousTargetTransform != null) {
            laserBeamAnchor.localScale = new Vector3(laserBeamAnchor.localScale.x, laserBeamAnchor.localScale.y, Vector3.Distance(laserBeamAnchor.position, previousTargetTransform.position));
            laserBeamAnchor.LookAt(previousTargetTransform);
        }
        if (builder.gameSpeed == 0f) {
            laserBeam.enabled = false;
        }
    }
}
