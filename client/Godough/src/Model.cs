using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godough
{
	public enum Result{
		Win,Lose,Tie
	}

	public enum MOVE
	{
		Air = 0,
		Rock = 1,
		Paper = 2,
		Scissor = 3
	}

	enum OpCode
	{
		MOVE = 0,
		REVEAL = 1,
		MOVE_REJECT = 2,
		REJECTED = 3,
		NEW_ROUND = 4,
		GAME_OVER = 5,
		JOIN = 6
	}

	class RevealMessage
	{
		public Dictionary<int, MOVE> moves { get; set; }
		public int? winner { get; set; }
		public bool isTie { get; set; }
	}

	class NewRound
	{
		public int roundNo { get; set; }
	}

	class MoveMessage
	{
		public MOVE move { get; set; }
	}

	class JoinMessage
	{
		public int uraPlayer { get; set; }
	}

	class MovedMessage
	{
		public int playerNo { get; set; }
	}

}
