using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamStatesEnum
{ 
    FOLLOWPLAYER,
    DONOTHING
}

public class CamMain : MonoBehaviour
{
    private CamStatesEnum camState = CamStatesEnum.FOLLOWPLAYER;

    public Transform playerCamRoot;
    private Transform camLocator;
    private Transform camLocatorOrig;
    private Transform playerTr;
    private Transform tr;

    private bool bCollidingWithWall = false;
    private bool camShake = false;

    public LayerMask camLayerMask;

    private float shakeIntensity = 0.0f;
    private float rumbleIntensity = 0.0f;
    private float shakeDecay;
    private float shakeVariation = 1.0f;
    private Vector3 shakeVelocity = Vector3.zero;
    private Vector3 shakePosition = Vector3.zero;
    private Quaternion shakeRotation = Quaternion.identity;
    public Vector3 shakeAngles = new Vector3(6.0f, 6.0f, 3.0f);
    public float shakeDur = 0.66f;

    private float smoothing = 0.0001f;
    private float rotateSpeed = 0.0001f;

    private void Awake()
    {
        tr = transform;
        playerTr = GameObject.Find("Player_Dummy").transform;
        camLocator = playerCamRoot.Find("CamLocator");
        camLocatorOrig = playerCamRoot.Find("CamLocatorOrig");
    }

    private void Update()
    {
        if (camState == CamStatesEnum.FOLLOWPLAYER)
        {
            FollowCam();

            camLocator.position = HandleCollisions(playerTr.position, camLocator.position);

            if (!bCollidingWithWall)
                camLocator.position = Vector3.Lerp(camLocator.position, camLocatorOrig.position, Time.deltaTime*50f);
        }
    }

    Vector3 HandleCollisions(Vector3 lookAtObjectPos, Vector3 destCam)
    {
        if (camState == CamStatesEnum.DONOTHING)
        {
            bCollidingWithWall = false;
            return destCam;
        }

        float camColRadius = 0.3f;
        Vector3 toDestVec = destCam - lookAtObjectPos;
        Ray ray = new Ray(lookAtObjectPos, toDestVec);
        Vector3 collisionProofedPos = destCam;

        // CAM COLLISION
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, camColRadius, out hitInfo, toDestVec.magnitude, camLayerMask))
        {
            if (!bCollidingWithWall)
                bCollidingWithWall = true;

            collisionProofedPos = hitInfo.point + (camColRadius * hitInfo.normal);
        }
        else if (bCollidingWithWall)
            bCollidingWithWall = false;

        return collisionProofedPos;
    }

    private void FollowCam()
    {
        HandleCameraShake();

        float deltaTime = Time.deltaTime;

        Vector3 camPosition = Vector3.Lerp(tr.position, HandleCollisions(playerTr.position, camLocator.position), 1 - Mathf.Pow(smoothing, deltaTime));
        Quaternion camRotation = Quaternion.Lerp(tr.rotation, Quaternion.LookRotation(playerTr.position - tr.position), 1 - Mathf.Pow(rotateSpeed, deltaTime));

        tr.position = camPosition + shakePosition;
        tr.rotation = camRotation * shakeRotation;
    }

    public void ShakeCam(float power)
    {
        shakeIntensity = power;
        camShake = true;
    }

    void HandleCameraShake()
    {
        if (camShake)
        {
            shakeDecay = shakeIntensity / shakeDur;
            camShake = false;
        }

        float shakeMin = shakeIntensity * (1 - shakeVariation);
        float shakeX = Random.Range(-1f, 1f) * Random.Range(shakeMin, shakeIntensity);
        float shakeY = Random.Range(-1f, 1f) * Random.Range(shakeMin, shakeIntensity);
        float shakeZ = Random.Range(-1f, 1f) * Random.Range(shakeMin, shakeIntensity);

        Vector3 shakeMag = new Vector3(shakeX * shakeAngles.x,
            shakeY * shakeAngles.y,
            shakeZ * shakeAngles.z);

        shakeRotation = Quaternion.Euler(shakeMag);

        shakeIntensity -= shakeDecay * Time.unscaledDeltaTime;
        shakeIntensity = Mathf.Clamp(shakeIntensity, 0, shakeIntensity);
    }
}
