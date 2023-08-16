using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoomUI.PlayerSetting
{
	public class PlayerCharacterSetter : MonoBehaviour
	{
		[SerializeField]
		private RectTransform characterContents;

		private BtnSetPlayer currentSetData;

		public List<CharacterData> CharacterDatas {  get; private set; }

		private void Awake()
		{
			var contents = characterContents.GetComponentsInChildren<BtnSetPlayer>();
			foreach(var item in contents)
			{
				item.OnChooseCharacter += SelectedCharacter;
			}
			currentSetData = contents[1];

			CharacterDatas = contents.Select(x => x.CharacterData).ToList();
		}

		private void Start()
		{
			currentSetData.InitButton(true);
		}

		private void SelectedCharacter(BtnSetPlayer value)
		{
			if(currentSetData.CharacterData.CharacterEnum != value.CharacterData.CharacterEnum)
			{
				currentSetData.UnSelectedCharacter();
				currentSetData = value;
			}
		}
	}
}