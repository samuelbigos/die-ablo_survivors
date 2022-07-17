using Godot;
using System;

public class FloatingText : Label
{
    [Export] private float _duration = 1.0f;

    private float _timer;
    private Spatial _obj;
    private Vector3 _cachedPos;
    private Camera _camera;
    private bool _persist;
    
    public void Init(Camera camera, Spatial obj, string text, Color col, bool persist = false)
    {
        _camera = camera;
        _obj = obj;
        Text = text;
        Modulate = col;
        _timer = _duration;
        _persist = persist;
        UpdatePos();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_persist)
        {
            _timer += delta;
        }
        else
        {
            _timer -= delta;
            if (_timer < 0.0f)
            {
                QueueFree();
            }
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
        if (_persist)
        {
            screen = _camera.UnprojectPosition(_cachedPos + Vector3.Up * Mathf.Sin(_timer * 2.0f) * 0.5f + Vector3.Up * 1.0f);
        }
        screen -= RectSize * 0.5f;
        RectGlobalPosition = screen;
    }
}
