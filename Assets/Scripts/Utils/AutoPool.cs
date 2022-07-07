using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPool : MonoBehaviour
{
	public Light dLight;
	public AnimationCurve curve;
	private bool bHasLight = true;

	void Awake()
	{
		if (dLight == null)
			bHasLight = false;
	}

	public void Init()
    {
		StartCoroutine(AutoPoolC());
	}

	IEnumerator AutoPoolC()
	{
		float timeMax = GetComponent<ParticleSystem>().main.duration;
		float ninth = timeMax / 9f;
		timeMax -= ninth;

		float timer = 0.0f;

		while (timer < timeMax)
		{
			timer += Time.deltaTime;

			if (bHasLight)
				dLight.intensity = curve.Evaluate(Mathf.InverseLerp(0f, timeMax, timer));
			yield return null;
		}

		yield return new WaitForSeconds(ninth + 0.5f);

		GameInfos.Instance.GetEnemyManager().DespawnDespawnFXPool(gameObject);
	}
}
