using Photon.Realtime;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable; 

namespace CustomProperty.Utils
{
	public static class CustProperty
	{
		public const string PROPERTYKEY_LOAD = "Load";
		public const string PROPERTYKEY_LOADTIME = "LoadTime";

		public static bool GetReady(this Player player)
		{
			return GetReadyProperty(player, PlayerProp.READY, false);
		}

		public static void SetReady(this Player player, bool ready)
		{
			SetReadyProperty(player, PlayerProp.READY, ready);
		}

		public static bool GetLoad(this Player player)
		{
			return GetReadyProperty(player, PROPERTYKEY_LOAD, false);
		}

		public static void SetLoad(this Player player, bool load)
		{
			SetReadyProperty(player, PROPERTYKEY_LOAD, load);
		}

		public static int GetLoadTime(this Room room)
		{
			return GetReadyProperty(room, PROPERTYKEY_LOADTIME, -1);
		}

		public static void SetLoadTime(this Room room, int loadTime)
		{
			SetReadyProperty(room, PROPERTYKEY_LOADTIME, loadTime);
		}

		private static T GetReadyProperty<T>(Player player, string propertyKey, T returnValue)
		{
			PhotonHashtable property = player.CustomProperties;

			if (property.ContainsKey(propertyKey)) //일단 해당 키 값이 있는지를 먼저 확인하기
				return (T)property[propertyKey];
			else
				return returnValue;
		}

		private static T GetReadyProperty<T>(Room room, string propertyKey, T returnValue)
		{
			PhotonHashtable property = room.CustomProperties;

			if (property.ContainsKey(propertyKey))
				return (T)property[propertyKey];
			else
				return returnValue;
		}

		private static void SetReadyProperty<T>(Player player, string propertyKey, T value)
		{
			PhotonHashtable property = new PhotonHashtable();

			property[propertyKey] = value;
			player.SetCustomProperties(property);
		}

		private static void SetReadyProperty<T>(Room room, string propertyKey, T value)
		{
			PhotonHashtable property = new PhotonHashtable();

			property[propertyKey] = value;
			room.SetCustomProperties(property);
		}
	}

}

