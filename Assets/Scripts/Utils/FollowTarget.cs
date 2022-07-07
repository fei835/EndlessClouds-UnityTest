using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    private Transform tr;
    public bool bSmooth = false;

    public bool bEnabled = true;
    public bool bFollowRotation = false;

    void Awake()
    {
        tr = transform;
    }

    void LateUpdate()
    {
        if (!bEnabled)
            return;
        if (target == null)
            return;

        if (bSmooth)
        {
            tr.position = Vector3.Lerp(tr.position, target.position, Time.deltaTime * 20);
            if (bFollowRotation)
                tr.rotation = Quaternion.Lerp(tr.rotation, target.rotation, Time.deltaTime * 20);
        }
        else
        {
            tr.position = target.position;
            if (bFollowRotation)
                tr.eulerAngles = new Vector3(tr.eulerAngles.x, target.eulerAngles.y, tr.eulerAngles.z);
        }
    }
}
