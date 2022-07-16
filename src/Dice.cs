using Godot;
using System;
using System.Collections.Generic;
using GodotOnReady.Attributes;

public partial class Dice : GridObject
{
    public static Dice Instance => _instance;
    private static Dice _instance;
    
    [Export] private float _moveTime = 0.25f;
    [Export] private Vector2 _start = new Vector2(5, 5);
    
    [OnReadyGet] private Game _game;
    [OnReadyGet] private Camera _camera;

    public float MoveTime => _moveTime;

    private Queue<Vector2> _moves = new Queue<Vector2>();
    private Queue<Vector2> _rotations = new Queue<Vector2>();
    private Quat _rotFrom;
    private Quat _rot;
    private Vector2 _forward;
    private int _topFace;

    private Dictionary<int, ModDice> _mods = new();
    private Dictionary<ModManager.ModTypes, List<MeshInstance>> _faces = new();
    
    protected override void Ready()
    {
        _gridPos = _start;
        
        base.Ready();
        
        _rot = GlobalTransform.basis.RotationQuat();
        _mods[0] = null;
        _mods[1] = null;
        _mods[2] = null;
        _mods[3] = null;
        _mods[4] = null;
        _mods[5] = null;
        
        _instance = this;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < (int)ModManager.ModTypes.COUNT; j++)
            {
                if (j > 1) break; // until i implement others
                
                ModManager.ModTypes type = (ModManager.ModTypes) j;
                if (!_faces.ContainsKey(type))
                    _faces[type] = new List<MeshInstance>();
                
                List<MeshInstance> list = _faces[type];
                MeshInstance mesh = GetNode<MeshInstance>($"{type.ToString()}{i + 1}");
                list.Add(mesh);
                ShaderMaterial mat = mesh.MaterialOverride as ShaderMaterial;
                mat = mat.Duplicate() as ShaderMaterial; // set unique material so we can change colour
                mesh.MaterialOverride = mat;
                mat.SetShaderParam("u_col_1", ModManager.Instance.ModColours[type]);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            _faces[ModManager.ModTypes.Number][i].Visible = true;
        }
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
                Basis rotBasis = GlobalTransform.basis.Rotated(new Vector3(rot.x, 0.0f, rot.y), Mathf.Pi * 0.5f);
                _rot = rotBasis.RotationQuat();

                _forward = (_gridPos - _moveStart).Normalized();
                
                for (int i = 0; i < 6; i++)
                {
                    if (FaceDirection(i, rotBasis).y > 0.5f) _topFace = i;
                }

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

    protected override void DoPreTurn()
    {
        int bottom = BottomFace();
        if (_mods[bottom] != null)
        {
            _mods[bottom].Activate(_gridPos, _forward);
        }
    }

    public void OnPickup(ModPickup pickup)
    {
        int face = BottomFace();
        SetFaceVisible(face, false);
        
        ModDice mod = new ModDice();
        mod.Type = pickup.Type;
        
        mod.Face = face;
        mod.Activate(_gridPos, _forward);
        _mods[face] = mod;
        
        SetFaceVisible(face, true);
    }

    private void SetFaceVisible(int face, bool visible)
    {
        if (_mods[face] == null)
        {
            _faces[ModManager.ModTypes.Number][face].Visible = visible;
        }
        else
        {
            _faces[_mods[face].Type][face].Visible = visible;
        }
    }
    
    private Vector3 FaceDirection(int face, Basis basis)
    {
        switch (face)
        {
            case 0:
                return -basis.y.Normalized();
            case 1:
                return basis.x.Normalized();
            case 2:
                return -basis.z.Normalized();
            case 3:
                return basis.z.Normalized();
            case 4:
                return -basis.x.Normalized();
            case 5:
                return basis.y.Normalized();
        }

        return Vector3.Zero;
    }

    private int TopFace()
    {
        return _topFace;
    }
    
    private int BottomFace()
    {
        return 7 - (TopFace() + 1) - 1;
    }
}
