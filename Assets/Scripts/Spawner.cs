using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> ObjectsToSpawn = new List<GameObject>();
    public bool IsTimer, Israndomized;
    public float TimetoSpawn;
    private float CurrentTimetoSpawn;

    void Start()
    {
        
    }
    void Update()
    {
        if (IsTimer)
        {
            UpdateTimer();
        }
    }

    public void SpawnObject()
    {
        int index = Israndomized ? Random.Range(0, ObjectsToSpawn.Count) : 0;
        if (ObjectsToSpawn.Count > 0)
        {
            Instantiate(ObjectsToSpawn[index], transform.position, transform.rotation );
        }
    }

    private void UpdateTimer()
    {
        if (CurrentTimetoSpawn > 0)
        {
            CurrentTimetoSpawn -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
            CurrentTimetoSpawn = TimetoSpawn;
        }
    }
}
