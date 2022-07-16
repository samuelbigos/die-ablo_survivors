using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Game : Singleton<Game>
{
    [Export] public Vector2 GridSize = new Vector2(10, 10);
    [Export] public Vector2 CellSize = new Vector2(1, 1);

    [Export] private List<PackedScene> _enemyScenes;

    public static Action DoPreTurn;
    public static Action DoTurn;
    public static Action DoPostTurn;
    
    public List<Enemy> Enemies => _enemies;
    
    [OnReadyGet] private Dice _dice;
    
    private List<Enemy> _enemies = new List<Enemy>();
    private int _turnCounter;

    [OnReady]
    private void Ready()
    {
        _dice.OnDestroyed += OnGridObjectDestroyed;
    
        ModPickup bulletPickup = ModManager.Instance.ModPickupScenes[ModManager.ModTypes.Bullet].Instance<ModPickup>();
        AddChild(bulletPickup);
        bulletPickup.Init(new Vector2(7, 7), ModManager.ModTypes.Bullet);

        Utils.RNG.Seed = (ulong) DateTime.Now.Millisecond;
    }

    public void TriggerTurn()
    {
        if (_turnCounter % 3 == 0)
        {
            SpawnEnemy();
        }
        
        _turnCounter++;
        
        DoPreTurn?.Invoke(); // evaluate the dice move and resulting actions
        DoTurn?.Invoke(); // evaluate enemy movement and attacks
        DoPostTurn?.Invoke(); // evaluate enemy destroyed
    }

    private void SpawnEnemy()
    {
        Vector2 pos = Vector2.Zero;
        uint rand4 = Utils.RNG.Randi() % 4;
        pos = rand4 switch
        {
            0 => new Vector2(0, Utils.RNG.Randi() % GridSize.y),
            1 => new Vector2(1, Utils.RNG.Randi() % GridSize.y),
            2 => new Vector2(Utils.RNG.Randi() % GridSize.x, 0),
            3 => new Vector2(Utils.RNG.Randi() % GridSize.x, 1),
            _ => pos
        };

        Enemy enemy = _enemyScenes[0].Instance<Enemy>();
        AddChild(enemy);
        enemy.Init(pos);
        _enemies.Add(enemy);
        enemy.OnDestroyed += OnGridObjectDestroyed;
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
    
    private void OnGridObjectDestroyed(GridObject obj)
    {
        if (obj is Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        if (obj is Dice)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GetTree().ReloadCurrentScene();
    }
}
