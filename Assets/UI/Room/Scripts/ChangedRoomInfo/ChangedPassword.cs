using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChangedRoomInfo
{
	public class ChangedPassword : MonoBehaviour
	{
		[SerializeField]
		private Toggle isPrivateRoom;

		[SerializeField]
		private TMP_InputField inputPassword;

		[SerializeField]
		private Image passwordBackImg;

		private void Awake()
		{
			isPrivateRoom.onValueChanged.AddListener((isChecked) => ClickedCheckBox(isChecked));
		}

		private void ClickedCheckBox(bool isChecked)
		{
			inputPassword.gameObject.SetActive(isChecked);
			passwordBackImg.gameObject.SetActive(!isChecked);
		}

		public void SetRoomPassword(string pwdValue)
		{
			bool isPrivate = pwdValue.Trim() != "";
			ClickedCheckBox(isPrivate);
			inputPassword.text = pwdValue;
		}

		public string GetRoomPassword()
		{
			return inputPassword.text;
		}

		public bool IsSetPrivate()
		{
			return isPrivateRoom.isOn;
		}
	}
}