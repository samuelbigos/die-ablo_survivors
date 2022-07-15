using Godot;
using System;
using GodotOnReady.Attributes;

public partial class Game : Spatial
{
    [Export] public Vector2 GridSize = new Vector2(10, 10);
    [Export] public Vector2 CellSize = new Vector2(1, 1);
    
    [OnReadyGet] private Dice _dice;

    [OnReady]
    private void Ready()
    {
        
    }

    public Vector2 GridToWorld(Vector2 grid)
    {
        return grid * CellSize;
    }
}
