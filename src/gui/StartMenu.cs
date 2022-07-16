using Godot;
using System;
using GodotOnReady.Attributes;

public partial class StartMenu : Control
{
    [Export] private PackedScene _gameScene;
    
    [OnReadyGet] private Button _startButton;

    [OnReady]
    private void Ready()
    {
        _startButton.Connect("pressed", this, nameof(OnStartPressed));
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneTo(_gameScene);
    }
}
