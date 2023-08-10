using CustomProperty;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace RoomUI.ChooseTeam
{
	public class PickedTeam : MonoBehaviour
	{
		[SerializeField]
		private List<TeamData> TempDatas;

		private ToggleGroup toggleGroup;

		private void Awake()
		{
			toggleGroup = GetComponent<ToggleGroup>();
			SetTeamInfo();
		}

		private void SetTeamInfo()
		{
			var teams = transform.GetComponentsInChildren<BtnTeam>();
			int index = 0;
			foreach(var data in TempDatas)
			{
				teams[index].teamData = data;
				teams[index].togChecked.group = toggleGroup;
				teams[index].togChecked.isOn = false;
				index++;
			}
			teams[0].togChecked.isOn = true;
		}

		public void InitTeam()
		{
			PhotonHashtable property = new PhotonHashtable();
			property[PlayerProp.TEAMCOLOR] = $"#{TempDatas[0].TeamColor.ToHexString()}";
			property[PlayerProp.TEAM] = TempDatas[0].TeamName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(property);
		}
	}
}