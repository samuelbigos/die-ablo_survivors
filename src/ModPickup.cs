using Godot;
using System;
using GodotOnReady.Attributes;

public partial class ModPickup : Spatial
{
    [OnReadyGet] private MeshInstance _mesh;

    public ModManager.ModTypes Type => _type;

    private ModManager.ModTypes _type;
    private Vector2 _gridPos;

    public void Init(Vector2 gridPos, ModManager.ModTypes type)
    {
        _type = type;
        _gridPos = gridPos;
        GlobalTransform = new Transform(GlobalTransform.basis, _gridPos.To3D());
        
        ShaderMaterial shader = _mesh.MaterialOverride as ShaderMaterial;
        shader.SetShaderParam("u_col_2", ModManager.Instance.ModColours[type]);
        
        Game.DoTurn += DoTurn;
    }

    private void DoTurn()
    {
        if (_gridPos == Dice.Instance.GridPos)
        {
            Dice.Instance.OnPickup(this);
            QueueFree();
            Game.DoTurn -= DoTurn;
        }
    }
}
