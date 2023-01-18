using Godot;
using Godough;
using Nakama.TinyJson;
using System;
using System.Threading.Tasks;

public class InGame : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.

	[Signal]
	delegate void IMovedSignal(bool imoved);
	[Signal]
	delegate void HistoryItemSignal(MOVE me, MOVE her, Result result);

	int myPlayerNo = 0;
	bool imoved = false;
	bool shemoved = false;

	public override void _Ready()
	{
		NakamaCon.Ins.Socket.ReceivedMatchState += Socket_ReceivedMatchState;
	}

	private async void Socket_ReceivedMatchState(Nakama.IMatchState obj)
	{
		GD.Print(obj.State.GetStringFromUTF8());

		switch (obj.OpCode)
		{
			case (long)OpCode.REVEAL:
				var reveal = fJSON.Deserialize<RevealMessage>(obj.State.GetStringFromUTF8());
				GD.Print(reveal);
				Reveal(reveal);
				break;
			case (long)OpCode.NEW_ROUND:
				var round = fJSON.Deserialize<NewRound>(obj.State.GetStringFromUTF8());
				GD.Print(round);
				IMoved(false);
				SheMoved(false);
				break;
			case (long)OpCode.JOIN:
				var join = fJSON.Deserialize<JoinMessage>(obj.State.GetStringFromUTF8());
				myPlayerNo = join.uraPlayer;
				break;
			case (long)OpCode.MOVE:
				var moved = fJSON.Deserialize<MovedMessage>(obj.State.GetStringFromUTF8());
				if(moved.playerNo == myPlayerNo)
					IMoved(true);
				else SheMoved(true);
				break;
			default:
				break;
		}
	}

	private void Reveal(RevealMessage message)
	{
		var herPlayerNo = myPlayerNo == 1 ? 2 : 1;
		var me = message.moves[myPlayerNo];
		var her = message.moves[herPlayerNo];

		Result r;
		if (message.isTie)
			r = Result.Tie;
		else
		{
			r = message.winner == myPlayerNo ? Result.Win : Result.Lose;
		}

		EmitSignal(nameof(HistoryItemSignal), me, her, r);
	}

	private void IMoved(bool imov)
	{
		EmitSignal(nameof(IMovedSignal), imoved = imov);
	}

	private void SheMoved(bool shemov)
	{
		shemoved = shemov;
	}

	private async void _on_Button_pressed()
	{
		await NakamaCon.Ins.LeaveCurrentMatch();
		GetTree().ChangeScene("scenes/MainMenu.tscn");
	}

	private async Task SendMove(MOVE move)
	{
		try
		{
			var moveMessage = new MoveMessage { move = move };
			await NakamaCon.Ins.SendMatchState((long)OpCode.MOVE, moveMessage);
		}
		catch (Exception ex)
		{
			GD.PrintErr("hat happend in game", ex);
			throw;
		}
	}

	private async void _on_Rock_pressed()
	{
		await SendMove(MOVE.Rock);
	}

	private async void _on_Paper_pressed()
	{
		await SendMove(MOVE.Paper);
	}

	private async void _on_Scissors_pressed()
	{
		await SendMove(MOVE.Scissor);
	}

}







