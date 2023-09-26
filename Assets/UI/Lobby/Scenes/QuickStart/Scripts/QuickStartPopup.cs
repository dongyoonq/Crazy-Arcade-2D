using CustomProperty;
using KDY;
using LobbyUI.QuickStart;
using Photon.Pun;
using Photon.Realtime;
using RoomUI.ChooseMap;
using RoomUI.PlayerSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class QuickStartPopup : MonoBehaviourPunCallbacks
{
	public const int ROOM_NUMBER = 9999;

	[SerializeField]
	private QuickChooseMap choosedMap;

	[SerializeField]
	private CharacterSelect chooseCharacter;

	[SerializeField]
	private QuickTeam quickTeam;

	[SerializeField]
	private Image mapIcon;

	[SerializeField]
	private TMP_Text mapName;

	[SerializeField]
	private Button mapChoose;

	private int choosedMapId;
	private string choosedMapTitle;

	[SerializeField]
	private Image charIcon;

	[SerializeField]
	private TMP_Text charName;

	[SerializeField]
	private Button charChoose;

	[SerializeField]
	private Button btnSelected;

	[SerializeField]
	private Button btnClosed;

	public void InitView()
	{
		mapChoose.onClick.AddListener(() => OpenChoosedMapView());
		charChoose.onClick.AddListener(() => OpenChoosedCharView());
		btnSelected.onClick.AddListener(() => QuickStartGame());
		//btnClosed.onClick.AddListener(() => CloseView());
		MapData initMap = choosedMap.GetInitMapData();
		PickedMapData(initMap);
		choosedMapId = 0;

		chooseCharacter.InitView();
		PickedCharacter(chooseCharacter.CurrentCharInfo.characterData);
	}

	private void OnDisable()
	{
		mapChoose.onClick.RemoveAllListeners();
		charChoose.onClick.RemoveAllListeners();
		btnSelected.onClick.RemoveAllListeners();
	}

	private void OpenChoosedMapView()
	{
		//choosedMap.InitView();
		choosedMap.gameObject.SetActive(true);
		choosedMap.SetMapInfo(choosedMapId);

		choosedMap.OnChoosedMap -= PickedMapData; //event cleaning
		choosedMap.OnChoosedMap += PickedMapData;
	}

	private void PickedMapData(MapData data)
	{
		mapIcon.sprite = data.MapIcon;
		mapName.text = data.Title;

		choosedMapId = data.Id;
		choosedMapTitle = data.Title;
	}

	private void OpenChoosedCharView()
	{
		chooseCharacter.InitView();
		chooseCharacter.gameObject.SetActive(true);

		chooseCharacter.OnClosedView -= PickedCharacter; //event cleaning
		chooseCharacter.OnClosedView += PickedCharacter;
	}

	private void PickedCharacter(CharacterData data)
	{
		charIcon.sprite = data.SelectedImage;
		charName.text = data.NameKOR;
	}

	private void QuickStartGame()
	{
		CloseView();

		PhotonHashtable playerProperty = new PhotonHashtable();
		playerProperty[PlayerProp.READY] = true;
		playerProperty[PlayerProp.CHARACTER] = chooseCharacter.CurrentCharInfo.characterData.CharacterEnum;
		PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 10;
		roomOptions.CustomRoomProperties = new PhotonHashtable() {
			{ RoomProp.ROOM_NAME, "QuickStartRoom" },
			{ RoomProp.ROOM_ID, ROOM_NUMBER  },
			{ RoomProp.ROOM_MAX, 10 },
			{ RoomProp.ROOM_MAP, choosedMapTitle },
		};
		roomOptions.CustomRoomPropertiesForLobby = new string[] { RoomProp.ROOM_NAME, RoomProp.ROOM_ID, RoomProp.ROOM_MAX, RoomProp.ROOM_MAP };

		PhotonHashtable roomProperty = new PhotonHashtable();
		roomProperty[RoomProp.ROOM_MAP] = choosedMapTitle;

		PhotonNetwork.JoinRandomOrCreateRoom(roomName: (ROOM_NUMBER + choosedMapId).ToString(), roomOptions: roomOptions, expectedCustomRoomProperties: roomProperty);
	}

	private void CloseView()
	{
		chooseCharacter.DisableView();
		gameObject.SetActive(false);
	}

    public void OnClicked()
    {
        GameManager.Sound.Onclick();
    }
}
