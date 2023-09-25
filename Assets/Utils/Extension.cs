using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extension : MonoBehaviour
{
	public enum CharacterEnum
	{
		Dao,
		Cappi,
		Marid,
		Bazzi,
		Dizni,
		Ethi,
		Mos,
		Uni,
		Random,
		None
	}

    public enum TEAM { RED, YELLOW, ORANGE, GREEN, SKY, BLUE, PURPLE, PINK, NONE }

	public enum SpanwState { EMPTY, USE }

	public enum RoomMode
	{
		Manner = 0,
		Free = 1,
		Random = 2
	}
}
