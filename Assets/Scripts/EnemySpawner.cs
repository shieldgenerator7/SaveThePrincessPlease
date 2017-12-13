using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject enemyPrefab;//the prefab to spawn
    public float spawnRate = 60;//how many enemies it spawns per minute
    public GameObject defaultTarget;

    //2017-12-7: FUTURE CODE: allow multiple colliders for this purpose
    private BoxCollider2D spawnArea;//teh area the enemies can spawn in
    private float lastSpawnTime;
    private float spawnDelay;

	// Use this for initialization
	void Start () {
        spawnArea = GetComponent<BoxCollider2D>();
        spawnDelay = 60 / spawnRate;
        HealthPool targetHP = defaultTarget.GetComponentInParent<HealthPool>();
        if (targetHP)
        {
            targetHP.onDeath += stopSpawning;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > lastSpawnTime + spawnDelay)
        {
            lastSpawnTime = Time.time;
            GameObject enemy = GameObject.Instantiate(enemyPrefab);
            float newX = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            float newY = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            enemy.transform.position = new Vector2(newX, newY);
            enemy.GetComponent<SkeletonAI>().initialTarget = defaultTarget;
        }
	}

    void stopSpawning()
    {
        Destroy(this);
    }
}
