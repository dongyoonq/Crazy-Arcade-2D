using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDY
{
    public class CreateRoomPanel : MonoBehaviour
    {
        [SerializeField] public TMP_InputField roomNameInput;
        [SerializeField] public CreateRoomMode roomMode;
		[SerializeField] public TMP_InputField passwordInput;
        [SerializeField] public Toggle passwordToggle;
        [SerializeField] public Button okBtn;
        [SerializeField] public Button cancelBtn;

        Color orgColor;

        private void Start()
        {
            orgColor = passwordInput.GetComponent<Image>().color;
            passwordToggle.onValueChanged.AddListener((x) => ChangeInputFieldColor(x));
        }

        private void ChangeInputFieldColor(bool check)
        {
            if (check)
                passwordInput.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            else
                passwordInput.GetComponent<Image>().color = orgColor;
        }

		public void GetSeletedRoomMode()
		{

		}
	}
}