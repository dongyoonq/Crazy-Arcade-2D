using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI
{
	public class CheckBox : MonoBehaviour
	{
		[SerializeField]
		private Button CheckButton;

		[SerializeField]
		private Image CheckMark;

		public UnityAction<bool> OnClickCheckBox;

		private bool isChecked;

		private void Awake()
		{
			CheckButton.onClick.AddListener(() => ClickedCheckBtn());
		}

		private void ClickedCheckBtn()
		{
			isChecked = !isChecked;
			CheckMark.gameObject.SetActive(isChecked);

			OnClickCheckBox?.Invoke(isChecked);
		}

		public void SetCheck()
		{
			SetCheckBoxStatet(true);
		}

		public void SetUnChecke()
		{
			SetCheckBoxStatet(false);
		}

		private void SetCheckBoxStatet(bool check)
		{
			isChecked = check;
			CheckMark.gameObject.SetActive(isChecked);
		}
	}
}