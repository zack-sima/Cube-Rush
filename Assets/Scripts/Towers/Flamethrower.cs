using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Tower {
    public GameObject firePrefab;
    public Transform shootAnchor;

    [HideInInspector]
    public float shootSpeed;
    public float[] shootSpeedByLevels;
    public int[] damageByLevels;
    private float shootDelay;

    public override void updateTowerLevels() {
        base.updateTowerLevels();
        shootSpeed = shootSpeedByLevels[level - 1];
        if (level == 1) {
        } else if (level == 2) {
        } else if (level == 3) {
            bottom.material = builder.blackMaterial;
        }
    }
    void Update() {
        towerUpdate();

        if (shootDelay > 0)
            shootDelay -= Time.deltaTime;
        else {
            shootDelay = shootSpeed + shootDelay;
            if (isShooting && (!currentTarget.GetComponent<Enemy>().isCamo || canShootCamo)) {
                GameObject insItem = Instantiate(firePrefab, shootAnchor.position, shootAnchor.rotation);
                insItem.GetComponent<Explosion>().damage = 0;
                insItem.GetComponent<Explosion>().builder = builder;

                builder.audioManager.Play("Flamethrower", 1.1f);

                //real explosion
                insItem = Instantiate(firePrefab, shootAnchor.position, shootAnchor.rotation);
                insItem.transform.Translate(Vector3.forward * 0.9f);
                insItem.GetComponent<Explosion>().damage = damageByLevels[level - 1];
                insItem.GetComponent<Explosion>().builder = builder;
                insItem.GetComponent<Explosion>().impactRange = 1.2f; 
                insItem.GetComponent<ParticleSystem>().Stop();
                insItem.GetComponent<Explosion>().explodeDelay = 0.25f; 
            }
        }
    }
}
