using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarPoint : MonoBehaviour
{
	[SerializeField]
	private Image StarImg;

	[SerializeField]
	private Sprite StarON;

	[SerializeField]
	private Sprite StarOFF;

	public void SetStar(bool isOn)
	{
		StarImg.sprite = isOn ? StarON : StarOFF;
	}
}
