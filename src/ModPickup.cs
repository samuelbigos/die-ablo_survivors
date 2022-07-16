using Godot;
using System;
using GodotOnReady.Attributes;

public partial class ModPickup : Spatial
{
    [OnReadyGet] private MeshInstance _mesh1;
    [OnReadyGet] private MeshInstance _mesh2;
    
    public ModManager.ModTypes Type => _type;

    private ModManager.ModTypes _type;
    private Vector2 _gridPos;

    public void Init(Vector2 gridPos, ModManager.ModTypes type)
    {
        _type = type;
        _gridPos = gridPos;
        GlobalTransform = new Transform(GlobalTransform.basis, _gridPos.To3D());
        
        ShaderMaterial shader1 = (_mesh1.MaterialOverride as ShaderMaterial).Duplicate() as ShaderMaterial;
        shader1.SetShaderParam("u_col_2", ModManager.Instance.ModColours[type]);
        _mesh1.MaterialOverride = shader1;
        
        SpatialMaterial shader2 = (_mesh2.MaterialOverride as SpatialMaterial).Duplicate() as SpatialMaterial;
        shader2.AlbedoColor = ModManager.Instance.ModColoursSecondary[type];
        _mesh2.MaterialOverride = shader2;
        
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
