using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
	public class ChooseMap : MonoBehaviourPunCallbacks
	{
		[SerializeField] 
		protected Image mapImg;

		[SerializeField]
		protected TMP_Text mapTitle;

		[SerializeField]
		protected TMP_Text maxPlayer;
		
		[SerializeField]
		protected Image levelImg;

		[SerializeField]
		protected MapPopularity popularity;

		protected MapData curChoosedMap { get; private set; }

		public UnityAction<MapData> OnChoosedMap;

		protected virtual void Awake()
		{
			OnChoosedMap += OnMapChoosed;
		}

		protected virtual void OnMapChoosed(MapData data)
		{
			curChoosedMap = data;

			mapTitle.text = data.Title;
			maxPlayer.text = data.MaxPlayer.ToString();
			mapImg.sprite = data.MapImg;
			//levelImg.sprite = data.Level
			popularity.SetPopularity(data.Popularity);
		}
	}
}