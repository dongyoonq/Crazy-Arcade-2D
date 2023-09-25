using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Extension;

namespace RoomUI.PlayerSetting
{
	public class PlayerCharacterSetter : MonoBehaviour
	{
		[SerializeField]
		private RectTransform characterContents;

		private BtnSetPlayer currentSetData;

		public List<CharacterData> CharacterDatas { get; private set; }

		private BtnSetPlayer[] btnContents;

		private void Awake()
		{
			btnContents = characterContents.GetComponentsInChildren<BtnSetPlayer>();
			foreach(var item in btnContents)
			{
				item.OnChooseCharacter += SelectedCharacter;
			}
			currentSetData = btnContents[1];

			CharacterDatas = btnContents.Select(x => x.CharacterData).ToList();
		}

		private void Start()
		{
			currentSetData.InitButton(true);
		}

		private void SelectedCharacter(BtnSetPlayer value)
		{
			if(currentSetData.CharacterData.CharacterEnum != value.CharacterData.CharacterEnum)
			{
				currentSetData?.UnSelectedCharacter();
				currentSetData = value;
			}
		}

		public void SelectedCharacter(CharacterEnum character)
		{
			var btnPlayer = characterContents.GetComponentsInChildren<BtnSetPlayer>()?.Where(x => x.CharacterData.CharacterEnum == character).FirstOrDefault();

			if(btnPlayer != null)
			{
				SelectedCharacter(btnPlayer);
			}
		}

		public void InitCharacterSetter()
		{
			currentSetData.InitButton(false);
			
			currentSetData = btnContents[1];
			currentSetData.InitButton(true);
		}
	}
}