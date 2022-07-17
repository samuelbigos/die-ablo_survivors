using Godot;
using System;
using GodotOnReady.Attributes;

public partial class Enemy : GridObject
{
    private SpatialMaterial _defaultMaterial;
    
    private bool _attacking;
    private Quat _rotStart;
    private Quat _rotEnd;
    private int _turnCount;

    protected override void DoTurn()
    {
        base.DoTurn();
        
        if (_attacking)
            return;

        if (_queueDestroy || _health <= 0)
            return;

        _turnCount++;
        if (_turnCount % 2 == 0)
        {
            Vector2[] path = Pathfinding.Instance.Evaluate(_gridPos, Dice.Instance.GridPos);
            if (path.Length > 1)
            {            
                _moveStart = _gridPos;
                _gridPos = path[1];
                _moving = true;
                _moveTimer = Dice.Instance.MoveTime;
            }
        }

        if (_gridPos == Dice.Instance.GridPos)
        {
            _attacking = true;
            _rotStart = GlobalTransform.basis.RotationQuat();
            Vector2 rotAround = _gridPos - _moveStart;
            rotAround = new Vector2(rotAround.y, -rotAround.x);
            _rotEnd = GlobalTransform.basis.Rotated(rotAround.Normalized().To3D(), Mathf.Pi * 0.5f).RotationQuat();

            Dice.Instance.OnHit(Damage, ModManager.ModTypes.Number);
        }
            
        Pathfinding.Instance.RegisterPosition(_gridPos);
    }

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
}