using Godot;
using Godot.Collections;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class MainMenu : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await NakamaCon.Ins.Init();
	}

	private async void _on_Button_pressed()
	{
		this.GetTree().ChangeScene("scenes/WaitingMatchmaking.tscn");

	}

}

