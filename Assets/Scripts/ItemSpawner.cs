using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Wave
{
    
    
    public GameObject[] typeOfEnemies;
    public float spawnInterval = 3;
}

public class ItemSpawner : MonoBehaviour
{
    public Wave[] Items;
    public Transform[] spawnPoints;
   

    private Wave currentWave;
    private int currentWaveNumber;
    private float nextSpawnTime;
    private bool canSpawn = true;
    



    private void Update()
    {
        currentWave = Items[currentWaveNumber];
        SpawnWave();

    }



    void SpawnWave()
    {
        if (canSpawn && nextSpawnTime < Time.time)
        {
            GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(randomEnemy, randomPoint.position, Quaternion.identity);
            nextSpawnTime = Time.time + currentWave.spawnInterval;
        }

    }
}

