using Godot;
using System;
using System.Linq;

public class MoveSelect : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	private void _on_Node2D_IMovedSignal(bool imoved)
	{
		foreach (var item in GetChildren().Cast<Node>().Where(x => x is Button).Cast<Button>())
		{
			item.Disabled = imoved;
		}
	}

}

