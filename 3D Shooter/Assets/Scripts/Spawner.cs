using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    [HideInInspector]
    public bool allEnemiesDefeated = false;

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timebetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), Quaternion.identity) as Enemy;
            if (spawnedEnemy.type == Enemy.Type.Roaming)
            {
                spawnedEnemy.transform.Rotate(Vector3.up, (float)System.Math.PI / 4.0f);
            }
            spawnedEnemy.onDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
            NextWave();
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
        else allEnemiesDefeated = true;
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timebetweenSpawns;
    }
}
