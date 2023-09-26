using CustomProperty;
using CustomProperty.Utils;
using KDY;
using Photon.Pun;
using Photon.Realtime;
using RoomUI.Chat;
using RoomUI.ChooseTeam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace LobbyUI.QuickStart
{
	public class MatchingPlayer
	{
		public MatchingPlayer(int entryNum)
		{
			EntryNum = entryNum;
		}

		public Player player { get; set; }

		public int MapId { get; set; }

		public int EntryNum { get; private set; }
	}

	public class QuickMatching : MonoBehaviourPunCallbacks
	{
		public const int ROOM_NUMBER = 9999;

		[SerializeField]
		private float duration = 30f;

		[SerializeField]
		private Scrollbar scrollbar;

		[SerializeField]
		private List<TeamData> teamColors;

		private Coroutine changeScrollbarSizeRoutine;

		private int mapId;

		private void Start()
		{
			
		}

		private void OnEnable()
		{
			changeScrollbarSizeRoutine = StartCoroutine(ChangeScrollbarSize());
			SetInPlayer();
		}

		public void EntryPlayer(Player player)
		{
			CheckMatching();
		}

		public void LeavePlayer(Player leavePlayer)
		{
			
		}

		private void SetInPlayer()
		{
			
			

			CheckMatching();
		}

		private void CheckMatching()
		{
			if(PhotonNetwork.IsMasterClient)
			{
				if (PhotonNetwork.PlayerList.Count() > 1)
				{
					foreach (var player in PhotonNetwork.PlayerList)
						SetTeam(player);

					PhotonNetwork.LoadLevel("GameScene");
				}	
			}
		}

		private void SetTeam(Player player)
		{
			var teamNum = UnityEngine.Random.Range(0, 8);

			PhotonHashtable playerProperty = new PhotonHashtable();
			playerProperty[PlayerProp.TEAM] = teamColors[teamNum].TeamName;
			playerProperty[PlayerProp.TEAMCOLOR] = $"#{(teamColors[teamNum].TeamColor).ToHexString()}";
			player.SetCustomProperties(playerProperty);
		}

		private void MatchingFailed()
		{
			gameObject.SetActive(false);
			PhotonNetwork.LeaveRoom();
		}

		private IEnumerator ChangeScrollbarSize()
		{
			float  startTime = Time.time;
			float initialSize = scrollbar.size;

			while (Time.time - startTime < duration)
			{
				float elapsedTime = Time.time - startTime;
				float newSize = Mathf.Lerp(initialSize, 1f, elapsedTime / duration);
				scrollbar.size = newSize;

				yield return null;
			}
			scrollbar.size = 1f;
			MatchingFailed();
		}
	}

}
