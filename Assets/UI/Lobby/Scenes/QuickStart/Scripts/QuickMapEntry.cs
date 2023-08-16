using LobbyUI.QuickStart;
using RoomUI.ChooseMap;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuickMapEntry : MonoBehaviour
{
	[SerializeField]
	private Button pickedMap;

	[SerializeField]
	private TMP_Text txt_Title;

	private MapData mapData;

	private void Awake()
	{
		pickedMap.onClick.AddListener(() => ChooseMap());
	}

	public void SetMapEntry(MapData data)
	{
		mapData = data;
		txt_Title.text = data.Title;
	}

	private void ChooseMap()
	{
		gameObject.GetComponentInParent<QuickChooseMap>()?.OnChoosedMap?.Invoke(mapData);
	}
}
