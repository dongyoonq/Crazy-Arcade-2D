using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomProperty
{
	[Serializable]
	public class RoomProp
	{
		public const string PROPERTY_KEY = "RoomProp";

		public int RoomId;

		public string RoomName;

		public string Password;
	}
}
