using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI
{
	public class RoomNotify : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text txtMessage;

		[SerializeField]
		private Button btnOK;

		public UnityAction<string> OnNotifyPopup;

		private void Awake()
		{
			btnOK.onClick.AddListener(() => { GameManager.Sound.Onclick(); transform.gameObject.SetActive(false); });
		}

		private void OnEnable()
		{
			OnNotifyPopup += ShowPopupView;
		}

		private void OnDisable()
		{
			OnNotifyPopup -= ShowPopupView;
		}

		private void ShowPopupView(string message)
		{
			txtMessage.text = message;
			
		}
	}

}
