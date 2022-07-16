using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Game : Singleton<Game>
{
    [Export] public Vector2 GridSize = new Vector2(10, 10);
    [Export] public Vector2 CellSize = new Vector2(1, 1);

    [Export] private List<PackedScene> _enemyScenes;

    public static Action DoTurn;
    
    [OnReadyGet] private Dice _dice;
    
    private List<Enemy> _enemies;

    [OnReady]
    private void Ready()
    {
        Enemy enemy = _enemyScenes[0].Instance<Enemy>();
        AddChild(enemy);
        enemy.Init(new Vector2(2, 2));
    }

    public void TriggerTurn()
    {
        DoTurn?.Invoke();
    }

    public Vector3 GridToWorld(Vector2 gridPos)
    {
        return (gridPos * CellSize).To3D();
    }

    public bool InBounds(Vector2 gridPos)
    {
        if (gridPos.x >= GridSize.x) return false;
        if (gridPos.x < 0) return false;
        if (gridPos.y >= GridSize.y) return false;
        if (gridPos.y < 0) return false;
        return true;
    }
}
