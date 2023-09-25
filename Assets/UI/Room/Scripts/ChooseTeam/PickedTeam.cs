using CustomProperty;
using Photon.Pun;
using Photon.Realtime;
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

		private BtnTeam[] btnTeams;

		private void Awake()
		{
			toggleGroup = GetComponent<ToggleGroup>();
			if (CheckPlayingRoom())
			{
				SetReturnTeamInfo();
            }
			else
			{
                SetTeamInfo();
            }
		}

		private void SetTeamInfo()
		{
			btnTeams = transform.GetComponentsInChildren<BtnTeam>();
			int index = 0;
			foreach(var data in TempDatas)
			{
				btnTeams[index].teamData = data;
				btnTeams[index].togChecked.group = toggleGroup;
				btnTeams[index].togChecked.isOn = false;
				index++;
			}
			btnTeams[0].togChecked.isOn = true;
		}

        private void SetReturnTeamInfo()
        {
			btnTeams = transform.GetComponentsInChildren<BtnTeam>();
            int index = 0;
            foreach (var data in TempDatas)
            {
                btnTeams[index].teamData = data;
                btnTeams[index].togChecked.group = toggleGroup;
				btnTeams[index].togChecked.isOn = false;
                index++;
            }

			PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;
			BtnTeam findTeam = Array.Find(btnTeams, x => x.teamData.TeamName == (string)property[PlayerProp.TEAM]);
			findTeam.togChecked.isOn = true;
        }

        public void InitTeam(Player player)
		{
			PhotonHashtable property = new PhotonHashtable();
			property[PlayerProp.TEAMCOLOR] = $"#{TempDatas[0].TeamColor.ToHexString()}";
			property[PlayerProp.TEAM] = TempDatas[0].TeamName;
			PhotonNetwork.LocalPlayer.SetCustomProperties(property);
		}

		public void InitTeam()
		{
			btnTeams[0].togChecked.isOn = true;
		}	

        private bool CheckPlayingRoom()
        {
            PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

            if (property.ContainsKey(RoomProp.ROOM_PLAYING) && (bool)property[RoomProp.ROOM_PLAYING])
            {
                return true;
            }

            return false;
        }
    }
}