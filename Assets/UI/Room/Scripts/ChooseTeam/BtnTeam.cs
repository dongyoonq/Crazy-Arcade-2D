using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomProperty;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.Events;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;

namespace RoomUI.ChooseTeam
{
	public class BtnTeam : MonoBehaviourPun
	{
		[SerializeField]
		private Button btnPickedTeam;

		public Toggle togChecked;

		public bool IsChecked { get { return togChecked.isOn; } }

		public TeamData teamData;

		public static UnityAction<Color> OnChangedCharacterTeam;

		private void Awake()
		{
			btnPickedTeam.onClick.AddListener(() => PickedTeam());
		}

		private void PickedTeam()
		{
            GameManager.Sound.Onclick();
            togChecked.isOn = true;

			PhotonHashtable property = new PhotonHashtable();
			property[PlayerProp.TEAMCOLOR] = $"#{teamData.TeamColor.ToHexString()}";
            property[PlayerProp.TEAM] = teamData.TeamName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(property);
		}
	}

}
