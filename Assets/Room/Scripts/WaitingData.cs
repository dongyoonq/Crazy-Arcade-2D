using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WaitingData", menuName = "Room/Waiting")]
public class WaitingData : ScriptableObject
{
	public Image WaitingView;

	public Image CloseView;

	public Image StateEnable;

	public Image StateDisable;

	public Image StateMaster;
}
