using Godot;
using Godough;
using System;

public class History : MarginContainer
{
	private SceneTreeTween tw;

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

	private void _on_Node2D_HistoryItemSignal(MOVE me, MOVE her, Result res)
	{
		try
		{
			var ps = (PackedScene)ResourceLoader.Load("res://scenes/HistoryItem.tscn");
			var hi = ps.Instance<HistoryItem>();
			hi.Init(me, her, res);

			var stackNode = GetNode("ScrollContainer/CenterContainer/Stack");
			stackNode.AddChild(hi);
			stackNode.MoveChild(hi, 0);
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
			throw;
		}
		//var ps = new PackedScene();
		//ps.ResourcePath = "scenes/HistoryItem.tscn";
		//var hi = ps.Instance<HistoryItem>();
		//hi.Init(me, her);
	}

	private void _on_ScrollContainer_mouse_entered()
	{
		tw?.Stop();

		tw = CreateTween();
		var twe = tw.TweenProperty(this, "modulate", Colors.White, 1);
		twe.SetTrans(Tween.TransitionType.Expo);
		twe.SetEase(Tween.EaseType.Out);
		tw.Play();
	}


	private void _on_ScrollContainer_mouse_exited()
	{
		tw?.Stop();

		var white = Colors.White;
		white.a = 0.2f;

		tw = CreateTween();
		var twe = tw.TweenProperty(this, "modulate", white, 1);
		twe.SetTrans(Tween.TransitionType.Linear);
		twe.SetEase(Tween.EaseType.In);
		tw.Play();
	}

}


