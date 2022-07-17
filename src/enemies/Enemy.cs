using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Enemy : GridObject
{
    [Export] private bool _ranged = false;
    [Export] private int _range = 10;

    public int Id;
    
    private bool _attacking;
    private Quat _rotStart;
    private Quat _rotEnd;
    private int _turnCount;
    private int _freezeTurns = 0;
    private bool _unFreeze;

    private List<Particles> _prevBullets = new List<Particles>();

    protected override void DoTurn()
    {
        base.DoTurn();

        foreach (Particles bullet in _prevBullets)
        {
            bullet.QueueFree();
        }
        
        if (_attacking)
            return;

        if (_queueDestroy || _health <= 0)
            return;
        
        if (_unFreeze)
        {
            MaterialOverride = _defaultMaterial;
            _unFreeze = false;
        }

        if (_freezeTurns == 0)
        {
            _turnCount++;
            if (_turnCount % 2 == 0)
            {
                Vector2[] path = Pathfinding.Instance.Evaluate(_gridPos, Dice.Instance.GridPos);
                if (path.Length > 1 && (!_ranged || path.Length > _range))
                {            
                    _moveStart = _gridPos;
                    _gridPos = path[1];
                    _moving = true;
                    _moveTimer = Dice.Instance.MoveTime;
                }

                if (!_moving && _ranged)
                {
                    RangedAttack();
                }
            }
        }

        if (_gridPos == Dice.Instance.GridPos)
        {
            if (_freezeTurns > 0)
            {
                OnHit(_health, ModManager.ModTypes.Freeze);
            }
            else
            {
                _attacking = true;
                _rotStart = GlobalTransform.basis.RotationQuat();
                Vector2 rotAround = _gridPos - _moveStart;
                rotAround = new Vector2(rotAround.y, -rotAround.x);
                _rotEnd = GlobalTransform.basis.Rotated(rotAround.Normalized().To3D(), Mathf.Pi * 0.5f).RotationQuat();

                Dice.Instance.OnHit(Damage, ModManager.ModTypes.Number);
            }
        }
        
        if (_freezeTurns == 1)
        {
            _unFreeze = true;
        }
        _freezeTurns = Mathf.Max(0, _freezeTurns - 1);
            
        Pathfinding.Instance.RegisterPosition(_gridPos);
    }

    private void RangedAttack()
    {
        List<Vector2> dirs = new();
        dirs.Add(new Vector2(1, 0));
        dirs.Add(new Vector2(-1, 0));
        dirs.Add(new Vector2(0, 1));
        dirs.Add(new Vector2(0, -1));

        for (int i = 0; i < 4; i++)
        {
            Particles bulletParticles = ModManager.Instance.BulletParticles.Instance<Particles>();
            Game.Instance.AddChild(bulletParticles);
            bulletParticles.GlobalPosition(Game.Instance.GridToWorld(_gridPos));
            bulletParticles.GlobalRotate(Vector3.Up, dirs[i].Angle() - Mathf.Pi * 0.5f);
        
            // change colour
            SpatialMaterial mat = bulletParticles.MaterialOverride as SpatialMaterial;
            mat = mat.Duplicate() as SpatialMaterial;
            bulletParticles.MaterialOverride = mat;
            mat.AlbedoColor = _defaultMaterial.AlbedoColor;
            bulletParticles.Emitting = true;
            _prevBullets.Add(bulletParticles);
        }

        if (Math.Abs(Dice.Instance.GridPos.x - _gridPos.x) < TOLERANCE ||
            Math.Abs(Dice.Instance.GridPos.y - _gridPos.y) < TOLERANCE)
        {
            Dice.Instance.OnHit(Damage, ModManager.ModTypes.Number);
        }
    }

    private const double TOLERANCE = 0.001f;

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_queueDestroy)
            return;

        if (_moving || _attacking)
        {
            _moveTimer = Mathf.Clamp(_moveTimer - delta, 0.0f, 1.0f);
            float t = Utils.EaseOutCubic(1.0f - (_moveTimer / Dice.Instance.MoveTime));

            float y = GlobalTransform.origin.y;
            Vector3 pos = Game.Instance.GridToWorld(_moveStart).LinearInterpolate(Game.Instance.GridToWorld(_gridPos), t);
            if (_justSpawned)
            {
                pos.y = y;
            }
            else
            {
                pos.y += Mathf.Sin(t * Mathf.Pi) * 0.5f;
            }

            if (_attacking)
            {
                Basis basis = new Basis(_rotStart.Slerp(_rotEnd, t));
                basis.Scale = GlobalTransform.basis.Scale;
                GlobalTransform = new Transform(basis, GlobalTransform.origin);
                
                if (_moveTimer == 0.0f)
                {
                    Destroy();
                }
            }
            
            GlobalTransform = new Transform(GlobalTransform.basis, pos);
            
            if (_moveTimer == 0.0f)
            {
                _moving = false;
            }
        }
    }

    public void Freeze(int turns, bool destroyIfDamaged)
    {
        _freezeTurns = turns;
        if (destroyIfDamaged && _health < _maxHealth)
        {
            OnHit(_health, ModManager.ModTypes.Freeze);
        }
        MaterialOverride = Resources.Instance.FreezeMaterial;
        _unFreeze = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        foreach (Particles bullet in _prevBullets)
        {
            bullet.QueueFree();
        }
    }
}