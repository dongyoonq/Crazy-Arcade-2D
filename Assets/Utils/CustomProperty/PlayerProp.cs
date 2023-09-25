using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomProperty
{
	[Serializable]
	public class PlayerProp
	{
        // READY is Player Check Ready, READY is used RoomPanel, PlayerWaitState, WaitingPlayer, GameStartController
        public const string READY = "Ready";

        // In GameScene All Player Load check count
        public const string LOAD = "Load";

        // TEAMCOLOR is dividing team player (distinguish team by color), used Room : set, Game : load
		public const string TEAMCOLOR = "TeamColor";

        // TEAM is dividing team player (distinguish team), used Room : set, Game : load
        public const string TEAM = "Team";

        // CHARACTER is selected room character, used Room : set, Game : instantiate
        public const string CHARACTER = "Character";

		// SLOT_NUMBER is current slot position
		public const string SLOT_NUMBER = "SlotNumber";
	}
}
