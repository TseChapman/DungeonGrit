using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //public EnemyBehavior enemyBehavior;


    [SerializeField] private GameObject HeroInMap; // so that the enemy can have the hero set in their spawn
    [SerializeField] private GameObject EnemyTypePrefab; // the type of enemy that needs to be spawned (MUST BE PREFAB)
    private GameObject newEnemy;

    //[SerializeField] private GameObject spawnPosition; // the transform postion that the enemy is going to be spawned at

    // or could just ask for x & y & z postion of placement too ?


    [SerializeField] private float spwanDelayTime = 10; // in seconds
    private float nextSpawnTime; // current time till next spawn

    [SerializeField] private float maxEnemys = 0; // max num of enemys spawned


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnEnemyTimer())
        {
            Spawn();
        }
    }

    // this is the code that times / checks when the enemy should spawn
    private bool ifSpawnEnemy()
    {
        // will also need to check if there is any enemy currently in same position?

        return true;
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
        nextSpawnTime = Time.time + spwanDelayTime;

        newEnemy = Instantiate(EnemyTypePrefab, transform.position, transform.rotation);



        // need to set hero component
        newEnemy.GetComponent<EnemyNormalBehavior>().setHero(HeroInMap);

        // EnemyNormalBehavior

/*      
 *      
 *      
 *      
 *      newEnemy.
        EnemyNormalBehavior.GetComponent<Health>().
        .GetComponent<hero>();


        newEnemy = GetComponent<Hero>();
        newEnemy.GetComponent<>




        FindObjectOfType<Player>();
        */
    }
}
