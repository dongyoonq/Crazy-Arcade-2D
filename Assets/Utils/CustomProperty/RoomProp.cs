using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomProperty
{
	public class RoomProp
	{
        public const string PROPERTY_KEY = "RoomProp";

		// ROOM_MAP_ID is  MapData.Id
		public const string ROOM_MAP_ID = "MapId";

		// ROOM_MAP_GROUP is MapData.Group
		public const string ROOM_MAP_GROUP = "MapGroup";

        // ROOM_MAP_GROUP is MapData.Title
        public const string ROOM_MAP = "Map";

		// ROOM_MAP_GROUP is MapData.FileName
		public const string ROOM_MAP_FILE = "MapFile";

        // ROOM_ID is RoomEntry, RoomChangedInfo used
        public const string ROOM_ID = "RoomId";

        // ROOM_NAME is RoomEntry, RoomChangedInfo used
        public const string ROOM_NAME = "RoomName";

		// ROOM_MAX is Mas Players 
		public const string ROOM_MAX = "RoomMaxNum";

		// ROOM_PASSWORD is RoomEntry, RoomChangedInfo used
		public const string ROOM_PASSWORD = "Password";

        // ROOM_STATE is RoomEntry used,, expected WaitingRoomEnter used
        public const string ROOM_STATE = "RoomState";

        // ROOM_MODE is RoomEntry, WaitingRoomEnter,, expected RoomChangedInfo used
        public const string ROOM_MODE = "Mode";

        public const string ROOM_PLAYING = "RoomPlay";

        // In GameScene All Player Load than LoadTime used
        public const string LOAD_TIME = "LoadTime";

		// NickName of All Player in the room
		public const string PLAYER_LIST = "PlayerList";

        public const string SLOT_STATE = "SlotState";
    }
}
