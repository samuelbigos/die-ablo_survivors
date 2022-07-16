using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Dice : MeshInstance
{
    public static Dice Instance => _instance;
    private static Dice _instance;
    
    [Export] private float _moveTime = 0.25f;
    [Export] private Vector2 _start = new Vector2(5, 5);
    
    [OnReadyGet] private Game _game;
    [OnReadyGet] private Camera _camera;

    public Vector2 GridPos => _gridPos;
    public float MoveTime => _moveTime;

    private Queue<Vector2> _moves = new Queue<Vector2>();
    private Queue<Vector2> _rotations = new Queue<Vector2>();
    private Vector2 _gridPos;
    private Vector2 _moveStart;
    private bool _moving;
    private float _moveTimer;
    private Quat _rotFrom;
    private Quat _rot;

    [OnReady] 
    private void Ready()
    {
        _gridPos = _start;
        this.GlobalPosition(_game.GridToWorld(_gridPos));

        _rot = GlobalTransform.basis.RotationQuat();

        _instance = this;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("w"))
        {
            _moves.Enqueue(new Vector2(0, -1));
            _rotations.Enqueue(new Vector2(-1, 0));
        }
        if (Input.IsActionJustPressed("a"))
        {
            _moves.Enqueue(new Vector2(-1, 0));
            _rotations.Enqueue(new Vector2(0, 1));
        }
        if (Input.IsActionJustPressed("s"))
        {
            _moves.Enqueue(new Vector2(0, 1));
            _rotations.Enqueue(new Vector2(1, 0));
        }
        if (Input.IsActionJustPressed("d"))
        {
            _moves.Enqueue(new Vector2(1, 0));
            _rotations.Enqueue(new Vector2(0, -1));
        }

        if (_moves.Count > 0)
        {
            Vector2 move = _moves.Dequeue();
            Vector2 rot = _rotations.Dequeue();
            
            if (_game.InBounds(_gridPos + move))
            {
                // apply any incomplete rotation.
                Basis basis = new Basis(_rot);
                basis.Scale = GlobalTransform.basis.Scale;
                GlobalTransform = new Transform(basis, GlobalTransform.origin);
                
                _moveStart = _gridPos;
                _gridPos += move;
                
                _moveTimer = _moveTime;
                _moving = true;

                _rotFrom = GlobalTransform.basis.RotationQuat();
                _rot = GlobalTransform.basis.Rotated(new Vector3(rot.x, 0.0f, rot.y), Mathf.Pi * 0.5f).RotationQuat();

                _game.TriggerTurn();
            }
        }

        if (_moving)
        {
            _moveTimer = Mathf.Clamp(_moveTimer - delta, 0.0f, 1.0f);
            float t = Utils.EaseOutCubic(1.0f - (_moveTimer / _moveTime));
            
            Basis rot = new Basis(_rotFrom.Slerp(_rot, t));
            rot.Scale = GlobalTransform.basis.Scale;
            Vector3 pos = _game.GridToWorld(_moveStart).LinearInterpolate(_game.GridToWorld(_gridPos), t);
            GlobalTransform = new Transform(rot, pos);
            
            if (_moveTimer == 0.0f)
            {
                _moving = false;
            }
        }
        
        _camera.GlobalPosition(GlobalTransform.origin + _camera.GlobalTransform.basis.z.Normalized() * 10.0f);
    }

    private Vector3 FaceDirection(int face)
    {
        Basis basis = GlobalTransform.basis;
        switch (face)
        {
            case 1:
                return -basis.y.Normalized();
            case 2:
                return basis.x.Normalized();
            case 3:
                return -basis.z.Normalized();
            case 4:
                return basis.z.Normalized();
            case 5:
                return -basis.x.Normalized();
            case 6:
                return basis.y.Normalized();
        }

        return Vector3.Zero;
    }

    private int TopFace()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (FaceDirection(i).y > 0.5f) return i;
        }

        return 0;
    }
    
    private int BottomFace()
    {
        return 7 - TopFace();
    }
}
