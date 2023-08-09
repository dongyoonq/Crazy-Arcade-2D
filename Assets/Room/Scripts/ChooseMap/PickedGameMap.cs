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
		public MapList mapList;

		protected override void Awake()
		{
			base.Awake();

			OnChoosedMap += ChangedGameMapInfo;
		}

		public void InitGameMap(MapData data)
		{
			base.OnMapChoosed(data);
		}

		private void ChangedGameMapInfo(MapData data)
		{
			photonView.RPC("ChangedGameMap", RpcTarget.Others, data.Id);
		}

		[PunRPC]
		private void ChangedGameMap(int mapId)
		{
			var data = mapList.Maps.Where(x => x.Id == mapId).Select(x => x).FirstOrDefault();

			base.OnMapChoosed(data);
		}
	}
}