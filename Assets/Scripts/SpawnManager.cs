using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _enemyType;
    [SerializeField]
    private GameObject[] powerups;

    private bool _spawnWaveOne;
    private bool _spawnWaveTwo;
    private bool _spawnWaveThree;
    private bool _spawnBoss;

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    public void StartSpawning()
    {
        _spawnWaveOne = true;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false && _spawnWaveOne == true)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyType[0], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(4.0f);
        }

        while (_stopSpawning == false && _spawnWaveTwo == true)
        {
            int randomEnemy = Random.Range(0, 2);
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyType[randomEnemy], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(3.0f);
        }

        while (_stopSpawning == false && _spawnWaveThree == true)
        {
            int randomEnemy = Random.Range(0, 5);
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyType[randomEnemy], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2.0f);
        }

    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            int randomRoll = Random.Range(0, 100);
            int randomPowerup;
            if (randomRoll <= 80)
            {
                randomPowerup = Random.Range(0, 5);
                Instantiate(powerups[randomPowerup], posToSpawn, Quaternion.identity);
            }
            else if (randomRoll > 80)
            {
                randomPowerup = Random.Range(5, 8);
                Instantiate(powerups[randomPowerup], posToSpawn, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(5.0f, 8.0f));
        }
    }

    public void EmergencyAmmoSpawn()
    {
        Vector3 ammoSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
        Instantiate(powerups[3], ammoSpawn, Quaternion.identity);
    }

    public void WaveTwo()
    {
        _spawnWaveOne = false;
        _spawnWaveTwo = true;
        Debug.Log("Wave 2");
    }

    public void WaveThree()
    {
        _spawnWaveTwo = false;
        _spawnWaveThree = true;
        Debug.Log("Wave 3");
    }

    public void BossWave() 
    {
        _spawnWaveThree = false;
        StartCoroutine(SpawnBossRoutine());
        _spawnBoss = true;
        Debug.Log("Wave 4: Boss Wave");
    }

    IEnumerator SpawnBossRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        if (_spawnBoss == true)
        {
            Vector3 posToSpawn = new Vector3(0, 9, 0);
            GameObject boss = Instantiate(_enemyType[5], posToSpawn, Quaternion.identity);
            boss.transform.parent = _enemyContainer.transform;
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnBossDeath()
    {
        _stopSpawning = true;
    }
    
}
