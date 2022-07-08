using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyCube : MonoBehaviour
{
    public int hp = 5;

    public Transform shootLocator;
    private Transform playerTr;
    private Transform tr;
    public Transform rawLookAt;

    private float rateOfFire = 3f;
    private float fireTimer = 0f;

    private bool bCanBehit = true;

    private void Start()
    {
        tr = transform;
        playerTr = GameInfos.Instance.GetPlayerMain().transform;
    }

    public void Init()
    {
        hp = 5;
        fireTimer = 0f;
        StartCoroutine(IFrames());
    }

    IEnumerator IFrames()
    {
        bCanBehit = false;
        yield return new WaitForSeconds(1f);
        bCanBehit = true;
    }

    private void Update()
    {
        if (hp <= 0)
            return;

        rawLookAt.LookAt(playerTr);
        tr.rotation = Quaternion.Lerp(tr.rotation, rawLookAt.rotation, Time.deltaTime * 5f);

        if (fireTimer > rateOfFire)
        {
            GameInfos.Instance.GetEnemyManager().SpawnSpawnable(SpawnableEnum.RocketBall, shootLocator.position, shootLocator.rotation);
            fireTimer = 0f;
        }
        fireTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bCanBehit)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("AttackEnemy"))
        {
            hp--;
            Camera.main.GetComponent<CamMain>().ShakeCam(0.03f);

            if (hp <= 0)
                GameInfos.Instance.GetEnemyManager().DespawnEnemyCube(gameObject);
        }
    }
}
