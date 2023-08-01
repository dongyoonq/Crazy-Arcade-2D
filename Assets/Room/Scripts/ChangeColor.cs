using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	private Renderer renderer;

	public Color changedColor = Color.white;

	private void Start()
	{
		renderer = GetComponent<Renderer>();
		renderer.material.color = changedColor;
	}
}
