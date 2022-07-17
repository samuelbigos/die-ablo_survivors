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
    [OnReadyGet] private Label _waveLabel;
    
    // sfx
    [OnReadyGet] private AudioStreamPlayer2D _e1HitSfx;
    [OnReadyGet] private AudioStreamPlayer2D _e2HitSfx;
    [OnReadyGet] private AudioStreamPlayer2D _e3HitSfx;
    [OnReadyGet] private AudioStreamPlayer2D _e1DestroySfx;
    [OnReadyGet] private AudioStreamPlayer2D _e2DestroySfx;
    [OnReadyGet] private AudioStreamPlayer2D _e3DestroySfx;
    [OnReadyGet] private AudioStreamPlayer2D _diceHitSfx;
    [OnReadyGet] private AudioStreamPlayer2D _moveSfx;
    [OnReadyGet] private AudioStreamPlayer2D _modSfx;
    [OnReadyGet] private AudioStreamPlayer2D _oneUpSfx;

    // debug
    [OnReadyGet] private Label _fps;
    [OnReadyGet] private Label _verts;
    [OnReadyGet] private Label _draws;
    
    private List<Enemy> _enemies = new List<Enemy>();
    private int _turnCounter;
    private bool _gameOver;
    private int _lastSpawnedPickup;

    private int _currentWave = -1;
    private bool _spawnedMod = false;
    private bool _collectedMod = false;
    private ModManager.ModTypes _lastPickup;
    private bool _endless;
    private int _showingWavesFor;

    [OnReady]
    private void Ready()
    {
        _dice.OnDestroyed += OnGridObjectDestroyed;
        _dice.OnDamaged += OnGridObjectDamaged;
        _dice.OnHealed += OnHealed;
        _dice.OnCollectPickup += OnCollectPickup;

        Utils.RNG.Seed = (ulong) DateTime.Now.Millisecond;
    }

    private void OnCollectPickup()
    {
        _collectedMod = true;
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
        _moveSfx.Play();
        
        DoPreTurn?.Invoke(); // evaluate the dice move and resulting actions
        DoTurn?.Invoke(); // evaluate enemy movement and attacks
        DoPostTurn?.Invoke(); // evaluate enemy destroyed
        
        ProcessWave();
        
        _turnCounter++;
        _turnsCounter.Text = $"{_turnCounter}";
    }

    private void BeginWave()
    {
        List<Waves.Spawn> spawns = Waves.W[_currentWave];
        List<int> positionsSpawned = new List<int>();
        foreach (Waves.Spawn spawn in spawns)
        {
            int id = (int) spawn.P.x + (int) spawn.P.y * (int) GridSize.y;
            if (!positionsSpawned.Contains(id)) // just in-case a wave double spawns a cell
            {
                SpawnEnemy(spawn.P, spawn.E);
                positionsSpawned.Add(id);
            }
        }

        _waveLabel.Text = $"Wave {_currentWave + 1}/{Waves.W.Count}";
        _waveLabel.Visible = true;
        _showingWavesFor = 3;
    }
    
    private void ProcessWave()
    {
        if (_showingWavesFor-- <= 0)
        {
            _waveLabel.Visible = false;
        }
        
        if (_endless)
        {
            if (_turnCounter % 4 == 0)
            {
                SpawnEnemy(RandPointOnEdge(0), 0);
            }
            
            if (_turnCounter % 7 == 0)
            {
                SpawnEnemy(RandPointOnEdge(0), 1);
            }
            
            if (_turnCounter % 12 == 0)
            {
                SpawnEnemy(RandPointOnEdge(0), 2);
            }

            if (_turnCounter % 30 == 0)
            {
                SpawnOneUp(RandPointOnEdge(2));
            }
            
            if (_turnCounter % 50 == 0)
            {
                SpawnPickup(RandPointOnEdge(2), ModManager.ModTypes.COUNT);
            }
            
            return;
        }
        
        if (_enemies.Count == 0 && !_spawnedMod)
        {
            if (_currentWave == -1)
            {
                SpawnPickup(new Vector2(10, 10), ModManager.ModTypes.Bullet);
            }
            else
            {
                SpawnOneUp(RandPointOnEdge(2));
                SpawnPickup(_dice.GridPos == new Vector2(10, 10) ? new Vector2(11, 10) : new Vector2(10, 10),
                    ModManager.ModTypes.COUNT);
            }
            _spawnedMod = true;
            _collectedMod = false;
        }
        
        if (_enemies.Count == 0 && _spawnedMod && _collectedMod)
        {
            _spawnedMod = false;
            _collectedMod = true;
            _currentWave++;

            if (_currentWave >= Waves.W.Count)
            {
                _endless = true;
                _waveLabel.Visible = true;
                _waveLabel.Text = $"You finished all {Waves.W.Count} waves!\n You took {_turnCounter} turns, try using fewer next time.\n\n" +
                                  $"Entering endless mode (very unbalanced/tested).";
            }
            else
            {
                BeginWave();
            }
        }
    }

    private void SpawnEnemy(Vector2 pos, int type)
    {
        Enemy enemy = _enemyScenes[type].Instance<Enemy>();
        AddChild(enemy);
        _enemies.Add(enemy);
        enemy.Init(pos);
        enemy.OnDestroyed += OnGridObjectDestroyed;
        enemy.OnDamaged += OnGridObjectDamaged;
        enemy.Id = type;
    }

    private Vector2 RandPointOnEdge(int fromEdge)
    {
        Vector2 pos = Vector2.Zero;
        uint rand4 = Utils.RNG.Randi() % 4;
        pos = rand4 switch
        {
            0 => new Vector2(fromEdge, Utils.RNG.Randi() % GridSize.y),
            1 => new Vector2(GridSize.x - 1 - fromEdge, Utils.RNG.Randi() % GridSize.y),
            2 => new Vector2(Utils.RNG.Randi() % GridSize.x, fromEdge),
            3 => new Vector2(Utils.RNG.Randi() % GridSize.x, GridSize.y - 1 - fromEdge),
            _ => pos
        };
        return pos;
    }
    
    private void SpawnPickup(Vector2 gridPos, ModManager.ModTypes type)
    {
        if (type == ModManager.ModTypes.COUNT)
        {
            do
            {
                type = (ModManager.ModTypes) Utils.RNG.RandiRange(1, (int) (ModManager.ModTypes.COUNT - 1));
            } while (type == _lastPickup);
        }
        ModPickup bulletPickup = ModManager.Instance.ModPickupScenes[type].Instance<ModPickup>();
        AddChild(bulletPickup);
        bulletPickup.Init(gridPos, type);
        _lastSpawnedPickup = _turnCounter;
        _lastPickup = type;
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

        if (obj is Dice)
        {
            _diceHitSfx.Play();
        }
        else if (obj is Enemy enemy)
        {
            if (enemy.Id == 1)
                _e1HitSfx.Play();
            if (enemy.Id == 2)
                _e2HitSfx.Play();
            if (enemy.Id == 2)
                _e3HitSfx.Play();
        }
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
            if (enemy.Id == 1)
                _e1DestroySfx.Play();
            if (enemy.Id == 2)
                _e2DestroySfx.Play();
            if (enemy.Id == 2)
                _e3DestroySfx.Play();
            
            _enemies.Remove(enemy);
            //TrySpawnPickup(obj.GridPos, ModManager.ModTypes.COUNT);
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
