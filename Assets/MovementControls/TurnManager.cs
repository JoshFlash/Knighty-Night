using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public static TurnManager instance { get; private set; }

	public static List<EnemyController> enemyControllers;

	private void Awake()
	{
		instance = this;
		enemyControllers = new List<EnemyController>();
	}

	private void Start()
	{
		EnemyController[] controllers = FindObjectsOfType<EnemyController>();
		for (int i = 0; i < controllers.Length; i++)
		{
			enemyControllers.Add(controllers[i]);
		}
	}

	public static void TriggerNextTurn()
	{
		foreach (EnemyController ec in TurnManager.enemyControllers)
		{
			ec.TargetHexCell();
		}
	}


}
