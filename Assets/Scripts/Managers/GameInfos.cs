using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfos : Singleton<GameInfos>
{
	protected GameInfos() { }

	private EnemyManager enemyManager;
	private PlayerMain playerMain;

	public void SetEnemyManager(EnemyManager em)
	{
		enemyManager = em;
	}

	public EnemyManager GetEnemyManager()
	{
		if (enemyManager == null)
		{
			Debug.Log("No EnemyManager assigned, check if scene has a EnemyManager");
			return null;
		}
		else
			return enemyManager;
	}

	public void SetPlayerMain(PlayerMain pm)
	{
		playerMain = pm;
	}

	public PlayerMain GetPlayerMain()
	{
		if (playerMain == null)
		{
			Debug.Log("No PlayerMain assigned, check if scene has a PlayerMain");
			return null;
		}
		else
			return playerMain;
	}
}
