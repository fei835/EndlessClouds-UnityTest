#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// I would probably separate EnemyManager, EnemySpawner, maybe have dedicated pool management system
// and use a mission structure to set the enemy wave system on a bigger project,
// but on this small scale I'll have them all in EnemyManager and would separate only if it feels necessary over time.

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyCubeGO;
    private ObjectPool<GameObject> enemyCubePool;

    public GameObject rocketBallGO;
    private ObjectPool<GameObject> rocketBallPool;
    
    public GameObject smokeFXGO;
    private ObjectPool<GameObject> smokeFXPool;

    [SerializeField]
    private List<GameObject> enemyCubeList = new List<GameObject>();

    private void Awake()
    {
        GameInfos.Instance.SetEnemyManager(this);

        enemyCubePool = new ObjectPool<GameObject>(createFunc: () => Instantiate(enemyCubeGO, Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true), 
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity : 200, maxSize : 1000);
        rocketBallPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(rocketBallGO, Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true), 
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity : 200, maxSize : 1000);
        smokeFXPool = new ObjectPool<GameObject>(createFunc: () => Instantiate(smokeFXGO, Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity: 200, maxSize: 1000);
    }

    private void Start()
    {
        SpawnEnemyCube(new Vector3(Random.Range(-5f, 5f), 0f, 10f), Quaternion.identity);
    }

    public GameObject SpawnEnemyCube(Vector3 pos, Quaternion rot)
    {
        SpawnSmokeFx(pos + Vector3.up, Quaternion.identity);
        GameObject instance = enemyCubePool.Get();
        instance.transform.position = pos;
        instance.transform.rotation = rot;
        instance.transform.SetParent(transform);
        instance.GetComponent<EnemyCube>().Init();
        enemyCubeList.Add(instance);
        return instance;
    }
    public void DespawnEnemyCube(GameObject go)
    {
        // Respawn an enemy at random within 3m radius from death
        Vector2 randomPos2D = Random.insideUnitCircle * 3f;
        SpawnEnemyCube(go.transform.position + new Vector3(randomPos2D.x, 0f, randomPos2D.y), Quaternion.identity);

        // Despawn previous
        SpawnSmokeFx(go.transform.position + Vector3.up, Quaternion.identity);
        enemyCubePool.Release(go);
        enemyCubeList.Remove(enemyCubeList[enemyCubeList.Count - 1]);
        enemyCubeList.TrimExcess();
    }

    public GameObject SpawnRocketBall(Vector3 pos, Quaternion rot)
    {
        GameObject instance = rocketBallPool.Get();
        instance.transform.position = pos;
        instance.transform.rotation = rot;
        instance.GetComponent<Projectile>().Init();
        return instance;
    }
    public void DespawnRocketBall(GameObject go)
    {
        rocketBallPool.Release(go);
    }
    public GameObject SpawnSmokeFx(Vector3 pos, Quaternion rot)
    {
        GameObject instance = smokeFXPool.Get();
        instance.transform.position = pos;
        instance.transform.rotation = rot;
        instance.GetComponent<AutoPool>().Init();
        return instance;
    }
    public void DespawnDespawnFXPool(GameObject go)
    {
        smokeFXPool.Release(go);
    }

    public int GetActiveEnemyCount()
    {
        return enemyCubeList.Count;
    }

#if TEST
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
            SpawnEnemyCube(new Vector3(Random.Range(-5f, 5f), 0f, 10f), Quaternion.identity);
        if (Input.GetKeyUp(KeyCode.Y))
            DespawnEnemyCube(enemyCubeList[enemyCubeList.Count - 1]);
        if (Input.GetKeyUp(KeyCode.U))
            SpawnSmokeFx(new Vector3(Random.Range(-5f, 5f), 1f, 10f), Quaternion.identity);
    }
#endif
}
