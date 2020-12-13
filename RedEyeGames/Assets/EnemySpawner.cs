using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //public EnemyBehavior enemyBehavior;

    bool stopSpawner = false;

    [SerializeField] private GameObject HeroInMap; // so that the enemy can have the hero set in their spawn


    [SerializeField] private GameObject EnemyTypePrefab; // the type of enemy that needs to be spawned (MUST BE PREFAB)
    private GameObject newEnemy;

    //[SerializeField] private GameObject spawnPosition; // the transform postion that the enemy is going to be spawned at

    // or could just ask for x & y & z postion of placement too ?

    [SerializeField] private float spwanDelayTime = 10; // in seconds
    private float nextSpawnTime = 0; // current time till next spawn


    [SerializeField] private bool haveMaxEnemys = true;
    [SerializeField] private float maxEnemys = 3; // max num of enemys spawned
    private float currentEnemys = 0;

    //private bool maxEnemysTrue = false;

    /*[SerializeField] private GameObject leftOfSpawnerCheck;
    [SerializeField] private GameObject rightOfSpawnerCheck;*/


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if timer only
        if(/*!haveMaxEnemys && */spawnEnemyTimer() && currentEnemys < maxEnemys && !stopSpawner) // if dont check for if max enemys is met (since the timer isnt changed if there is a max of enemys)
        {
            // Start countdown for next spawntime
            if (!haveMaxEnemys || currentEnemys < maxEnemys && haveMaxEnemys) // if there isn't a max of enemys            or             if the max num of enemys havent been reached AND there is a max of enemys
                nextSpawnTime = Time.time + spwanDelayTime;

            Spawn();
        }

        // if there is a max amount of enemys   !!!!! is this needed in update?
        /*else if (haveMaxEnemys)
        {

        }*/
    }

    // this is the code that times / checks when the enemy should spawn
    /*private bool ifSpawnEnemy()
    {
        // will also need to check if there is any enemy currently in same position?

        return true;
    }*/

    // called by Enemy that was spawned
    public void ifEnemyDied()
    {
        // decrease max amount of enemys
        currentEnemys--;

        // start the timer IF next spawn time is less then current time (if timer isn't already going) 
        if (nextSpawnTime < Time.time) 
        {
            nextSpawnTime = Time.time + spwanDelayTime;
        }
    }



    // could have 2 different enemy spawn types

    // timer (enemy spawns every x seconds)

    // max (enemy spawns after certain amount of time)
    private bool spawnEnemyTimer()
    {
        return Time.time >= nextSpawnTime;
    }

    // it was disucssed that then answer should be both ...
    



    private void Spawn()
    {
        // increase the number of current enemys
        currentEnemys++;

        /*// Start countdown for next spawntime
        if(!haveMaxEnemys || currentEnemys < maxEnemys) // if there isn't a max of enemys            or             if the max num of enemys havent been reached 
            nextSpawnTime = Time.time + spwanDelayTime;*/

        // Infinate loop if timer isnt stopped (when the max amount of enemys is reached)
        /*else
        {
            maxEnemysTrue = true;
        }*/

        newEnemy = Instantiate(EnemyTypePrefab, transform.position, transform.rotation);

        // need to set hero Component
        newEnemy.GetComponent<EnemyNormalBehavior>().setHero(HeroInMap);

        // set the enemySpawner Component
        newEnemy.GetComponent<EnemyController>().enemySpawner = this.gameObject;
    }

    public void StopSpawner()
    {
        stopSpawner = true;
    }

    public void StartSpawner()
    {
        stopSpawner = false;
    }
}
