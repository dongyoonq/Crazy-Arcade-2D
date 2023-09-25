using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDY
{
    public class PasswordRoomPanel : MonoBehaviour
    {
        [SerializeField] public TMP_InputField passwordInput;
        [SerializeField] public Button confirmBtn;

		private void OnDisable()
		{
			passwordInput.text = string.Empty;
		}
	}
}