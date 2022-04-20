using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    //skiptele beginning if it is spawned mid-path
    public bool isCamo;
    [HideInInspector]
    public bool skipTeleport;
    public Material[] levelColors;
    public float[] levelSpeeds;
    [HideInInspector]
    public EnemyNode enemyNode;
    [HideInInspector]
    public float distanceTravelled;
    public Transform[] path;
    public int health;
    [HideInInspector]
    public float freezeDelay;
    Material currentMaterial;
    float speed = 1;
    [HideInInspector]
    public int pathIndex = 1;

    void Start() {
        updateHealth();
        if (skipTeleport)
            return;
        if (path.Length > 0) {
            transform.position = new Vector3(path[0].position.x, transform.position.y, path[0].position.z);
            if (health <= levelColors.Length) {
                findColor();
            }
        } else {
            Destroy(gameObject);
        }
    }
    public void takeDamage(int damage) {
        int myDamage = damage;
        if (health - damage < 0)
            myDamage += health - damage;

        enemyNode.builder.playerData.damageDealt += myDamage;
        enemyNode.builder.playerData.saveData(); 

        int deltaHealth = health;
        if (enemyNode.builder.damageBuffTimer > 0)
            health -= damage * 2;
        else
            health -= damage; 
        if ((deltaHealth > 8 && health <= 8) || (deltaHealth > 22 && health <= 22) || (deltaHealth > 51 && health <= 51) || (deltaHealth > 100 && health <= 100) || (deltaHealth > 200 && health <= 200)) {
            StartCoroutine(spawnDuplicate(0.038f, transform.position, pathIndex, distanceTravelled));
        }
    }
    IEnumerator spawnDuplicate(float movementDelay, Vector3 position, int myPathIndex, float myDistance) {
        for (float i = 0; i < movementDelay; i += Time.deltaTime)
            yield return null;
        GameObject insItem = enemyNode.spawnEnemy(position, path, health, isCamo);
        insItem.GetComponent<Enemy>().pathIndex = myPathIndex;
        insItem.GetComponent<Enemy>().distanceTravelled = myDistance;
        insItem.GetComponent<Enemy>().freezeDelay = freezeDelay;
        insItem.GetComponent<Enemy>().skipTeleport = true;
        insItem.GetComponent<Enemy>().isCamo = isCamo;
    }
    void findColor() {
        Renderer r = GetComponent<Renderer>();
        for (int i = health - 1; i < levelColors.Length; i++) {
            if (levelColors[i] != null) {
                speed = levelSpeeds[i];
                r.material = new Material(levelColors[i]);
                if (isCamo) {
                    r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.3f);
                    CustomFunctions.SetMaterialRenderingMode(r.material, CustomFunctions.RenderingMode.Transparent);
                }
                currentMaterial = new Material(r.material); 
                break;
            }
        }
    }

    void Update() {
        float newSpeed = speed;
        if (freezeDelay > 0) {
            freezeDelay -= Time.deltaTime;
            if (freezeDelay > 3f) {
                newSpeed *= 0f; 
            } else {
                newSpeed *= 0.4311000f; 
            }
            if (!isCamo)
                GetComponent<Renderer>().material.color = new Color(currentMaterial.color.r + 0.3f, currentMaterial.color.g + 0.3f, currentMaterial.color.b + 0.3f, 1);
            else
                GetComponent<Renderer>().material.color = new Color(currentMaterial.color.r + 0.3f, currentMaterial.color.g + 0.3f, currentMaterial.color.b + 0.3f, 0.3f);
        } else {
            findColor(); 
        }
        transform.localScale = new Vector3(0.4f + distanceTravelled / 800f, 0.4f + distanceTravelled / 800f + Mathf.Sqrt(health) / 200f, 0.4f + distanceTravelled / 800f);
        transform.LookAt(new Vector3(path[pathIndex].position.x, transform.position.y, path[pathIndex].position.z));
        transform.Translate(Vector3.forward * Time.deltaTime * newSpeed);
        distanceTravelled += Time.deltaTime * newSpeed;
        transform.rotation = Quaternion.identity;

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[pathIndex].position.x, path[pathIndex].position.z)) < 0.02f * speed * enemyNode.builder.gameSpeed) {
            transform.position = new Vector3(path[pathIndex].position.x, transform.position.y, path[pathIndex].position.z);
            pathIndex++;
            if (pathIndex > path.Length - 1) {
                //player loses health when a cube passes the finish line
                enemyNode.builder.playerHealth -= health;
                enemyNode.builder.playerCash += health; 

                for (int i = 0; i < enemyNode.enemies.Length; i++)
                    if (enemyNode.enemies[i] == gameObject) {
                        enemyNode.enemies[i] = null;
                        break;
                    }
                Destroy(gameObject);
            }
        }
    }
    public void updateHealth() {
        if (health <= 0) {
            for (int i = 0; i < enemyNode.enemies.Length; i++)
                if (enemyNode.enemies[i] == gameObject) {
                    enemyNode.enemies[i] = null;
                    break;
                }
            Destroy(gameObject);
        } else {
            if (health <= levelColors.Length) {
                findColor();
            }
        }
    }
}
