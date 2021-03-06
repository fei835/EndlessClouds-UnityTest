using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform tr;
    public float speed = 25f;
    public float lifeTime = 4f;
    public float spawnTime = 0f;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        tr = transform;
    }

    public void Init()
    {
        col.enabled = true;
        GameInfos.Instance.GetEnemyManager().SpawnSpawnable(SpawnableEnum.SmokeFX, tr.position, Quaternion.identity);
        spawnTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        tr.Translate(0, 0, Time.deltaTime * speed, Space.Self);
        tr.LookAt(Vector3.Lerp(tr.position + tr.forward, tr.position + Vector3.down, Time.deltaTime * 0.2f));

        if (Time.timeSinceLevelLoad > spawnTime + lifeTime)
            GameInfos.Instance.GetEnemyManager().DespawnSpawnable(SpawnableEnum.RocketBall, gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameInfos.Instance.GetPlayerMain().bIsInvincible)
                return;

            GameInfos.Instance.GetEnemyManager().SpawnSpawnable(SpawnableEnum.HurtFX, tr.position, Quaternion.identity);
            GameInfos.Instance.GetPlayerMain().camMain.ShakeCam(0.2f);

            StartCoroutine(AutoDestruct());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Env"))
        {
            GameInfos.Instance.GetEnemyManager().SpawnSpawnable(SpawnableEnum.HurtFX, tr.position, Quaternion.identity);
            StartCoroutine(AutoDestruct());
        }
    }

    IEnumerator AutoDestruct()
    {
        col.enabled = false;
        yield return null;
        GameInfos.Instance.GetEnemyManager().DespawnSpawnable(SpawnableEnum.RocketBall, gameObject);
    }
}
