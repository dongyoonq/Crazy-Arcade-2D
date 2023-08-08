using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RoomUI.ScriptForTest
{
	/*
	public class LoginPanel : MonoBehaviour
	{
		[SerializeField] TMP_InputField idInputField;

		private void OnEnable()
		{
			idInputField.text = GetRandomId();
		}

		public void Login()
		{
			if (idInputField.text.Trim() == "")
			{
				Debug.LogError("Invalid player name");
				return;
			}

			PhotonNetwork.LocalPlayer.NickName = idInputField.text; //LocalPlayer = 나 자신에 대한 정보(서버 접속자에 대한 정보). NickName을 지정해야함. 
			PhotonNetwork.ConnectUsingSettings();
		}

		private string GetRandomId()
		{
			return string.Format("Player {0}", Random.Range(1000, 10000));
		}
	}
	//*/
}
