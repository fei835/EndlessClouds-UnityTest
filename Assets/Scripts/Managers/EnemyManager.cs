#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum SpawnableEnum
{
    RocketBall,
    SmokeFX,
    HurtFX,
    SlashFX
}

// I would probably separate EnemyManager, have a dedicated pool management system
// and use a mission structure to set the enemy wave system and/or level designs on a bigger project,
// but on this small scale I'll have them all in EnemyManager and would separate only if it feels necessary over time.
public class EnemyManager : MonoBehaviour
{
    public GameObject enemyCubeGO;
    private ObjectPool<GameObject> enemyCubePool;

    public GameObject[] spawnableGos;
    private ObjectPool<GameObject>[] spawnablePoolsArray;

    [SerializeField]
    private List<GameObject> enemyCubeList = new List<GameObject>();

    private void Awake()
    {
        GameInfos.Instance.SetEnemyManager(this);

        enemyCubePool = new ObjectPool<GameObject>(createFunc: () => Instantiate(enemyCubeGO, Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true), 
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity : 200, maxSize : 1000);

        // Cannot use for loop due to retroactive call of CreateFunction.
        spawnablePoolsArray = new ObjectPool<GameObject>[4]
        {
            new ObjectPool<GameObject>(createFunc: () => Instantiate(spawnableGos[0], Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity: 200, maxSize: 1000),
            new ObjectPool<GameObject>(createFunc: () => Instantiate(spawnableGos[1], Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity: 200, maxSize: 1000),
            new ObjectPool<GameObject>(createFunc: () => Instantiate(spawnableGos[2], Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity: 200, maxSize: 1000),
            new ObjectPool<GameObject>(createFunc: () => Instantiate(spawnableGos[3], Vector3.zero, Quaternion.identity), actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), false, defaultCapacity: 200, maxSize: 1000)
        };
    }

    // Mission / GameManager
    private void Start()
    {
        SpawnEnemyCube(new Vector3(Random.Range(-5f, 5f), 0f, 10f), Quaternion.identity);
    }

    // Pool Manager
    public GameObject SpawnEnemyCube(Vector3 pos, Quaternion rot)
    {
        SpawnSpawnable(SpawnableEnum.SmokeFX, pos + Vector3.up, Quaternion.identity);
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
        StartCoroutine(RespawnNearbyC(go.transform.position + new Vector3(randomPos2D.x, 0f, randomPos2D.y)));

        // Despawn previous
        SpawnSpawnable(SpawnableEnum.SmokeFX, go.transform.position + Vector3.up, Quaternion.identity);
        enemyCubePool.Release(go);
        enemyCubeList.Remove(enemyCubeList[enemyCubeList.Count - 1]);
        enemyCubeList.TrimExcess();
    }

    IEnumerator RespawnNearbyC(Vector3 pos)
    {
        yield return new WaitForSeconds(2f);
        Transform instanceTr = SpawnEnemyCube(pos, Quaternion.identity).transform;
        instanceTr.localEulerAngles = new Vector3(0f, Random.Range(0f, 180f), 0f);
    }

    public GameObject SpawnSpawnable(SpawnableEnum spawnable, Vector3 pos, Quaternion rot)
    {
        GameObject instance = spawnablePoolsArray[(int)spawnable].Get();
        instance.transform.position = pos;
        instance.transform.rotation = rot;
        instance.SendMessage("Init");
        return instance;
    }
    public void DespawnSpawnable(SpawnableEnum spawnable, GameObject go)
    {
        spawnablePoolsArray[(int)spawnable].Release(go);
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
            SpawnSpawnable(SpawnableEnum.SmokeFX, new Vector3(Random.Range(-5f, 5f), 1f, 10f), Quaternion.identity);
        if (Input.GetKeyUp(KeyCode.I))
            SpawnSpawnable(SpawnableEnum.HurtFX, new Vector3(Random.Range(-5f, 5f), 1f, 10f), Quaternion.identity);
    }
#endif
}
