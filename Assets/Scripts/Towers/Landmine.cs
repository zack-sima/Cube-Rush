using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : Tower {
    public GameObject explosionPrefab;
    float detonateDelay = 0.36f;
    public int[] damageByLevels; 

    public override void updateTowerLevels() {
        base.updateTowerLevels(); 
    }
    void Update() {
        towerUpdate();
        if (currentTarget != null) {
            detonateDelay -= Time.deltaTime;
            if (detonateDelay <= 0) {
                //detonate mine
                GameObject insItem = Instantiate(explosionPrefab, new Vector3(transform.position.x, 0.39f, transform.position.z), Quaternion.identity);
                insItem.GetComponent<Explosion>().builder = builder;
                insItem.GetComponent<Explosion>().impactRange = 3f; 
                insItem.GetComponent<Explosion>().damage = damageByLevels[level - 1];
                insItem.transform.localScale = new Vector3(insItem.transform.localScale.x * 1.3f, insItem.transform.localScale.y * 1.3f, insItem.transform.localScale.z * 1.3f);

                insItem.transform.Rotate(-90, 0, 0);
                Destroy(gameObject);
            }
        }
    }
}