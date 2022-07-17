using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Game : Singleton<Game>
{
    [Export] private string _startScene;
    
    [Export] public Vector2 GridSize = new Vector2(10, 10);
    [Export] public Vector2 CellSize = new Vector2(1, 1);

    [Export] private List<PackedScene> _enemyScenes;
    [Export] private PackedScene _floatingText;
    [Export] private PackedScene _oneUpScene;
    
    public static Action DoPreTurn;
    public static Action DoTurn;
    public static Action DoPostTurn;
    
    public List<Enemy> Enemies => _enemies;
    public Camera Camera => _camera;
    
    [OnReadyGet] private Dice _dice;
    [OnReadyGet] private DiceIndicator _diceIndicator;
    [OnReadyGet] private Camera _camera;
    [OnReadyGet] private CanvasLayer _canvas;
    [OnReadyGet] private Label _turnsCounter;
    [OnReadyGet] private Label _coinsCounter;
    
    // debug
    [OnReadyGet] private Label _fps;
    [OnReadyGet] private Label _verts;
    [OnReadyGet] private Label _draws;
    
    private List<Enemy> _enemies = new List<Enemy>();
    private int _turnCounter;
    private bool _gameOver;
    private int _lastSpawnedPickup;

    [OnReady]
    private void Ready()
    {
        _dice.OnDestroyed += OnGridObjectDestroyed;
        _dice.OnDamaged += OnGridObjectDamaged;
        _dice.OnHealed += OnHealed;

        Utils.RNG.Seed = (ulong) DateTime.Now.Millisecond;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // debug
        _fps.Text = $"FPS: {Performance.GetMonitor(Performance.Monitor.TimeFps):02}";
        _verts.Text = $"Verts: {Performance.GetMonitor(Performance.Monitor.RenderVerticesInFrame)}";
        _draws.Text = $"Draws: {Performance.GetMonitor(Performance.Monitor.RenderDrawCallsInFrame)}";
        
        if (_gameOver)
            GetTree().ChangeScene(_startScene);
        
        _coinsCounter.Text = $"{_dice.Coins}";
    }

    public void TriggerTurn()
    {
        DoPreTurn?.Invoke(); // evaluate the dice move and resulting actions
        DoTurn?.Invoke(); // evaluate enemy movement and attacks
        DoPostTurn?.Invoke(); // evaluate enemy destroyed
        
        _turnCounter++;

        _turnsCounter.Text = $"{_turnCounter}";
        
        if (_turnCounter == 1)
        {
            TrySpawnPickup(new Vector2(16, 16), ModManager.ModTypes.COUNT);
        }
        
        if (_turnCounter % 3 == 0)
        {
            SpawnEnemy();
        }

        if (_turnCounter % 30 == 0)
        {
            SpawnOneUp(RandPointOnEdge());
        }
    }

    private void SpawnEnemy()
    {
        Vector2 pos = RandPointOnEdge();
        Enemy enemy = _enemyScenes[2].Instance<Enemy>();
        AddChild(enemy);
        _enemies.Add(enemy);
        enemy.Init(pos);
        enemy.OnDestroyed += OnGridObjectDestroyed;
        enemy.OnDamaged += OnGridObjectDamaged;
    }

    private Vector2 RandPointOnEdge()
    {
        Vector2 pos = Vector2.Zero;
        uint rand4 = Utils.RNG.Randi() % 4;
        pos = rand4 switch
        {
            0 => new Vector2(0, Utils.RNG.Randi() % GridSize.y),
            1 => new Vector2(GridSize.x - 1, Utils.RNG.Randi() % GridSize.y),
            2 => new Vector2(Utils.RNG.Randi() % GridSize.x, 0),
            3 => new Vector2(Utils.RNG.Randi() % GridSize.x, GridSize.y - 1),
            _ => pos
        };
        return pos;
    }

    private void TrySpawnPickup(Vector2 gridPos, ModManager.ModTypes type)
    {
        int spawn = 1;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, ModManager.ModTypes.Bullet);
            SpawnPickup(gridPos + Vector2.Up, ModManager.ModTypes.Heal);
            SpawnPickup(gridPos + Vector2.Up * 2.0f, ModManager.ModTypes.Lightning);
            SpawnPickup(gridPos + Vector2.Up * 3.0f, ModManager.ModTypes.Coin);
            SpawnPickup(gridPos + Vector2.Up * -1.0f, ModManager.ModTypes.Freeze);
            SpawnOneUp(gridPos + Vector2.Right);
        }
        
        spawn = 10;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, type);
        }
        
        spawn = 30;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, type);
        }
        
        spawn = 70;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, type);
        }
        
        spawn = 110;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, type);
        }
        
        spawn = 160;
        if (_turnCounter >= spawn && _lastSpawnedPickup < spawn)
        {
            SpawnPickup(gridPos, type);
        }
    }

    private void SpawnPickup(Vector2 gridPos, ModManager.ModTypes type)
    {
        if (type == ModManager.ModTypes.COUNT)
        {
            type = (ModManager.ModTypes) Utils.RNG.RandiRange(1, (int) (ModManager.ModTypes.COUNT - 1));
        }
        ModPickup bulletPickup = ModManager.Instance.ModPickupScenes[type].Instance<ModPickup>();
        AddChild(bulletPickup);
        bulletPickup.Init(gridPos, type);
        _lastSpawnedPickup = _turnCounter;
    }

    private void SpawnOneUp(Vector2 gridPos)
    {
        OneUp oneUp = _oneUpScene.Instance<OneUp>();
        AddChild(oneUp);
        oneUp.Init(gridPos, 50);
        _lastSpawnedPickup = _turnCounter;
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
    
    private void OnGridObjectDamaged(GridObject obj, int damage, ModManager.ModTypes type)
    {
        FloatingText floatingText = _floatingText.Instance<FloatingText>();
        _canvas.AddChild(floatingText);
        string text = obj is Dice ? $"-{damage}" : $"{damage}";
        Color col = obj is Dice ? ModManager.Instance.ModColours[ModManager.ModTypes.Number] : ModManager.Instance.ModColours[type];
        floatingText.Init(_camera, obj, text, col);
    }
    
    private void OnHealed(Dice dice, int amount)
    {
        FloatingText floatingText = _floatingText.Instance<FloatingText>();
        _canvas.AddChild(floatingText);
        string text = $"+{amount}";
        floatingText.Init(_camera, dice, text, ModManager.Instance.ModColours[ModManager.ModTypes.Heal]);
    }

    public void OnCoinCollect(int amount)
    {
        _dice.Coins += amount;
        FloatingText floatingText = _floatingText.Instance<FloatingText>();
        _canvas.AddChild(floatingText);
        string text = $"+{amount}";
        floatingText.Init(_camera, _dice, text, ModManager.Instance.ModColours[ModManager.ModTypes.Coin]);
        _coinsCounter.Text = $"{_dice.Coins}";
    }
    
    private void OnGridObjectDestroyed(GridObject obj)
    {
        if (obj is Enemy enemy)
        {
            _enemies.Remove(enemy);
            TrySpawnPickup(obj.GridPos, ModManager.ModTypes.COUNT);
        }

        if (obj is Dice)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _gameOver = true;
    }
}
