using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		InitGameManager();
	}

	private static void InitGameManager()
	{
		if (GameManager.Instance == null)
		{
			GameObject gameManager = new GameObject();
			gameManager.name = "GameManager";
			gameManager.AddComponent<GameManager>();
		}
	}
}