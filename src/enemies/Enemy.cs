using Godot;
using System;
using GodotOnReady.Attributes;

public partial class Enemy : MeshInstance
{
    [Export] private Color _color;

    private SpatialMaterial _mat;
    
    private Vector2 _gridPos;
    private Vector2 _moveStart;
    private bool _moving;
    private float _moveTimer;
    
    [OnReady]
    public void Ready()
    {
        _mat = GetActiveMaterial(0) as SpatialMaterial;
        _mat = _mat.Duplicate(false) as SpatialMaterial;
        MaterialOverride = _mat;
        _mat.AlbedoColor = _color;
        
        Game.DoTurn += DoTurn;
    }

    public void Init(Vector2 gridPos)
    {
        _gridPos = gridPos;
        GlobalTransform = new Transform(GlobalTransform.basis, _gridPos.To3D());
    }

    private void DoTurn()
    {
        Vector2[] path = Pathfinding.Instance.Evaluate(_gridPos, Dice.Instance.GridPos);
        if (path.Length > 0)
        {            
            _moveStart = _gridPos;
            _gridPos = path[1];
            _moving = true;
            _moveTimer = Dice.Instance.MoveTime;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_moving)
        {
            _moveTimer = Mathf.Clamp(_moveTimer - delta, 0.0f, 1.0f);
            float t = Utils.EaseOutCubic(1.0f - (_moveTimer / Dice.Instance.MoveTime));
            
            Vector3 pos = Game.Instance.GridToWorld(_moveStart).LinearInterpolate(Game.Instance.GridToWorld(_gridPos), t);
            pos.y += Mathf.Sin(t * Mathf.Pi);
            GlobalTransform = new Transform(GlobalTransform.basis, pos);
            
            if (_moveTimer == 0.0f)
            {
                _moving = false;
            }
        }
    }
}