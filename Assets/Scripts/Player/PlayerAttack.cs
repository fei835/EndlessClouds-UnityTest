using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, other.transform.position- transform.position, out hit, 3f))
                GameInfos.Instance.GetEnemyManager().SpawnSpawnable(SpawnableEnum.SlashFX, hit.point, Quaternion.identity);
        }
    }
}
