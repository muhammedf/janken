using Godot;
using Nakama;
using System;
using System.Threading.Tasks;

public class WaitingMatchmaking : Node2D
{
	private IMatchmakerTicket ticket;

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("waiting_godot");
		ticket = await NakamaCon.Ins.Socket.AddMatchmakerAsync();
		NakamaCon.Ins.Socket.ReceivedMatchmakerMatched += Socket_ReceivedMatchmakerMatched;
	}

	private void Socket_ReceivedMatchmakerMatched(Nakama.IMatchmakerMatched obj)
	{
		GetTree().ChangeScene("scenes/InGame.tscn");
	}

	private async void _on_Button_pressed()
	{
		GetTree().ChangeScene("scenes/MainMenu.tscn");

		await NakamaCon.Ins.Socket.RemoveMatchmakerAsync(ticket);
		NakamaCon.Ins.Socket.ReceivedMatchmakerMatched -= Socket_ReceivedMatchmakerMatched;
	}

}
