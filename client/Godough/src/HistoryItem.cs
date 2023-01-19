using Godot;
using Godough;
using Godough.src;
using System;

public class HistoryItem : GridContainer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void Init(MOVE me, MOVE her, Result result)
	{
		InitRPS(me, "Me");
		InitRPS(her, "Her");

		var cr = GetNode<ColorRect>("Node2D/ColorRect");

		var color = Colors.Blue;
		if (result == Result.Win)
			color = Colors.Green;
		else if (result == Result.Lose)
			color = Colors.Red;

		cr.Color = color;
	}

	private void InitRPS(MOVE move, string who)
	{
		string fileName = null;
		switch (move)
		{
			case MOVE.Rock:
				fileName = "rock.png";
				break;
			case MOVE.Paper:
				fileName = "paper.png";
				break;
			case MOVE.Scissor:
				fileName = "scissors.png";
				break;
			default:
				break;
		}

		var sprite = GetNode<Sprite>(who+"/Sprite");
		sprite.Texture = ResourceManager.Ins.Load<Texture>("res://assets/" + fileName, true);
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
