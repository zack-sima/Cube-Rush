using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingProjectile : MonoBehaviour {
    //this is determined by firing mechanism; if active, will launch and allow collision detections
    public Vector3 hitbox; 
    [HideInInspector]
    public Tower parent;
    [HideInInspector]
    public GameObject currentTarget; 
    [HideInInspector]
    public bool isActive;
    public bool hasExplosion;
    public bool canChaseTarget; 
    public GameObject explosionPrefab; 
    public int health;
    public int damage; 
    public int maxHealth; 
    public float timeAlive;
    public float speed; 
    private GameObject[] hitEnemies; 

    void Start() {
        //this is set to heatlh because dart can only hit the same number of enemies as it has health
        hitEnemies = new GameObject[health];
        maxHealth = health;
        if (damage == 0)
            damage = 1; 
    }

    void Update() {
        timeAlive += Time.deltaTime;
        if ((timeAlive > 3 && isActive) || parent == null)
            Destroy(gameObject);

        if (isActive) {
            if (canChaseTarget && currentTarget != null && health == maxHealth && parent != null &&
                Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentTarget.transform.position.x, currentTarget.transform.position.z)) > 0.8f &&
                Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(parent.transform.position.x, parent.transform.position.z)) < 2f) {
                Quaternion originalRot = transform.rotation; 
                transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));
                Quaternion newRot = transform.rotation;
                transform.rotation = originalRot; 
                float originalAngle = Quaternion.Angle(originalRot, newRot);

                if (originalAngle > 10f) {
                    transform.Rotate(0, Time.deltaTime * 300f, 0);
                    float newAngle = Quaternion.Angle(transform.rotation, newRot);
                    if (newAngle > originalAngle) {
                        transform.Rotate(0, Time.deltaTime * -600f, 0);
                    }
                }
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(hitbox.x, hitbox.y, hitbox.z))) {
                if (health <= 0)
                    break; 
                if (i.GetComponent<Enemy>() != null) {
                    bool previouslyHit = false; 
                    foreach (GameObject h in hitEnemies) {
                        if (h != null && h == i.gameObject) {
                            previouslyHit = true; 
                            break; 
                        }
                    }
                    if (!previouslyHit) {
                        if (!hasExplosion || health > 1) {
                            i.GetComponent<Enemy>().takeDamage(damage);
                            int extraDamage = 0;
                            if (i.GetComponent<Enemy>().health < 0)
                                extraDamage = 0 - i.GetComponent<Enemy>().health;
                            i.GetComponent<Enemy>().updateHealth();
                            parent.builder.playerCash += damage - extraDamage;
                        }
                        if (!hasExplosion)
                            parent.builder.GetComponent<AudioManager>().Play("Pop", 3f);
                        health--;
                        for (int j = 0; j < hitEnemies.Length; j++) {
                            if (hitEnemies[j] == null) {
                                hitEnemies[j] = i.gameObject;
                                break;
                            }
                        }
                    }
                }
            }
            if (health <= 0) {
                if (hasExplosion) {
                    GameObject insItem = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    insItem.transform.Rotate(-90, 0, 0);
                    if (insItem.GetComponent<Explosion>() != null) {
                        insItem.GetComponent<Explosion>().builder = parent.builder;
                        insItem.GetComponent<Explosion>().impactRange = 2.6f;
                        insItem.GetComponent<Explosion>().damage = damage;
                    } else {
                        insItem.GetComponent<SnowExplosion>().builder = parent.builder;
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
