using UnityEngine;

public class GameController : MonoBehaviour
{
    public int spawnRate = 10;
    public int initialSpawnCount = 2;
    public int spawnCountIncrease = 2;

    int turnCount = 0;
    int spawnCount = 0;
    int enemiesLeftToSpawn = 0;

    MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        spawnCount = initialSpawnCount;
        mapManager = FindObjectOfType<MapManager>();
    }

    public void NextMove()
    {
        turnCount++;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            var enemyController = enemy.GetComponent<EnemyBase>();
            enemyController.TakeTurn();
        }

        if(turnCount % spawnRate == 0)
        {
            enemiesLeftToSpawn = spawnCount;
            spawnCount += spawnCountIncrease;
            // Debug.Log("spawn began");
        }

        if(enemiesLeftToSpawn > 0)
        {
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
            Shuffle(spawners);
            // Debug.Log($"{spawners.Length} spawners found");

            for(int i = 0; i < spawners.Length && enemiesLeftToSpawn > 0; i++)
            {
                Spawner spawner = spawners[i].GetComponent<Spawner>();
                if(spawner.Spawn())
                {
                    enemiesLeftToSpawn--;
                }
            }
        }
    }

    void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
