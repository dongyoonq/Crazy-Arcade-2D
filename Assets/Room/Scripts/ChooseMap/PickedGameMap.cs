using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace RoomUI.ChooseMap
{
    public class PickedGameMap : ChooseMap
    {
		protected override void Awake()
		{
			base.Awake();
		}

		public void InitGameMap(MapData data)
		{
			base.OnMapChoosed(data);
		}
	}
}