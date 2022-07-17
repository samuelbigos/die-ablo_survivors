using Godot;
using System;

public class FloatingText : Label
{
    [Export] private float _duration = 1.0f;

    private float _timer;
    private GridObject _obj;
    private Vector3 _cachedPos;
    private Camera _camera;
    
    public void Init(Camera camera, GridObject obj, string text, Color col)
    {
        _camera = camera;
        _obj = obj;
        Text = text;
        Modulate = col;
        _timer = _duration;
        UpdatePos();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        _timer -= delta;
        if (_timer < 0.0f)
        {
            QueueFree();
        }
        
        UpdatePos();
    }

    private void UpdatePos()
    {
        if (IsInstanceValid(_obj))
        {
            _cachedPos = _obj.GlobalTransform.origin;
        }
        Vector2 screen = _camera.UnprojectPosition(_cachedPos + Vector3.Up * Utils.EaseOutCubic(1.0f - (_timer / _duration)) * 3.0f);
        screen -= RectSize * 0.5f;
        RectGlobalPosition = screen;
    }
}
