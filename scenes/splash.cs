using Godot;
using System;

public partial class splash : Node2D
{
	[Export]
	public Button restartButton;

	public event Action RestartButtonPressed;

	[Export]
	public Button mmButton;
	public event Action MMButtonPressed;




	public override void _Ready()
	{
		// Connect button signals
		if (restartButton != null)
		{
			GD.Print("do 1");
			restartButton.Pressed += OnRestartButtonPressed;
		}
		if (mmButton != null)
		{
			mmButton.Pressed += OnMMButtonPressed;
		}


	}
	private void OnRestartButtonPressed()
	{
		GD.Print("do 2");
		SceneManager.Instance.ChangeScene("res://scenes/boarding_school.tscn");
	}
	private void OnMMButtonPressed()
	{
		SceneManager.Instance.ChangeScene("res://scenes/MainMenu.tscn");
	}
	
}
